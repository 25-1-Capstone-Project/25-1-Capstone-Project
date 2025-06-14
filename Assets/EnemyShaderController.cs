using UnityEngine;

public class EnemyShaderController : MonoBehaviour
{
    Material enemyMaterial;

    public void Start()
    {
        enemyMaterial = GetComponent<SpriteRenderer>().material;
    }
    private void OnDisable() {
        enemyMaterial.SetInt("_On_Off", 0);
    } 
    
    public void OnOutline()
    {
        enemyMaterial.SetInt("_On_Off", 1);

    }
    public void OffOutline()
    {
        enemyMaterial.SetInt("_On_Off", 0);
    }
}
