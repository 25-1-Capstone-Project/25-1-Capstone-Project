using UnityEngine;
using UnityEngine.SceneManagement;
public class BootStrap : MonoBehaviour
{
    [SerializeField] string firstSceneName = "MainMenu";

    void Awake()
    {

        SceneManager.LoadScene(firstSceneName);
    }

}
