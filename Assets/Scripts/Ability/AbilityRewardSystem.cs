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
        // ���� �÷��̾ �ر� ������ �����Ƽ�� ���͸�
        List<AbilityData> candidates = database.GetUnlockedAbilities(PlayerScript.Instance.GetUnlockedAbilities());
        var choices = candidates.OrderBy(_ => Random.value).Take(3).ToList();

        abilityWindow.SetActive(true);
        GameManager.Instance.SetTimeScale(0f); // �Ͻ�����

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
        // â �ݱ� �� �ð� �簳
        abilityWindow.SetActive(false);
        GameManager.Instance.SetTimeScale(1f);

        // �����Ƽ ����
        var abilityManager = PlayerScript.Instance.GetComponent<AbilityManager>();
        var instance = Instantiate(chosen.abilityPrefab).GetComponent<PlayerAbility>();
        abilityManager.EquipAbility(instance);

        // �����Ƽ �ر� ���
        PlayerScript.Instance.RegisterUnlockedAbility(chosen);
    }
}
