using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    public Image skillIconUI;
    public Image cooldownOverlay;

    public void UpdateSkillIcon(Sprite icon)
    {
        if (skillIconUI == null) return;

        if (icon != null)
            skillIconUI.sprite = icon;
        else
            skillIconUI.sprite = null;
    }

    public void UpdateCooldown(float ratio)
    {
        if (cooldownOverlay == null) return;
        cooldownOverlay.fillAmount = ratio;
    }

}
