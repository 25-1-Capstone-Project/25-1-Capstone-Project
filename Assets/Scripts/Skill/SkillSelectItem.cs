using UnityEngine;

public class SkillSelectItem : MonoBehaviour
{
    public int skillIndex;
    public GameObject dialogText;
    public Sprite skillicon;

    private bool playerInRange = false;

    private void Start()
    {
        skillIndex = Random.Range(0, SkillManager.Instance.SkillPatterns.Length);

        SkillPattern pattern = SkillManager.Instance.SkillPatterns[skillIndex];

        skillicon = pattern.skillIcon;
        GetComponent<SpriteRenderer>().sprite = skillicon;
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.K))
        {
            PlayerScript.Instance.SkillSetting(skillIndex);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            dialogText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false; 
            dialogText.SetActive(false);
        }
    }
}
