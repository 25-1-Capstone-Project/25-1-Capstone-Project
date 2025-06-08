import numpy as np
import pandas as pd

np.random.seed(42)

n_per_class = 250
total_samples = n_per_class * 4

data = []

for action in range(4):
    distance = np.random.uniform(0, 1, n_per_class)
    hp = np.random.uniform(0, 1, n_per_class)
    player_hp = np.random.uniform(0, 1, n_per_class)
    player_velocity = np.random.uniform(0, 1, n_per_class)
    
    if action == 0:
        distance = np.clip(np.random.normal(0.2, 0.1, n_per_class), 0, 1)
        hp = np.clip(np.random.normal(0.8, 0.1, n_per_class), 0, 1)
    elif action == 1:
        distance = np.clip(np.random.normal(0.9, 0.05, n_per_class), 0, 1)
        hp = np.clip(np.random.normal(0.6, 0.1, n_per_class), 0, 1)
    elif action == 2:
        player_hp = np.clip(np.random.normal(0.5, 0.15, n_per_class), 0, 1)
        hp = np.clip(np.random.normal(0.4, 0.1, n_per_class), 0, 1)
    else:
        player_velocity = np.clip(np.random.normal(0.9, 0.1, n_per_class), 0, 1)
        hp = np.clip(np.random.normal(0.6, 0.1, n_per_class), 0, 1)

    for i in range(n_per_class):
        data.append([distance[i], hp[i], player_hp[i], player_velocity[i], action])

df_balanced = pd.DataFrame(data, columns=["distance", "hp", "player_hp", "player_velocity", "action"])

file_path = "/Users/bag-yunsu/fuzzy_attack_training_data.csv"
df_balanced.to_csv(file_path, index=False)

file_path
