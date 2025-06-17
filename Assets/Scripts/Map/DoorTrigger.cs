using UnityEngine;

public enum Direction { Up, Down, Left, Right }
public class DoorTrigger : MonoBehaviour
{
    public Direction direction;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            MapManager.Instance.MoveToRoom(direction);

            FirebaseUploader uploader = Object.FindFirstObjectByType<FirebaseUploader>();
   
        }
    }
}
