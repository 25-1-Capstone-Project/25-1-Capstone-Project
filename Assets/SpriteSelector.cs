using UnityEngine;

public class SpriteSelector : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;
    SpriteRenderer spriteRenderer;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void OnEnable()
    {
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
    }

}
