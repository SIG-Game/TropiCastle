public class ChasingEnemyState : BaseEnemyState
{
    public ChasingEnemyState(EnemyController enemyController) :
        base(enemyController)
    {
    }

    public override void StateEnter()
    {
    }

    public override void StateUpdate()
    {
        // Movement toward the player occurs in the FixedUpdate
        // method in EnemyController.cs

        if (enemyController.ShouldStopChasingPlayer())
        {
            enemyController.SwitchState(enemyController.IdleState);
        }
    }
}
