using UnityEngine;
using UnityEngine.UI;

public class AbilityChoiceUI : MonoBehaviour
{
    //public Text abilityNameText;
    public Image abilityIcon;
    private AbilityData data;
    private System.Action<AbilityData> onClick;

    public void Setup(AbilityData data, System.Action<AbilityData> onClickCallback)
    {
        Debug.Log("버튼생성완");
        this.data = data;
        //abilityNameText.text = data.abilityName;
        abilityIcon.sprite = data.abilityImage;
        onClick = onClickCallback;
    }

    public void OnClick()
    {
        onClick?.Invoke(data);
    }
}
