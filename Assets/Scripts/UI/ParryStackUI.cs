using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ParryStackUI : MonoBehaviour
{
    [SerializeField] GameObject[] parryStacksIcons;
    [SerializeField] GameObject[] parryStacksCases;
    public void AddParryStackIcon()
    {
        Debug.Log("ParryStackUI AddParryStackIcon() currentParryStack: " + PlayerScript.Instance.GetPlayerRuntimeStats().currentParryStack);
        parryStacksIcons[PlayerScript.Instance.GetPlayerRuntimeStats().currentParryStack - 1].SetActive(true);
    }
    public void RemoveParryStackIcon(int cost)
    {

        int index = PlayerScript.Instance.GetPlayerRuntimeStats().currentParryStack;
        for (int i = 0; i < cost; i++)
        {
            parryStacksIcons[index--].SetActive(false);
        }
    }
    public void RemoveAllParryStackIcon()
    {
        for (int i = 0; i < parryStacksIcons.Length; i++)
        {
            parryStacksIcons[i].SetActive(false);
        }

    }
    public void SetMaxParryStack()
    {
        for (int i = 0; i < parryStacksCases.Length; i++)
        {
            parryStacksCases[i].SetActive(false);
            parryStacksIcons[i].SetActive(false);
        }

        for (int i = 0; i < PlayerScript.Instance.GetPlayerRuntimeStats().maxParryStack; i++)
        {
            parryStacksCases[i].SetActive(true);
        }
    }
}
