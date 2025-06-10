import pandas as pd
import numpy as np

np.random.seed(42)
n_samples = 1000

# 피처 생성
distance = np.random.uniform(1, 10, n_samples)
player_hp = np.random.uniform(1, 100, n_samples)
hp = np.random.uniform(1, 100, n_samples)
player_velocity = np.random.uniform(1, 5, n_samples)

actions = []

# 카테고리 분류 함수
def hp_level(x):
    if x <= 33:
        return 'low'
    elif x <= 66:
        return 'mid'
    else:
        return 'high'

def vel_level(x):
    if x <= 2:
        return 'slow'
    elif x < 4:
        return 'mid'
    else:
        return 'fast'

def dist_level(x):
    if x <= 3:
        return 'near'
    elif x <= 7:
        return 'mid'
    else:
        return 'far'

# 액션 분류 규칙 기반 분포 균형 맞추기
for i in range(n_samples):
    d = distance[i]
    p_hp = player_hp[i]
    b_hp = hp[i]
    vel = player_velocity[i]

    d_cat = dist_level(d)
    p_hp_cat = hp_level(p_hp)
    b_hp_cat = hp_level(b_hp)
    v_cat = vel_level(vel)

    # 명확한 규칙으로 커버
    if d_cat == 'near' and p_hp_cat in ['mid', 'high'] and b_hp_cat in ['mid', 'high']:
        action = 0  # Slash
    elif d_cat == 'mid' and (v_cat == 'fast' or p_hp_cat == 'low'):
        action = 1  # Shot
    elif d_cat == 'far' and (b_hp_cat == 'low' or v_cat == 'fast'):
        action = 2  # AOE
    elif p_hp_cat == 'low' and b_hp_cat == 'high':
        action = 3  # JumpSmash
    else:
        # 가장 가까운 조건 선택
        if d_cat == 'near':
            action = 0
        elif d_cat == 'mid':
            action = 1
        elif d_cat == 'far':
            action = 2
        else:
            action = 3

    actions.append(action)

# 데이터프레임 구성
df = pd.DataFrame({
    'player_hp': player_hp,
    'hp': hp,
    'distance': distance,
    'player_velocity': player_velocity,
    'action': actions
})

# 클래스 분포 확인
print(df['action'].value_counts(normalize=True))

# 저장
df.to_csv("balanced_skill_data.csv", index=False)
