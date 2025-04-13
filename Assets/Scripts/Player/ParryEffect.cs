using UnityEngine;
using System.Collections;
public class ParryEffect : MonoBehaviour
{
    public Sprite[] parryFrames;
    public float frameRate = 0.05f;

    private void OnEnable() {
        
        if (parryFrames.Length > 0)
        {
            StartCoroutine(PlayParryEffect());
        }
    }

    IEnumerator PlayParryEffect()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        for (int i = 0; i < parryFrames.Length; i++)
        {
            sr.sprite = parryFrames[i];
            yield return new WaitForSeconds(frameRate);
        }

        Destroy(gameObject);
    }

}
