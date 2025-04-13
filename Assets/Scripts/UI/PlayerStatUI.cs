using UnityEngine;
using UnityEngine.UI;
public class PlayerStatUI : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private RectTransform cooldownUI;
    [SerializeField] private Vector3 uiOffset = new Vector3(0.6f, 0.8f, 0f);

    [SerializeField] private Slider hpBar;
    [SerializeField] private Image cooldownImage;

    public void UI_HPBarUpdate()
    {
        hpBar.value = (float)player.UIHealth / (float)player.UIMaxHealth;
    }
  
    public void UI_ParryCooldownUpdate()
    {
        cooldownImage.fillAmount = player.ParryCooldownRatio;
        Vector3 worldPos = player.transform.position + uiOffset;
        cooldownUI.position = Camera.main.WorldToScreenPoint(worldPos);
    }

}
