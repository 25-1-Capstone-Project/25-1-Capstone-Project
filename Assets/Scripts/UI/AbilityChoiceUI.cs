using UnityEngine;
using UnityEngine.UI;

public class AbilityChoiceUI : MonoBehaviour
{
    [Header("UI�������")]
    public Image abilityIconImage;
    public Text abilityNameText;
    public Text abilityDetailText;

    private AbilityData data;
    private System.Action<AbilityData> onClick;

    private bool isBlinking = false;
    public float blinkSpeed = 3f;


    void Update()
    {
        if (isBlinking && abilityIconImage != null)
        {
            float alpha = (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f; // 0~1 사이 값
            Color c = abilityIconImage.color;
            c.a = alpha;
            abilityIconImage.color = c;
        }

    }

    public void Setup(AbilityData data, System.Action<AbilityData> onClickCallback, bool isRecommended)
    {
        this.data = data;
        onClick = onClickCallback;

        // UI ��� �ݿ�
        abilityIconImage.sprite = data.abilityImage;
        abilityNameText.text = data.abilityName;
        abilityDetailText.text = data.abliltyText;

        isBlinking = isRecommended;

        // 깜빡임이 꺼졌을 때 투명도 1로 맞추기
        if (!isBlinking && abilityIconImage != null)
        {
            Color c = abilityIconImage.color;
            c.a = 1f;
            abilityIconImage.color = c;
        }

    }

    public void OnClick()
    {
        onClick?.Invoke(data);
    }

    public void SetBlinking(bool blinking)
    {
        isBlinking = blinking;
    }
    
    public AbilityData GetAbilityData()
    {
        return data;
    }
}
