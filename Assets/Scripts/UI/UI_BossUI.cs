using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_BossUI : MonoBehaviour
{
    [SerializeField] GameObject bossUI;
    [SerializeField] TMP_Text bossNameText;
    [SerializeField] Slider bossHealthBar;
    float maxHealth = 0;
    public void SetActiveBossUI(bool active)
    {
        bossUI.SetActive(active);
    }

    public void SetBossName(string name)
    {
        bossNameText.text = name;
    }

    public void SetBossHealth(float currentHealth)
    {

        float healthPercentage = currentHealth / maxHealth;
        bossHealthBar.value = healthPercentage;

    }
    public void SetBossMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
    }
}
