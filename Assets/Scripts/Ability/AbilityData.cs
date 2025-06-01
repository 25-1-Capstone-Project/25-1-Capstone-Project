using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Player/Ability/AbilityData")]
public class AbilityData : ScriptableObject
{
    public string abilityName;
    public Sprite abilityImage;
    public GameObject abilityPrefab;
    public List<AbilityData> prerequisites;
    public int maxLevel = 3;
}
