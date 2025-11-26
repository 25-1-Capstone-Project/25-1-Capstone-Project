using System.Collections;
using UnityEngine;

public class UI_DeadInfo : MonoBehaviour
{
    [SerializeField] GameObject deadInfoPanel;
    [SerializeField] GameObject deadPointText;
    [SerializeField] GameObject GoToHubButton;

    [SerializeField] GameObject mask;

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

    public void PlayMaskShrink()
    {
        StartCoroutine(MaskShrinkRoutine());
    }

    IEnumerator MaskShrinkRoutine()
    {
        float duration = 0.8f;
        float t = 0f;

        Vector3 startScale = new Vector3(100f, 100f, 100f);
        Vector3 endScale = new Vector3(0.05f, 0.05f, 1f);

        mask.transform.localScale = startScale;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float normalized = t / duration;

            mask.transform.localScale = Vector3.Lerp(startScale, endScale, normalized);
            yield return null;
        }

        mask.transform.localScale = endScale;

        yield return new WaitForSecondsRealtime(0.3f);

        GoToHubButton.SetActive(true);
        deadPointText.SetActive(true);
    }
}
