using UnityEngine;

public interface IEnemyAttackBehavior
{
    void Attack(EnemyController enemy, Transform target);
}
