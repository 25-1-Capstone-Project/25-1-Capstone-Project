using UnityEngine;
using System.Collections;
using System;

public class WoodProjectile : ProjectileEnemyAttack
{
    Transform target;

    bool on = false;
    private void OnEnable()
    {
        base.Start();
        on = false;
        target = GameManager.Instance.playerScript.GetPlayerTransform();
        StartCoroutine(Routine());
    }
    public IEnumerator Routine()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(1f,2.5f));
        Shot();
    }
    protected override void FixedUpdate()
    {
        if (!on)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            SetDirectionVec(direction);
        }
        else
        {
            rb.linearVelocity = (Vector2)transform.right * speed;
        }

    }
    public void Shot()
    {
        on = true;
    }

}