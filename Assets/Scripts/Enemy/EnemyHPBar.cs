using UnityEngine;
using UnityEngine.UI;


public class EnemyHPBar : MonoBehaviour
{
    [Header("HP Bar")]
    public Slider healthSlider;
    public GameObject healthBarCanvas;

    public void SetHealth(float current, float max)
    {
        if (!healthBarCanvas.activeSelf) Show();
        if (healthSlider != null)
        {
            healthSlider.value = current / max;
        }
    }

    public void Show()
    {
        if (healthBarCanvas != null)
        {
            healthSlider.value = 1f;
            healthBarCanvas.SetActive(true);
        }
    }

    public void Hide()
    {
        if (healthBarCanvas != null)
        {
            healthBarCanvas.SetActive(false);
        }
    }
}
