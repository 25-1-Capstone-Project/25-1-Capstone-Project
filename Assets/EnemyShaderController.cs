using UnityEngine;

public class EnemyShaderController : MonoBehaviour
{
    Material enemyMaterial;
    [SerializeField] private float outlineThickness = 2f;
    public void Start()
    {
        enemyMaterial = GetComponent<SpriteRenderer>().material;
    }
    private void OnDisable() {
        enemyMaterial.SetFloat("_OutlineColor", 0);
    } 
    
    public void OnOutline()
    {
        enemyMaterial.SetFloat("_OutlineColor", outlineThickness);

    }
    public void OffOutline()
    {
        enemyMaterial.SetFloat("_OutlineColor", 0);
    }
}
