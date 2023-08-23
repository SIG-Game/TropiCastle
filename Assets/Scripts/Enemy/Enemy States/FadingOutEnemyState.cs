public class FadingOutEnemyState : BaseEnemyState
{
    public FadingOutEnemyState(EnemyController enemyController) :
        base(enemyController)
    {
    }

    public override void StateEnter()
    {
        enemyController.DisableCollider();
    }

    public override void StateUpdate()
    {
        float newAlpha = enemyController.GetNewFadingOutAlpha();

        if (newAlpha <= 0f)
        {
            newAlpha = 0f;

            enemyController.EnemyDeath();
        }

        enemyController.SetAlpha(newAlpha);
    }
}
