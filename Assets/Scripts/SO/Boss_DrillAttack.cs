using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Boss/AttackPattern/Wood/Boss_DrillAttack")]
public class Boss_DrillAttack : EnemyAttackPattern
{

    public float attackSpeed; // 

    public override IEnumerator Execute(EnemyBase enemy)
    {
        enemy.GetRigidbody().linearVelocity = Vector2.zero;


        enemy.GetAnimatorController().PlayAttack();

        yield return enemy.StartCoroutine(enemy.OutLineRoutine(attackChargeSec));

        Vector2 PlayerPos = GameManager.Instance.playerScript.GetPlayerTransform().position;
        Vector2 dir = (GameManager.Instance.playerScript.transform.position - enemy.transform.position).normalized;


        Vector2 startPos = enemy.GetRigidbody().position;
        Vector2 endPos = PlayerPos + dir * 2f;
        bool hasDealtDamage = false;
        enemy.gameObject.layer = LayerMask.NameToLayer("EnemyAttack");
        enemy.AddAttackEffect(EffectPooler.Instance.SpawnFromPool("WoodDrill", startPos + dir, Quaternion.FromToRotation(Vector3.right, dir)));
        // 속도 기반 이동 (거리 / 속도 = 필요한 시간)
        float totalDistance = Vector2.Distance(startPos, endPos);
        float travelTime = totalDistance / attackSpeed;
        float elapsedTime = 0f;
        bool isWall = false;
        while (elapsedTime < travelTime)
        {
            float t = elapsedTime / travelTime;
            Vector2 currentPos = Vector2.Lerp(startPos, endPos, t);
            enemy.GetRigidbody().MovePosition(currentPos);

            if (!hasDealtDamage)
            {
                Collider2D hit = Physics2D.OverlapCircle(currentPos, 0.3f, LayerMask.GetMask("Player", "PlayerDash"));
                if (hit != null && hit.CompareTag("Player"))
                {
                    GameManager.Instance.playerScript.TakeAttack(enemy);
                    hasDealtDamage = true;
                }
            }
            RaycastHit2D wallHit = Physics2D.Raycast(enemy.transform.position, dir, 1f, LayerMask.GetMask("Wall", "Hole"));
            if (wallHit != false)
            {
                enemy.GetRigidbody().MovePosition(enemy.transform.position);
                isWall = true;
                break;
            }
            enemy.Effects[0].transform.position = enemy.transform.position + (Vector3)dir;
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        enemy.Effects[0].SetActive(false);
        if (!isWall) enemy.GetRigidbody().MovePosition(endPos); // 정확한 끝점 보정
        enemy.gameObject.layer = LayerMask.NameToLayer("Enemy");
        enemy.enemyShaderController.OffOutline();
        yield return new WaitForSeconds(attackPostDelay);
    }
}