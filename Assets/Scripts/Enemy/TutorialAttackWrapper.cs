using UnityEngine;
using System.Collections;



[CreateAssetMenu(menuName = "Enemy/AttackPattern/_TutorialWrapper")]
public class TutorialAttackWrapper : EnemyAttackPattern
{
    public EnemyAttackPattern inner;           // 감쌀 실제 패턴(예: SwordSlash)

    private void OnValidate()
    {
        attackRange = inner.attackRange;
    }
    public override IEnumerator Execute(EnemyBase enemy)
    {
        enemy.IsAttacking = true;
        enemy.enemyShaderController.OnOutline();
        enemy.GetRigidbody().linearVelocity = Vector2.zero;
        PlayerScript.Instance.OnParryInput += ParryInputEvent;
        float time = inner.attackChargeSec - 0.1f;
        PlayerScript.Instance.OnlyParryDuringTime(time);
        
        GameManager.Instance.SetTimeScale(0, time);

        UIManager.Instance.SetActiveGuideUI(true, "[우클릭]!");
        yield return enemy.StartCoroutine(inner.Execute(enemy));


        yield return null;
        enemy.enemyShaderController.OffOutline();
        enemy.IsAttacking = false;
    }

    void ParryInputEvent()
    {

        UIManager.Instance.SetActiveGuideUI(true, "[좌클릭]을 눌러 마무리 일격!");
        GameManager.Instance.SetTimeScale(1f);
        GameManager.Instance.SetTimeScale(0, 0.5f);
        PlayerScript.Instance.OnAttackInput += AttackInputEvent;
        PlayerScript.Instance.OnParryInput -= ParryInputEvent;

    }
    void AttackInputEvent()
    {
        // 연출 강화: 약간의 슬로우
        GameManager.Instance.SetTimeScale(1f);
        UIManager.Instance.SetActiveGuideUI(false);
        PlayerScript.Instance.OnAttackInput -= AttackInputEvent;
    }

}
