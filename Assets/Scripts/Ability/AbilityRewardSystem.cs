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
        List<AbilityData> candidates = database.GetUnlockedAbilities(PlayerScript.Instance.GetUnlockedAbilities());
        
        if (candidates == null || candidates.Count == 0) return;

        var choices = candidates.OrderBy(_ => Random.value).Take(3).ToList();

        abilityWindow.SetActive(true);
        GameManager.Instance.SetTimeScale(0f);

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            //Debug.Log("µà¹Ù");
            if (i < choices.Count)
                choiceButtons[i].Setup(choices[i], OnAbilityChosen);
            else
                choiceButtons[i].gameObject.SetActive(false);
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
