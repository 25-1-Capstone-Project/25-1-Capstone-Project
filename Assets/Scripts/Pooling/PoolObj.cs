using UnityEngine;

//오브젝트 풀링을 위한 스크립트
public class PoolObj : MonoBehaviour
{
    void OnDisable()
    {
        EffectPooler.Instance.ReturnToPool(gameObject);
    }
}
