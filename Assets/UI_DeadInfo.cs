using UnityEngine;

public class UI_DeadInfo : MonoBehaviour
{
    [SerializeField] GameObject deadInfoPanel;
    [SerializeField] GameObject deadPointText;
    [SerializeField] GameObject GoToHubButton;

    public void OnClickGoToHub()
    {
        GameManager.Instance.ChangeStateByEnum(EGameState.Hub);
        GameManager.Instance.SetTimeScale(1f);
    }
    public void SetActiveDeadInfoPanel(bool active)
    {
        deadInfoPanel.SetActive(active);
    }
}
