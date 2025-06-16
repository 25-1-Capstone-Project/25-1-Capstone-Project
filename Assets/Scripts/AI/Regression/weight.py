import numpy as np
import pandas as pd
from sklearn.linear_model import Ridge
from sklearn.preprocessing import MinMaxScaler
from sklearn.model_selection import train_test_split
import matplotlib.pyplot as plt
import json

# 1. 데이터 로드
df = pd.read_csv('balanced_skill_data.csv')

# 2. 가우시안 소속함수 정의
def gaussian_mf(x, mean, sigma):
    return np.exp(-0.5 * ((x - mean) / (sigma + 1e-10))**2)

# 3. 분위수 기반 퍼지 분할
def quantile_fuzzy_partition(data, feature, n_components=3):
    qs = np.linspace(0, 1, n_components)
    means = np.quantile(data[feature], qs)
    if len(means) > 1:
        avg_spacing = np.mean(np.diff(means))
        sigma = avg_spacing / 2
    else:
        sigma = 0.1
    return [(m, sigma) for m in means]

# 4. 정규화
scaler = MinMaxScaler()
df[['hp_norm', 'player_hp_norm', 'distance_norm', 'player_velocity_norm']] = scaler.fit_transform(
    df[['hp', 'player_hp', 'distance', 'player_velocity']]
)

# 5. 퍼지 멤버십 계산
labels = ['low', 'medium', 'high']

distance_params = quantile_fuzzy_partition(df, 'distance_norm', 3)
for i, lbl in enumerate(labels):
    df[f'distance_{lbl}'] = gaussian_mf(df['distance_norm'], *distance_params[i])

hp_params = quantile_fuzzy_partition(df, 'hp_norm', 3)
for i, lbl in enumerate(labels):
    df[f'hp_{lbl}'] = gaussian_mf(df['hp_norm'], *hp_params[i])

player_hp_params = quantile_fuzzy_partition(df, 'player_hp_norm', 3)
for i, lbl in enumerate(labels):
    df[f'player_hp_{lbl}'] = gaussian_mf(df['player_hp_norm'], *player_hp_params[i])

velocity_params = quantile_fuzzy_partition(df, 'player_velocity_norm', 3)
for i, lbl in enumerate(labels):
    df[f'player_velocity_{lbl}'] = gaussian_mf(df['player_velocity_norm'], *velocity_params[i])

# 6. 퍼지 규칙 생성 (고전적 IF-THEN 방식)
rules = []
rule_id = 0

for d_lbl in labels:
    for h_lbl in labels:
        for ph_lbl in labels:
            for v_lbl in labels:
                condition = (
                    (df[f'distance_{d_lbl}'] > 0.5) &
                    (df[f'hp_{h_lbl}'] > 0.5) &
                    (df[f'player_hp_{ph_lbl}'] > 0.5) &
                    (df[f'player_velocity_{v_lbl}'] > 0.5)
                )
                subset = df[condition]

                if len(subset) < 10:
                    continue  # 최소 샘플 수 조건

                X = subset[['distance_norm', 'hp_norm', 'player_hp_norm', 'player_velocity_norm']]
                y = subset['action'].astype(int)

                model = Ridge(alpha=0.5)
                model.fit(X, y)

                # 각 규칙의 중심점 계산 (퍼지 추론에서 사용)
                center = np.array([
                    distance_params[labels.index(d_lbl)][0],
                    hp_params[labels.index(h_lbl)][0], 
                    player_hp_params[labels.index(ph_lbl)][0],
                    velocity_params[labels.index(v_lbl)][0]
                ])
                
                # 정규화된 중심점
                center_df = pd.DataFrame([center], columns=['hp', 'player_hp', 'distance', 'player_velocity'])
                center_norm = scaler.transform(center_df)[0]


                rules.append({
                    'rule_id': rule_id,
                    'fuzzy_condition': {
                        'distance': d_lbl,
                        'hp': h_lbl,
                        'player_hp': ph_lbl,
                        'player_velocity': v_lbl
                    },
                    'center': center,
                    'center_norm': center_norm,
                    'coeffs': model.coef_.tolist(),
                    'intercept': model.intercept_,
                    'n_samples': len(subset),
                    'params': {
                        'distance': distance_params[labels.index(d_lbl)],
                        'hp': hp_params[labels.index(h_lbl)],
                        'player_hp': player_hp_params[labels.index(ph_lbl)],
                        'player_velocity': velocity_params[labels.index(v_lbl)]
                    }
                })
                rule_id += 1

# 7. IF-THEN 형태로 출력 (처음 5개만)
print("\n=== 퍼지 규칙 예시 (처음 5개) ===")
for r in rules[:5]:
    c = r['fuzzy_condition']
    co = r['coeffs']
    inter = r['intercept']
    print(f"""
RULE {r['rule_id']:02d}:
IF distance is {c['distance']} AND
   hp is {c['hp']} AND
   player_hp is {c['player_hp']} AND
   player_velocity is {c['player_velocity']}
THEN
   skill_score = {co[0]:.3f} * distance_norm +
                 {co[1]:.3f} * hp_norm +
                 {co[2]:.3f} * player_hp_norm +
                 {co[3]:.3f} * player_velocity_norm +
                 {inter:.3f}
   # {r['n_samples']}개 샘플 기반
""")

# 8. Sugeno 방식 추론 엔진 구현 (이미지와 동일한 방식)
def fuzzy_skill_inference(hp, player_hp, distance, player_velocity, rules, scaler):
    # 입력 데이터를 DataFrame으로 만들어서 컬럼명 명시
    input_data = pd.DataFrame([[hp, player_hp, distance, player_velocity]], 
                              columns=['hp', 'player_hp', 'distance', 'player_velocity'])
    input_norm = scaler.transform(input_data)[0]
    
    total_weight = 0
    weighted_sum = 0
    
    for rule in rules:
        distance_membership = gaussian_mf(input_norm[2], *rule['params']['distance'])
        hp_membership = gaussian_mf(input_norm[0], *rule['params']['hp'])
        player_hp_membership = gaussian_mf(input_norm[1], *rule['params']['player_hp'])
        velocity_membership = gaussian_mf(input_norm[3], *rule['params']['player_velocity'])
        
        w_i = distance_membership * hp_membership * player_hp_membership * velocity_membership
        
        if w_i > 0.001:
            coeffs = rule['coeffs']
            z_i = (coeffs[0] * input_norm[0] + 
                   coeffs[1] * input_norm[1] + 
                   coeffs[2] * input_norm[2] + 
                   coeffs[3] * input_norm[3] + 
                   rule['intercept'])
            
            weighted_sum += w_i * z_i
            total_weight += w_i
    
    if total_weight == 0:
        return 0
    
    skill_score = weighted_sum / total_weight
    skill_score = np.clip(skill_score, 0, 3)
    return int(round(skill_score))

# 9. 성능 평가
print("\n=== 성능 평가 ===")
X = df[['hp', 'player_hp', 'distance', 'player_velocity']]
y = df['action'].astype(int)

# 학습/테스트 데이터 분할
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

# 테스트 데이터에 대해 퍼지 추론 실행
y_pred = []
for _, row in X_test.iterrows():
    pred_skill = fuzzy_skill_inference(
        row['hp'], 
        row['player_hp'],
        row['distance'],
        row['player_velocity'], 
        rules,  # valid_rules -> rules로 수정
        scaler
    )
    y_pred.append(pred_skill)

# 정확도 계산
accuracy = np.mean(np.array(y_pred) == y_test.values)
print(f"테스트 정확도: {accuracy:.2%}")

# 혼동 행렬 출력
from sklearn.metrics import confusion_matrix, classification_report
cm = confusion_matrix(y_test, y_pred)
print("\n혼동 행렬:")
print(cm)
print("\n분류 보고서:")
print(classification_report(y_test, y_pred, target_names=['Slash', 'Shot', 'AOE', 'JumpSmash']))

# 10. Sugeno 방식 상세 추론 예시
print("\n=== Sugeno 추론 과정 상세 예시 ===")
def detailed_fuzzy_inference(hp, player_hp, distance, player_velocity, rules, scaler, show_details=True):
    input_data = pd.DataFrame([[hp, player_hp, distance, player_velocity]],
                              columns=['hp', 'player_hp', 'distance', 'player_velocity'])
    input_norm = scaler.transform(input_data)[0]
    
    if show_details:
        print(f"입력값: HP={hp}, Player_HP={player_hp}, Distance={distance}, Velocity={player_velocity}")
        print(f"정규화: {input_norm}")
    
    total_weight = 0
    weighted_sum = 0
    active_rules = []
    
    for rule in rules:
        d_mem = gaussian_mf(distance, *rule['params']['distance'])
        h_mem = gaussian_mf(hp, *rule['params']['hp'])
        ph_mem = gaussian_mf(player_hp, *rule['params']['player_hp'])
        v_mem = gaussian_mf(player_velocity, *rule['params']['player_velocity'])
        
        w_i = d_mem * h_mem * ph_mem * v_mem
        
        if w_i > 0.001:
            coeffs = rule['coeffs']
            z_i = (coeffs[0] * input_norm[0] + coeffs[1] * input_norm[1] + 
                   coeffs[2] * input_norm[2] + coeffs[3] * input_norm[3] + rule['intercept'])
            
            weighted_sum += w_i * z_i
            total_weight += w_i
            
            if show_details and len(active_rules) < 3:
                cond = rule['fuzzy_condition']
                active_rules.append({
                    'rule_id': rule['rule_id'],
                    'condition': f"distance={cond['distance']}, hp={cond['hp']}, player_hp={cond['player_hp']}, velocity={cond['player_velocity']}",
                    'memberships': [d_mem, h_mem, ph_mem, v_mem],
                    'weight': w_i,
                    'output': z_i
                })
    
    if show_details:
        print(f"\n활성화된 상위 규칙들:")
        for rule_info in active_rules:
            print(f"  규칙 {rule_info['rule_id']:02d}: {rule_info['condition']}")
            print(f"    멤버십: {[f'{m:.3f}' for m in rule_info['memberships']]}")
            print(f"    가중치 w_i = {rule_info['weight']:.4f}")
            print(f"    출력 z_i = {rule_info['output']:.3f}")
        
        print(f"\n최종 계산:")
        print(f"  총 가중치: Σw_i = {total_weight:.4f}")
        print(f"  가중합: Σ(w_i × z_i) = {weighted_sum:.4f}")
    
    if total_weight == 0:
        return 0
    
    skill_score = weighted_sum / total_weight
    skill_score = np.clip(skill_score, 0, 3)
    
    if show_details:
        print(f"  최종 출력: z̄ = {weighted_sum:.4f} / {total_weight:.4f} = {skill_score:.4f}")
        print(f"  스킬 번호: {int(round(skill_score))}")
    
    return int(round(skill_score))


# 기술 0: Slash
test_case_0 = [0.6, 0.4, 0.3, 0.5]

# 기술 1: Shot
test_case_1 = [0.2, 0.8, 0.7, 0.3]

# 기술 2: AOE
test_case_2 = [0.9, 0.6, 0.2, 0.8]

# 기술 3: JumpSmash
test_case_3 = [0.4, 0.5, 0.9, 0.2]

print("\n--- 기술 0 (Slash) 테스트 ---")
detailed_fuzzy_inference(*test_case_0, rules, scaler, show_details=True)

print("\n--- 기술 1 (Shot) 테스트 ---")
detailed_fuzzy_inference(*test_case_1, rules, scaler, show_details=True)

print("\n--- 기술 2 (AOE) 테스트 ---")
detailed_fuzzy_inference(*test_case_2, rules, scaler, show_details=True)

print("\n--- 기술 3 (JumpSmash) 테스트 ---")
detailed_fuzzy_inference(*test_case_3, rules, scaler, show_details=True)

export_rules = []
for rule in rules:  # 여기 수정
    export_rules.append({
        'rule_id': rule['rule_id'],  # 원래 'cluster_id' 대신 rule_id가 있습니다
        'center': rule['center'],
        'coeffs': rule['coeffs'],
        'intercept': rule['intercept'],
        'n_samples': rule['n_samples']
    })

scaler_data = {
    'min': scaler.data_min_.tolist(),
    'max': scaler.data_max_.tolist(),
    'feature_names': ['hp', 'player_hp', 'distance', 'player_velocity']
}

with open('fuzzy_rules.json', 'w') as f:
    json.dump(export_rules, f, indent=2)

with open('fuzzy_scaler.json', 'w') as f:
    json.dump(scaler_data, f, indent=4)