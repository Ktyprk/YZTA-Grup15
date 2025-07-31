using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackBehavior : IEnemyAttackBehavior
{

    private Vector3 hitboxCenter = new Vector3(0, 1f, 1f);
    private Vector3 hitboxSize = new Vector3(1f, 1f, 1f);
    private LayerMask playerLayer;
    EnemyData enemyData;
    private string meleeAttack = "meleeAttack";
    public MeleeAttackBehavior(EnemyData enemyData)
    {
        this.enemyData = enemyData;
    }
    public void Attack(EnemyController controller, Transform target)
    {
        /* if (target.TryGetComponent(out ICombat combatTarget))
         {
             combatTarget.TakeDamage(enemy.EnemyData.damage);
         }*/
        playerLayer = 1 << LayerMask.NameToLayer("Player");

        Vector3 boxCenter = controller.transform.position + controller.transform.TransformDirection(hitboxCenter);

        controller.attackGizmoCenter = hitboxCenter;
        controller.attackGizmoSize = hitboxSize;
        controller.showAttackGizmo = true;

        Collider[] hits = Physics.OverlapBox(boxCenter, hitboxSize / 2f, controller.transform.rotation, playerLayer);

        HashSet<GameObject> damagedEnemies = new();

        foreach (Collider hit in hits)
        {
            GameObject enemy = hit.gameObject;

            if (!damagedEnemies.Contains(enemy))
            {
                if (enemy.TryGetComponent<ICombat>(out var combatTarget))
                {
                    int minDamage = (int)controller.EnemyData.damage;
                    int maxDamage = (int)controller.EnemyData.damage;
                    hit.TryGetComponent<PlayerController>(out var playerTarget);
                    

                    int playerDamage = Random.Range(minDamage, maxDamage + 1);
                    if (playerTarget != null)
                        playerTarget.AddAttack(enemyData.name, meleeAttack, playerDamage);
                    combatTarget.TakeDamage(playerDamage);
                    damagedEnemies.Add(enemy);
                }
            }
        }
       
    }

}
