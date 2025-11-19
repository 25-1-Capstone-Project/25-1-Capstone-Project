using UnityEngine;

public class UI_DeadInfo : MonoBehaviour
{
    [SerializeField] GameObject deadInfoPanel;
    [SerializeField] GameObject deadPointText;
    [SerializeField] GameObject GoToHubButton;

    public void OnClickGoToHub()
    {
        SetActiveDeadInfoPanel(false);
        GameManager.Instance.SetTimeScale(1f);
        GameManager.Instance.ChangeStateByEnum(EGameState.MainMenu);
    }
    public void SetActiveDeadInfoPanel(bool active)
    {
        GameManager.Instance.SetTimeScale(0f);
        deadInfoPanel.SetActive(active);
    }
}
