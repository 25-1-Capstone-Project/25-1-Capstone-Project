using UnityEngine;

public class NextFloorTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.GoToNextDungeonFloor();
        }
    }
}
