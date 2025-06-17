using UnityEngine;

public class Ghost : MonoBehaviour
{
    private ParticleSystem particle;
    private Material material;
    private void Start()
    {
        particle = GetComponent<ParticleSystem>();
        material = particle.GetComponent<ParticleSystemRenderer>().material;
    }
    public void SetActive(bool active)
    {
        if (active)
        {
            particle.Play();
        }
        else
        {
            particle.Stop();
        }
    }
    public void SetSprite(SpriteRenderer spriteRenderer)
    {
        Sprite sprite = spriteRenderer.sprite;
        Rect rect = sprite.textureRect;
        Texture tex = sprite.texture;

        Vector4 uvRect = new Vector4(
            rect.x / tex.width,
            rect.y / tex.height,
            rect.width / tex.width,
            rect.height / tex.height
        );

        material.SetTexture("_MainTex", tex);
        material.SetVector("_UVRect", uvRect);

    }

}