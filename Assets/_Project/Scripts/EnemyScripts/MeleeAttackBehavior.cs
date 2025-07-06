using UnityEngine;

public class MeleeAttackBehavior : IEnemyAttackBehavior
{
    public void Attack(EnemyController enemy, Transform target)
    {
        if (target.TryGetComponent(out ICombat combatTarget))
        {
            combatTarget.TakeDamage(enemy.EnemyData.damage);
        }
    }
}
