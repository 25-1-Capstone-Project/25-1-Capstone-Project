using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    Stage stage;

    void Start()
    {
        stage = FindObjectOfType<Stage>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            stage.DoorSet();
            AudioManager.Instance.PlaySFX("CloseMetalDoor");
            Destroy(gameObject);

        }
    }

}
