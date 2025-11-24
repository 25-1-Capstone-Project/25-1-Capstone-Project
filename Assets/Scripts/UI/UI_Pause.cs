using UnityEngine;

public class UI_Pause : MonoBehaviour
{
    [Header("Pause UI")]
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public GameObject controlsMenu;

    private void Start()
    {
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        if (settingsMenu != null)
        {
            settingsMenu.SetActive(false);
        }
        if (controlsMenu != null)
        {
            controlsMenu.SetActive(false);
        }
    }

    public void TogglePause()
    {
        bool isPaused = pauseMenu.activeSelf;
         GameManager.Instance.playerScript?.SetActivePlayerInput(isPaused);
        pauseMenu.SetActive(!isPaused);
        float time = isPaused ? 1f : 0f;
        GameManager.Instance.SetTimeScale(time);
    }

    public void OpenSettings()
    {
        settingsMenu.SetActive(true);
    }

    public void SetActiveSettings()
    {
        settingsMenu.SetActive(!settingsMenu.activeSelf );
    }

    public void OpenControls()
    {
        controlsMenu.SetActive(true);
    }

    public void CloseControls()
    {
        controlsMenu.SetActive(false);
    }
    public void OnClickGoToMainMenu()
    {
        pauseMenu.SetActive(false);
        GameManager.Instance.SetTimeScale(1);
        GameManager.Instance.ChangeStateByEnum(EGameState.MainMenu);
    }
}
