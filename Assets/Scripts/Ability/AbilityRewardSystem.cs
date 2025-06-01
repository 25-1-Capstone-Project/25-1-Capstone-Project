using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AbilityRewardSystem : MonoBehaviour
{
    public AbilityDatabase database;
    public GameObject abilityWindow;
    public AbilityChoiceUI[] choiceButtons;

    public void ShowAbilityChoices()
    {
        // 현재 플레이어가 해금 가능한 어빌리티만 필터링
        List<AbilityData> candidates = database.GetUnlockedAbilities(PlayerScript.Instance.GetUnlockedAbilities());
        var choices = candidates.OrderBy(_ => Random.value).Take(3).ToList();

        abilityWindow.SetActive(true);
        GameManager.Instance.SetTimeScale(0f); // 일시정지

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < choices.Count)
                choiceButtons[i].Setup(choices[i], OnAbilityChosen);
            else
                choiceButtons[i].gameObject.SetActive(false);
        }
    }

    private void OnAbilityChosen(AbilityData chosen)
    {
        // 창 닫기 및 시간 재개
        abilityWindow.SetActive(false);
        GameManager.Instance.SetTimeScale(1f);

        // 어빌리티 장착
        var abilityManager = PlayerScript.Instance.GetComponent<AbilityManager>();
        var instance = Instantiate(chosen.abilityPrefab).GetComponent<PlayerAbility>();
        abilityManager.EquipAbility(instance);

        // 어빌리티 해금 등록
        PlayerScript.Instance.RegisterUnlockedAbility(chosen);
    }
}
