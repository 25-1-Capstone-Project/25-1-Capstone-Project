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
        deadInfoPanel.SetActive(active);
    }
}
