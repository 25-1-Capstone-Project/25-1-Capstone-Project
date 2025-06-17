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
        material.mainTexture = spriteRenderer.sprite.texture;
    }

}