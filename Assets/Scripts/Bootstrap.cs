using UnityEngine;
using UnityEngine.SceneManagement;


public class BootStrap : MonoBehaviour
{
    [SerializeField] string firstSceneName = "MainMenu";


    private void Start()
    {
       // QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60; // FPS 설정
        SceneManager.LoadScene(firstSceneName);

    }
}
