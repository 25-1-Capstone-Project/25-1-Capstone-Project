import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from sklearn.cluster import KMeans
from sklearn.linear_model import LinearRegression, Ridge
from sklearn.preprocessing import MinMaxScaler
from sklearn.mixture import GaussianMixture
from sklearn.model_selection import train_test_split
from sklearn.metrics import mean_squared_error
import seaborn as sns
from sklearn.preprocessing import LabelEncoder
import json

# 2. 데이터 읽기
df = pd.read_csv('balanced_skill_data.csv')

# 가우시안 소속함수 정의
def gaussian_mf(x, mean, sigma):
    return np.exp(-0.5 * ((x - mean) / (sigma + 1e-10))**2)

# 2. 분위수 기반 가우시안 퍼지 분할
def quantile_fuzzy_partition(data, feature, n_components=3):
    qs = np.linspace(0, 1, n_components)
    means = np.quantile(data[feature], qs)
    if len(means) > 1:
        avg_spacing = np.mean(np.diff(means))
        sigma = avg_spacing / 2
    else:
        sigma = 0.1
    return [(m, sigma) for m in means]


# distance 퍼지 자동 분할
distance_params = quantile_fuzzy_partition(df, 'distance', 3)
df['distance_near'] = gaussian_mf(df['distance'], *distance_params[0])
df['distance_mid'] = gaussian_mf(df['distance'], *distance_params[1])
df['distance_far'] = gaussian_mf(df['distance'], *distance_params[2])

# hp 퍼지 자동 분할
hp_params = quantile_fuzzy_partition(df, 'hp', 3)
df['hp_low'] = gaussian_mf(df['hp'], *hp_params[0])
df['hp_medium'] = gaussian_mf(df['hp'], *hp_params[1])
df['hp_high'] = gaussian_mf(df['hp'], *hp_params[2])

# player_hp 퍼지 자동 분할
player_hp_params = quantile_fuzzy_partition(df, 'player_hp', 3)
df['player_hp_low'] = gaussian_mf(df['player_hp'], *player_hp_params[0])
df['player_hp_medium'] = gaussian_mf(df['player_hp'], *player_hp_params[1])
df['player_hp_high'] = gaussian_mf(df['player_hp'], *player_hp_params[2])

# player_velocity 퍼지 자동 분할
player_velocity = quantile_fuzzy_partition(df, 'player_velocity', 3)
df['player_velocity_low'] = gaussian_mf(df['player_velocity'], *player_velocity[0])
df['player_velocity_medium'] = gaussian_mf(df['player_velocity'], *player_velocity[1])
df['player_velocity_high'] = gaussian_mf(df['player_velocity'], *player_velocity[2])


# 4. 정규화
scaler = MinMaxScaler()
df[['hp_norm', 'player_hp_norm', 'distance_norm', 'player_velocity_norm']] = scaler.fit_transform(
    df[['hp', 'player_hp', 'distance', 'player_velocity']]
)

# 5. 클러스터링
kmeans = KMeans(n_clusters=27, random_state=42, n_init=10)
features = df[['hp_norm', 'player_hp_norm','distance_norm', 'player_velocity_norm']]
kmeans.fit(features)
df['cluster'] = kmeans.labels_


# 7. 규칙 생성
rules = []
for i in range(kmeans.n_clusters):
    cluster_data = df[df['cluster'] == i]

    if len(cluster_data) < 5:
        continue

    X = cluster_data[["distance_norm", "hp_norm", "player_hp_norm", "player_velocity_norm"]]
    y = cluster_data['action'].astype(int)


    try:
        model = Ridge(alpha=0.5)
        model.fit(X, y)

        rules.append({
            'cluster_id': i,
            'center': kmeans.cluster_centers_[i],
            'coeffs': model.coef_.tolist() + [model.intercept_],
            'model': model,
            'n_samples': len(cluster_data)
        })
    except Exception as e:
        print(f"클러스터 {i} 학습 실패: {e}")

# 6. 규칙 필터링
valid_rules = [
    r for r in rules 
    if any(c != 0 for c in r['coeffs'][:3]) and r['n_samples'] >= 10
]
print(f"생성된 규칙: {len(rules)}개, 유효 규칙: {len(valid_rules)}개")
    

# 8. 추론 엔진 구현
def fuzzy_skill_inference(hp, player_hp, distance, player_velocity, rules, scaler):
    input_df = pd.DataFrame([[hp, player_hp, distance, player_velocity]],
                            columns=['hp', 'player_hp', 'distance', 'player_velocity'])
    input_norm = scaler.transform(input_df)[0]

    total_weight = 0
    weighted_sum = 0

    for rule in rules:
        distance = np.linalg.norm(input_norm - rule['center'])
        alpha = 1 / (1 + distance)

        coeffs = rule['coeffs']
        z = (
            coeffs[0] * input_norm[0] +
            coeffs[1] * input_norm[1] +
            coeffs[2] * input_norm[2] +
            coeffs[3]
        )
        weighted_sum += alpha * z
        total_weight += alpha

    if total_weight == 0:
        return 0

    skill_score = weighted_sum / total_weight
    np.clip(skill_score, 0, 3)  # 스킬 번호는 0~3 범위로 제한
    return int(round(skill_score))

# 8. 성능 평가
# 입력 특성과 타겟(스킬 번호) 정의
X = df[['hp', 'player_hp', 'distance', 'player_velocity']]
y = df['action'].astype(int)  # 스킬 번호가 정수라고 가정

# 학습/테스트 데이터 분할
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

# 테스트 데이터에 대해 퍼지 추론 실행 및 예측 스킬 산출
y_pred = []
for _, row in X_test.iterrows():
    pred_skill = fuzzy_skill_inference(
        row['hp'], 
        row['player_hp'],
        row['distance'],
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
    [0.9, 0.3, 0.1, 0.2],  # hp, player_hp, distance, player_velocity
    [0.6, 0.7, 0.95, 0.4], 
    [0.3, 0.6, 0.6, 0.5],  
    [0.6, 0.5, 0.5, 0.95], 
]

for ti in test_inputs:
    pred = fuzzy_skill_inference(ti[0], ti[1], ti[2], ti[3], valid_rules, scaler)
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
plt.bar(range(len(valid_rules)), [r['n_samples'] for r in valid_rules])
plt.xlabel('Rule ID')
plt.ylabel('Number of Samples')
plt.title('Sample Counts per Valid Rule')
plt.show()


import json

export_rules = []
for rule in valid_rules:
    export_rules.append({
        'cluster_id': rule['cluster_id'],
        'center': rule['center'].tolist(),
        'coeffs': rule['coeffs'][:-1],  # 회귀계수
        'intercept': rule['coeffs'][-1], # 절편
        'n_samples': rule['n_samples']
    })

with open('fuzzy_rules.json', 'w') as f:
    json.dump(export_rules, f, indent=2)

scaler_data = {
    'min': scaler.data_min_.tolist(),
    'max': scaler.data_max_.tolist()
}

with open('fuzzy_scaler.json', 'w') as f:
    json.dump(scaler_data, f, indent=2)
