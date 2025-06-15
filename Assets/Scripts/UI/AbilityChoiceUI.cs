using UnityEngine;
using UnityEngine.UI;

public class AbilityChoiceUI : MonoBehaviour
{
    [Header("UI备己夸家")]
    public Image abilityIconImage;
    public Text abilityNameText;
    public Text abilityDetailText;

    private AbilityData data;
    private System.Action<AbilityData> onClick;

    public void Setup(AbilityData data, System.Action<AbilityData> onClickCallback)
    {
        this.data = data;
        onClick = onClickCallback;

        // UI 夸家 馆康
        abilityIconImage.sprite = data.abilityImage;
        abilityNameText.text = data.abilityName;
        abilityDetailText.text = data.abliltyText;
    }


    public void OnClick()
    {
        onClick?.Invoke(data);
    }
}
