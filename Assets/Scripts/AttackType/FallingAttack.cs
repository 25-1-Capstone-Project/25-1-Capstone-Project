
using System;
using System.Collections;
using UnityEngine;

public class FallingAttack : EnemyAttackBase
{
    Collider2D attackCollider;
    void Awake()
    {
        attackCollider = GetComponent<Collider2D>();
     
    }
    void DisableObject()
    {
        attackCollider.enabled = false;
        gameObject.SetActive(false); // 공격 오브젝트 비활성화

    }
    // public void PlayFall(float delay, float damageTime, float pauseTime)
    // {
    //     StartCoroutine(PlayFallRoutine(delay, damageTime, pauseTime));
    // }
    // public IEnumerator PlayFallRoutine(float delay, float damageTime, float pauseTime)
    // {
    
    //     yield return new WaitForSeconds(delay);
    //     attackCollider.enabled = true; // 공격 콜라이더 활성화
    //     yield return new WaitForSeconds(damageTime);
    //     attackCollider.enabled = false;
    //     yield return new WaitForSeconds(pauseTime);
    //     gameObject.SetActive(false); // 공격 오브젝트 비활성화
    // }
}
