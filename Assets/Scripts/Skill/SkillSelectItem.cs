using System.Collections;
using UnityEngine;

public class SkillSelectItem : MonoBehaviour
{
    public int skillIndex;
    public GameObject dialogText;
    public Sprite skillicon;

    private bool playerInRange = false;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        skillIndex = Random.Range(0, SkillManager.Instance.SkillPatterns.Length);
        SkillPattern pattern = SkillManager.Instance.SkillPatterns[skillIndex];
        skillicon = pattern.skillIcon;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = skillicon;

        Destroy(gameObject, 7f);

        StartCoroutine(BlinkBeforeDestroy(5f, 2f));
    }

    private IEnumerator BlinkBeforeDestroy(float waitTime, float blinkDuration)
    {
        yield return new WaitForSeconds(waitTime);

        float blinkEndTime = Time.time + blinkDuration;
        bool visible = true;

        while (Time.time < blinkEndTime)
        {
            visible = !visible;
            spriteRenderer.enabled = visible;

            yield return new WaitForSeconds(0.2f);
        }

        spriteRenderer.enabled = true;
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
