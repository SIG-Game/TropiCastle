public class ChasingEnemyState : BaseEnemyState
{
    public ChasingEnemyState(EnemyController enemyController) :
        base(enemyController)
    {
    }

    public override void StateEnter()
    {
        enemyController.Agent.enabled = true;
    }

    public override void StateUpdate()
    {
        if (enemyController.ShouldStopChasingPlayer())
        {
            enemyController.SwitchState(enemyController.IdleState);
        }
        else
        {
            enemyController.Agent.SetDestination(
                enemyController.GetPlayerColliderPosition());
        }
    }
}
