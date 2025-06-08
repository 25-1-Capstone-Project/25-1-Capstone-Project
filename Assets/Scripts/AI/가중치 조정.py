import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from sklearn.model_selection import train_test_split
from sklearn.cluster import KMeans
from sklearn.linear_model import LinearRegression
from sklearn.preprocessing import MinMaxScaler, OneHotEncoder
import json

# 1. 소속함수 정의 (경량 삼각형 소속함수 예시)
import numpy as np

# 가우시안 소속함수 정의
def gaussian_mf(x, mean, sigma):
    return np.exp(-0.5 * ((x - mean) / (sigma + 1e-10))**2)


# 2. 데이터 읽기
df = pd.read_csv('fuzzy_attack_training_data.csv')

# distance 퍼지 집합
df['distance_near'] = gaussian_mf(df['distance'], 0.0, 0.15)
df['distance_mid'] = gaussian_mf(df['distance'], 0.5, 0.15)
df['distance_far'] = gaussian_mf(df['distance'], 1.0, 0.15)

# hp 퍼지 집합
df['hp_low'] = gaussian_mf(df['hp'], 0.0, 0.15)
df['hp_medium'] = gaussian_mf(df['hp'], 0.5, 0.15)
df['hp_high'] = gaussian_mf(df['hp'], 1.0, 0.15)

# player_hp 퍼지 집합
df['player_hp_low'] = gaussian_mf(df['player_hp'], 0.0, 0.15)
df['player_hp_medium'] = gaussian_mf(df['player_hp'], 0.5, 0.15)
df['player_hp_high'] = gaussian_mf(df['player_hp'], 1.0, 0.15)

# player_velocity 퍼지 집합
df['player_velocity_low'] = gaussian_mf(df['player_velocity'], 0.0, 0.15)
df['player_velocity_medium'] = gaussian_mf(df['player_velocity'], 0.5, 0.15)
df['player_velocity_high'] = gaussian_mf(df['player_velocity'], 1.0, 0.15)


# 4. 정규화
scaler = MinMaxScaler()
norm_features = scaler.fit_transform(df[["distance", "hp", "player_hp", "player_velocity"]])
df[["distance", "hp", "player_hp", "player_velocity"]] = norm_features

# 5. 클러스터링
features = df[['distance', 'hp', 'player_hp', 'player_velocity']]
kmeans = KMeans(n_clusters=27, random_state=42, n_init=10)
df['cluster'] = kmeans.fit_predict(features)

# 6. action 원-핫 인코딩
encoder = OneHotEncoder(sparse_output=False)
action_onehot = encoder.fit_transform(df[['action']])

# 7. 규칙 생성
rules = []
for cluster_id in range(kmeans.n_clusters):
    cluster_df = df[df['cluster'] == cluster_id].reset_index(drop=True)

    X = cluster_df[["distance", "hp", "player_hp", "player_velocity"]]
    y = action_onehot[df['cluster'] == cluster_id]

    try:
        model = LinearRegression()
        model.fit(X, y)

        rules.append({
            'cluster_id': cluster_id,
            'center': kmeans.cluster_centers_[cluster_id],
            'model': model,
            'sample_count': len(cluster_df)
        })
    except Exception as e:
        print(f"클러스터 {cluster_id} 학습 실패: {e}")

valid_rules = [
    r for r in rules
    if any(abs(c) > 1e-6 for c in r['model'].coef_.flatten())  # 회귀 계수가 모두 0에 가깝지 않은지 확인
    and r['sample_count'] >= 10  # 샘플 수 기준 필터링
]
    

# 8. 추론 엔진 구현
def fuzzy_skill_inference(distance, hp, player_hp, player_velocity, rules, scaler):
    input_df = pd.DataFrame([[distance, hp, player_hp, player_velocity]],
                            columns=['distance', 'hp', 'player_hp', 'player_velocity'])
    input = scaler.transform(input_df)
    skill_scores = np.zeros(4)  # 스킬 4개
    total_weight = 0

    for rule in rules:
        dist = np.linalg.norm(input[0] - rule['center'])
        weight = 1 / (1 + dist)

        pred = rule['model'].predict(pd.DataFrame(input,
                                                  columns=['distance', 'hp', 'player_hp', 'player_velocity']))[0]

        skill_scores += weight * pred
        total_weight += weight

    if total_weight == 0:
        return np.argmax(skill_scores)  # 디폴트 반환

    skill_scores /= total_weight
    return np.argmax(skill_scores)

# 8. 성능 평가
# 입력 특성과 타겟(스킬 번호) 정의
X = df[['distance', 'hp', 'player_hp', 'player_velocity']]
y = df['action'].astype(int)  # 스킬 번호가 정수라고 가정

# 학습/테스트 데이터 분할
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

# 테스트 데이터에 대해 퍼지 추론 실행 및 예측 스킬 산출
y_pred = []
for _, row in X_test.iterrows():
    pred_skill = fuzzy_skill_inference(
        row['distance'], 
        row['hp'], 
        row['player_hp'], 
        row['player_velocity'], 
        valid_rules, 
        scaler
    )
    y_pred.append(pred_skill)

# 정확도 계산
accuracy = np.mean(np.array(y_pred) == y_test.values)
print(f"테스트 정확도: {accuracy:.2%}")

# 9. 테스트
test_inputs = [
    [0.1, 0.9, 0.3, 0.2],  # Slash 예상
    [0.95, 0.6, 0.7, 0.4], # Shot 예상
    [0.6, 0.3, 0.6, 0.5],  # AOE 예상
    [0.5, 0.6, 0.5, 0.95], # JumpSmash 예상
]

for ti in test_inputs:
    pred = fuzzy_skill_inference(*ti, valid_rules, scaler)
    print(f"입력 {ti} → 예측 스킬: {pred} (0=Slash,1=Shot,2=AOE,3=JumpSmash)")




# 클러스터 시각화 (distance_norm vs hp_norm)
plt.figure(figsize=(10, 6))
for rule in valid_rules:
    cluster_data = df[df['cluster'] == rule['cluster_id']]
    plt.scatter(
        cluster_data['distance'], 
        cluster_data['hp'],
        label=f"Cluster {rule['cluster_id']}"
    )
plt.xlabel('Distance (Normalized)')
plt.ylabel('HP (Normalized)')
plt.title('Clustered Data by Valid Rules')
plt.legend()
plt.show()

# 유효 규칙별 샘플 수 시각화
plt.figure(figsize=(10, 6))
plt.bar(range(len(valid_rules)), [r['sample_count'] for r in valid_rules])
plt.xlabel('Rule ID')
plt.ylabel('Number of Samples')
plt.title('Sample Counts per Valid Rule')
plt.show()


"""
# 10. 규칙 저장 (JSON)
export_rules = []
for rule in rules:
    export_rules.append({
        'cluster_id': rule['cluster_id'],
        'center': rule['center'].tolist(),
        'coef': rule['model'].coef_.tolist(),
        'intercept': rule['model'].intercept_.tolist(),
        'sample_count': rule['sample_count']
    })

with open('fuzzy_rules.json', 'w') as f:
    json.dump(export_rules, f, indent=2)

scaler_data = {
    'min': scaler.data_min_.tolist(),
    'max': scaler.data_max_.tolist()
}
with open('fuzzy_scaler.json', 'w') as f:
    json.dump(scaler_data, f, indent=2)
"""
