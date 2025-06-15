using System.Collections;
using UnityEngine;

public class AbilityChest : MonoBehaviour
{
    private Animator animator;
    private bool opened = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (opened) return;

        if (other.CompareTag("Player"))
        {
            opened = true;
            animator.enabled = true;
            StartCoroutine(AfterChestAnim());
        }
    }

    private IEnumerator AfterChestAnim()
    {
        yield return new WaitForSeconds(1.0f);
        UIManager.Instance.abilityUI.ShowAbilityChoices();
    }
}
