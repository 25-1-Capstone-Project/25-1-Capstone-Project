using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UI_BossInfo : MonoBehaviour
{
    [SerializeField] GameObject bossUI;
    [SerializeField] TMP_Text bossNameText;
    [SerializeField] Slider bossHealthBar;

    public void SetActiveBossUI(bool active)
    {
        bossUI.SetActive(active);
    }

    public void SetBossName(string name)
    {
        bossNameText.text = name;
    }

    public void SetBossHealth(float currentHealth, float maxHealth)
    {

        float healthPercentage = currentHealth / maxHealth;
        bossHealthBar.value = healthPercentage;

    }
  
}
