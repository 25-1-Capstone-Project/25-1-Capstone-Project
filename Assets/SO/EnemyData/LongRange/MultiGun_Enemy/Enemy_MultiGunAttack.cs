using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Enemy/AttackPattern/Enemy/Enemy_MultiGunAttack")]
public class Enemy_MultiGunAttack : EnemyAttackPattern
{
    public override IEnumerator Execute(Enemy enemy)
    {

        yield return new WaitForSeconds(attackPostDelay);

    }
}
