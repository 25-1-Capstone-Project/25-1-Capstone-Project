using UnityEngine;
using System.Collections;
[CreateAssetMenu(menuName = "Boss/AttackPattern/Skeleton/Boss_MeteoAttack")]
class Boss_MeteoAttack : EnemyAttackPattern
{
    [SerializeField] float attackCount = 5;
    public override IEnumerator Execute(EnemyBase boss)
    {  
        boss.GetAnimatorController().PlayAttack();
        yield return new WaitForSeconds(attackChargeSec);


        for (int i = 0; i < attackCount; i++)
        {
            //플레이어의 위치에 보석을 떨굼 맞으면 데미지
            Vector2 playerPos = PlayerScript.Instance.transform.position;
            GameObject meteoProj = EffectPooler.Instance.SpawnFromPool("Skeleton_MeteoProjectile",playerPos);
            FallingAttack enemyAttack = meteoProj.GetComponent<FallingAttack>();
            enemyAttack.SetDamage(boss.GetDamage());
           // enemyAttack.PlayFall(0.4f, 0.2f, 0.5f); animation envent로 대체
            yield return new WaitForSeconds(attackDuration); 
        }
        boss.GetAnimatorController().EndAttack();
        yield return new WaitForSeconds(attackPostDelay);
          boss.GetAnimatorController().EndAttack();
    }
}