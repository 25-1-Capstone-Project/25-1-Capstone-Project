using UnityEngine;
using UnityEngine.UI;

public class SkillSelect : MonoBehaviour
{
    public GameObject skillSelectWindow;
    private System.Action<int> onSkillSelected;

    public void ShowSkillWindow(System.Action<int> callback)
    {
        skillSelectWindow.SetActive(true);
        onSkillSelected = callback;
    }


    public void OnClickSkillButton(int index)
    {
        Debug.Log($"��ų ��ư �Է�: {index}");
        skillSelectWindow.SetActive(false);
        GameManager.Instance.SetTimeScale(1f);

        onSkillSelected?.Invoke(index);
        onSkillSelected = null;
    }
}
