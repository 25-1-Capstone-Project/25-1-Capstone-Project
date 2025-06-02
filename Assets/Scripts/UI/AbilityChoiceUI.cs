using UnityEngine;
using UnityEngine.UI;

public class AbilityChoiceUI : MonoBehaviour
{
    public Image abilityIcon;
    private AbilityData data;
    private System.Action<AbilityData> onClick;

    public void Setup(AbilityData data, System.Action<AbilityData> onClickCallback)
    {
        this.data = data;
        abilityIcon.sprite = data.abilityImage;
        onClick = onClickCallback;
    }

    public void OnClick()
    {
        onClick?.Invoke(data);
    }
}
