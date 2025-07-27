using UnityEngine;

public interface IBossEnemyAttackBehavior
{
    void Attack(BossEnemyController enemy, Transform target);
}
