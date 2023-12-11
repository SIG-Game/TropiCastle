using UnityEngine;

public class InitialEnemyState : BaseEnemyState
{
    private float stateEnterTime;

    public InitialEnemyState(EnemyController enemyController) :
        base(enemyController)
    {
    }

    public override void StateEnter()
    {
        stateEnterTime = Time.time;
    }

    public override void StateUpdate()
    {
        if (Time.time - stateEnterTime >=
            enemyController.InitialWaitTimeBeforeIdleSeconds)
        {
            enemyController.SwitchState(enemyController.IdleState);
        }
    }
}
