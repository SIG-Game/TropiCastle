public abstract class BaseEnemyState
{
    protected EnemyController enemyController;

    public BaseEnemyState(EnemyController enemyController)
    {
        this.enemyController = enemyController;
    }

    public abstract void StateEnter();

    public abstract void StateUpdate();
}
