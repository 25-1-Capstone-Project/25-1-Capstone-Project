using System.Collections;
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

    void Start()
    {
        StartCoroutine(DisableAfterTime(5f));
    }
    private IEnumerator DisableAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
