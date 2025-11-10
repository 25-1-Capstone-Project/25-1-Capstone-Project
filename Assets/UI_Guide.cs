using TMPro;
using UnityEngine;

public class UI_Guide : MonoBehaviour
{
    [SerializeField] GameObject guideObject;
    [SerializeField] TMP_Text guideText;

    public void SetActiveGuideText(bool active, string msg)
    {
        guideObject.SetActive(active);
        guideText.text = msg;
    }
    
}
