using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UI_PlayerStatus : MonoBehaviour
{
    [Header("HP Settings")]
    [SerializeField] private Image hpPrefab;
    [SerializeField] private Transform hpRoot;

    // 0: 빈하트 / 1: 찬하트
    [SerializeField] private Sprite[] hpSprites;

    [Header("Layout Settings")]
    [SerializeField] private float startX = 15.0f;
    [SerializeField] private float startY = -15.0f;
    [SerializeField] private float spacingX = 36.8f;

    private List<Image> hpIcons = new List<Image>();

    public void HPUIInit(int maxHP)
    {
        foreach (var icon in hpIcons)
        {
            if (icon != null) Destroy(icon.gameObject);
        }
        hpIcons.Clear();

        Transform parent = hpRoot != null ? hpRoot : transform;

        for (int i = 0; i < maxHP; i++)
        {
            Image newIcon = Instantiate(hpPrefab, parent);

            RectTransform rect = newIcon.rectTransform;
            rect.anchorMin = new Vector2(0.0f, 1.0f);
            rect.anchorMax = new Vector2(0.0f, 1.0f);
            rect.pivot = new Vector2(0.0f, 1.0f);
            rect.anchoredPosition = new Vector3(startX + (spacingX * i), startY, 0.0f);

            newIcon.sprite = hpSprites.Length > 1 ? hpSprites[1] : newIcon.sprite;

            hpIcons.Add(newIcon);
        }
    }

    public void UI_HPBarUpdate(int currentHP, int maxHP)
    {
        if (hpIcons.Count == 0 || hpSprites.Length < 2) return;

        // c처음부터 검사 -> 확인하고 채우는 방식으로
        for (int i = 0; i < hpIcons.Count; i++)
        {
            if (i < currentHP)
            {
                hpIcons[i].sprite = hpSprites[1];
            }
            else
            {
                hpIcons[i].sprite = hpSprites[0];
            }
        }
    }

    public void UI_ParryCooldownUpdate()
    {
        //cooldownImage.fillAmount =  GameManager.Instance.playerScript.ParryCooldownRatio;
        //Vector3 worldPos =  GameManager.Instance.playerScript.transform.position + uiOffset;
        //cooldownUI.position = Camera.main.WorldToScreenPoint(worldPos);
    }
}