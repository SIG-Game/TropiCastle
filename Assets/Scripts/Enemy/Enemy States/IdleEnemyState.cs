public class IdleEnemyState : BaseEnemyState
{
    public IdleEnemyState(EnemyController enemyController) :
        base(enemyController)
    {
    }

    public override void StateEnter()
    {
    }

    public override void StateUpdate()
    {
        if (enemyController.ShouldStartChasingPlayer())
        {
            enemyController.SwitchState(enemyController.ChasingState);
        }
    }
}
