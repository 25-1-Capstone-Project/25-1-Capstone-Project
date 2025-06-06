using UnityEngine;

public class DisableEffect : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Wall"))
        {
            gameObject.SetActive(false);
        }
    }
}
