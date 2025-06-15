using UnityEngine;

public class SkillSelectNPC : MonoBehaviour
{
    public GameObject dialogText;
    private bool isPlayerInRange = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            dialogText.SetActive(true);
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            dialogText.SetActive(false);
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.K))
        {
            PlayerScript.Instance.OpenSkillWindow();
            dialogText.SetActive(false);
        }
    }
}
