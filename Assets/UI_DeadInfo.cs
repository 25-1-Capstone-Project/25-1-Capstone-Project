using UnityEngine;

public class UI_DeadInfo : MonoBehaviour
{
    [SerializeField] GameObject deadInfoPanel;
    [SerializeField] GameObject deadPointText;
    [SerializeField] GameObject GoToHubButton;

    public void OnClickGoToHub()
    {
        PlayerScript.Instance.InitPlayer();
        SetActiveDeadInfoPanel(false);
        GameManager.Instance.SetTimeScale(1f);
        GameManager.Instance.ChangeStateByEnum(EGameState.Hub);

    }
    public void SetActiveDeadInfoPanel(bool active)
    {
        deadInfoPanel.SetActive(active);
    }
}
