using UnityEngine;
using UnityEngine.UI;

public class PrototypeUIManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private RectTransform cooldownUI;
    [SerializeField] private Vector3 uiOffset = new Vector3(0.6f, 0.8f, 0f);

    [SerializeField] private Text parryStackText;
    [SerializeField] private Slider hpBar;
    [SerializeField] private Image cooldownImage;


    private void Update()
    {
        parryStackText.text = player.ParryStack.ToString();
        hpBar.value = (float)player.UIHealth / (float)player.UIMaxHealth;

        cooldownImage.fillAmount = player.ParryCooldownRatio;

        Vector3 worldPos = playerTransform.position + uiOffset;
        cooldownUI.position = Camera.main.WorldToScreenPoint(worldPos);
    }
}
