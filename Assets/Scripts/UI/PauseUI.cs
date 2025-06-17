using UnityEngine;

public class PauseUI : MonoBehaviour
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
        PlayerScript.Instance.SetActivePlayerInput(isPaused);
        pauseMenu.SetActive(!isPaused);
        float time = isPaused ? 1f : 0f;
        GameManager.Instance.SetTimeScale(time);
    }

    public void OpenSettings()
    {
        settingsMenu.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
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
        GameManager.Instance.SetTimeScale(1f);
        Destroy(PlayerScript.Instance.gameObject);
        GameManager.Instance.ChangeStateByEnum(EGameState.MainMenu);

    }
}
