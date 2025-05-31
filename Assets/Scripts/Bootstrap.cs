using UnityEngine;
using UnityEngine.SceneManagement;
public class BootStrap : MonoBehaviour
{
    [SerializeField] string firstSceneName = "MainMenu";

    void Awake()
    {
        if (SaveManager.Instance == null)
        {
            gameObject.AddComponent<SaveManager>();
        }

        SceneManager.LoadScene(firstSceneName);
    }

}
