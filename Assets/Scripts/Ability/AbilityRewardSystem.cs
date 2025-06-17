using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AbilityRewardSystem : MonoBehaviour
{
    public AbilityDatabase database;
    public GameObject abilityWindow;
    public AbilityChoiceUI[] choiceButtons;
    private List<string> recommendedAbilities = new List<string>();
    
    public void SetRecommendedAbilities(List<string> recommended)
    {
        recommendedAbilities = recommended;
    }
    public void ShowAbilityChoices()
    {
        List<AbilityData> candidates = database.GetUnlockedAbilities(PlayerScript.Instance.GetUnlockedAbilities());

        if (candidates == null || candidates.Count == 0) return;

        var choices = candidates.OrderBy(_ => Random.value).Take(3).ToList();

        abilityWindow.SetActive(true);
        GameManager.Instance.SetTimeScale(0f);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < choices.Count)
            {
                // 추천 증강 여부 체크
                bool isRecommended = recommendedAbilities.Contains(choices[i].abilityName);
                
                // Setup 함수에 추천 여부를 넘겨서 UI 반짝임 적용 가능
                choiceButtons[i].Setup(choices[i], OnAbilityChosen, isRecommended);
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnAbilityChosen(AbilityData chosen)
    {
        abilityWindow.SetActive(false);
        GameManager.Instance.SetTimeScale(1f);

        var abilityManager = PlayerScript.Instance.GetComponent<AbilityManager>();
        var instance = Instantiate(chosen.abilityPrefab).GetComponent<PlayerAbility>();
        abilityManager.EquipAbility(instance);

        PlayerScript.Instance.RegisterUnlockedAbility(chosen);
    }
}
