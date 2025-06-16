using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 25.06.16 안예선
/// 체력바 간단하게 추가 
/// HPBar 통제 클래스 -> EnemyBase에서 호출해 사용
/// 정말 간단하게 하드코딩식으로 한 거라 추후 수정 필요...
/// </summary>
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
