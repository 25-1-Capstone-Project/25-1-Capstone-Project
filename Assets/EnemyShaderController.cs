using UnityEngine;

public class EnemyShaderController : MonoBehaviour
{
    private Material enemyMaterial;
    [SerializeField] private float outlineThickness = 2f;

    private void Start()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        enemyMaterial = spriteRenderer.material;

        //텍스처 크기 기반으로 텍셀 사이즈 계산
        Texture mainTex = enemyMaterial.mainTexture;
        if (mainTex != null)
        {
            Vector2 texelSize = new Vector2(1f / mainTex.width, 1f / mainTex.height);
            enemyMaterial.SetVector("_CustomTexelSize", texelSize);
        }
    }

    private void OnDisable()
    {
        enemyMaterial.SetFloat("_OutlineThickness", 0);
    }

    public void OnOutline()
    {
        enemyMaterial.SetFloat("_OutlineThickness", outlineThickness);
    }

    public void OffOutline()
    {
        enemyMaterial.SetFloat("_OutlineThickness", 0);
    }
}
