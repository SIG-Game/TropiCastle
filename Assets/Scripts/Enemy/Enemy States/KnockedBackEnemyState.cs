using UnityEngine;

public class KnockedBackEnemyState : BaseEnemyState
{
    private float knockbackStartTime;

    public KnockedBackEnemyState(EnemyController enemyController) :
        base(enemyController)
    {
    }

    public override void StateEnter()
    {
        ResetKnockbackStartTime();
    }

    public override void StateUpdate()
    {
        if (Time.time - knockbackStartTime >=
            enemyController.GetWaitTimeAfterKnockbackSeconds())
        {
            enemyController.SwitchState(enemyController.IdleState);
        }
    }

    public void ResetKnockbackStartTime()
    {
        knockbackStartTime = Time.time;
    }
}
