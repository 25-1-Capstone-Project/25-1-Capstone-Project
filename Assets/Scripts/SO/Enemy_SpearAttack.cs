using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Enemy/AttackPattern/Enemy/Enemy_SpearAttack")]
public class Enemy_SpearAttack : EnemyAttackPattern
{
    public GameObject pathLinePrefab;

    public float attackSpeed; // 

    public override IEnumerator Execute(EnemyBase enemy)
    {
        enemy.GetRigidbody().linearVelocity = Vector2.zero;

        Vector2 playerPos = GameManager.Instance.playerScript.GetPlayerTransform().position;
        Vector2 dir = (playerPos - (Vector2)enemy.transform.position).normalized;
        Vector2 startPos = enemy.GetRigidbody().position;
        Vector2 endPos = playerPos + dir * 2f;

        GameObject path = EffectPooler.Instance.SpawnFromPool("EnemyAttackLineEffect", startPos, Quaternion.FromToRotation(Vector3.right, dir));
        LineRenderer line = path.GetComponent<LineRenderer>();

        RaycastHit2D previewHit =
            Physics2D.Raycast(startPos, dir, 10f, LayerMask.GetMask("Wall", "Hole"));
        Vector2 previewEnd = previewHit ? previewHit.point : endPos;

        line.positionCount = 2;
        line.SetPosition(0, startPos);
        line.SetPosition(1, previewEnd);

        enemy.AddAttackEffect(path);

        enemy.GetAnimatorController().PlayAttack();
        yield return enemy.StartCoroutine(enemy.OutLineRoutine(attackChargeSec));

        yield return enemy.StartCoroutine(BlinkLine(line, 1));

        // 타이밍용으로 넣긴 했는데 좀 루즈해질 위험이...
        yield return new WaitForSeconds(0.07f);

        enemy.ClearAttackEffect();

        enemy.GetAnimatorController().FreezeFrame(true);
        enemy.SpriteFlip();
        yield return new WaitForEndOfFrame();

        bool hasDealtDamage = false;
        enemy.gameObject.layer = LayerMask.NameToLayer("EnemyAttack");

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
                Collider2D hit = Physics2D.OverlapCircle(
                    currentPos, 0.3f, LayerMask.GetMask("Player", "PlayerDash"));

                if (hit != null && hit.CompareTag("Player"))
                {
                    GameManager.Instance.playerScript.TakeAttack(enemy);
                    hasDealtDamage = true;
                }
            }

            RaycastHit2D wallHit =
                Physics2D.Raycast(enemy.transform.position, dir, 1f,
                    LayerMask.GetMask("Wall", "Hole"));

            if (wallHit)
            {
                isWall = true;
                break;
            }

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        enemy.GetAnimatorController().FreezeFrame(false);
        if (!isWall)
            enemy.GetRigidbody().MovePosition(endPos);

        enemy.gameObject.layer = LayerMask.NameToLayer("Enemy");
        enemy.enemyShaderController.OffOutline();
        yield return new WaitForSeconds(attackPostDelay);
    }


    private IEnumerator BlinkLine(LineRenderer line, int blinkCount, float interval = 0.04f)
    {
        for (int i = 0; i < blinkCount; i++)
        {
            line.enabled = false;
            yield return new WaitForSeconds(interval);

            line.enabled = true;
            yield return new WaitForSeconds(interval);

        }
    }
}