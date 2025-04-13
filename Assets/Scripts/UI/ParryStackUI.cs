using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ParryStackUI : MonoBehaviour
{
    [SerializeField] GameObject[] parryStacksIcons;
    [SerializeField] GameObject[] parryStacksCases;
    public void AddParryStackIcon()
    {
        Debug.Log("ParryStackUI AddParryStackIcon() currentParryStack: " + Player.instance.GetPlayerRuntimeStats().currentParryStack);
        parryStacksIcons[Player.instance.GetPlayerRuntimeStats().currentParryStack - 1].SetActive(true);
    }
    public void RemoveParryStackIcon(int cost)
    {

        int index = Player.instance.GetPlayerRuntimeStats().currentParryStack;
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

        for (int i = 0; i < Player.instance.GetPlayerRuntimeStats().maxParryStack; i++)
        {
            parryStacksCases[i].SetActive(true);
        }
    }
}
