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
        GameManager.Instance.playerScript.OnParryInput += ParryInputEvent;
        GameManager.Instance.playerScript.OnParrySuccess += ParrySuccessEvent;
        float time = inner.attackChargeSec;
        GameManager.Instance.playerScript.SetCanMove(false);
        GameManager.Instance.playerScript.OnlyParryAfterTime(time);
        
        GameManager.Instance.SetTimeScale(0, time);
        yield return enemy.StartCoroutine(inner.Execute(enemy));

        GameManager.Instance.playerScript.OnParrySuccess -= ParrySuccessEvent;
        enemy.enemyShaderController.OffOutline();
        enemy.IsAttacking = false;
    }

    void ParryInputEvent()
    {
        UIManager.Instance.guideUI.SetActiveGuideUI(false);
        GameManager.Instance.SetTimeScale(1f);
        GameManager.Instance.playerScript.OnParryInput -= ParryInputEvent;
    }
    void ParrySuccessEvent()
    {
        GameManager.Instance.SetTimeScale(0f, 0.3f);
        UIManager.Instance.guideUI.SetActiveGuideUI(true, "스턴당한 적을 [좌클릭]을 눌러 마무리 일격!");

        GameManager.Instance.playerScript.OnAttackInput += AttackInputEvent;
        GameManager.Instance.playerScript.OnParryInput -= ParryInputEvent;
        GameManager.Instance.playerScript.OnParrySuccess -= ParrySuccessEvent;
    }
    void AttackInputEvent()
    {

        GameManager.Instance.SetTimeScale(1f);
        UIManager.Instance.guideUI.SetActiveGuideUI(false);
        GameManager.Instance.playerScript.SetCanMove(true);
        GameManager.Instance.playerScript.SetCanUseParry(true);
        GameManager.Instance.playerScript.OnAttackInput -= AttackInputEvent;
    }

}

