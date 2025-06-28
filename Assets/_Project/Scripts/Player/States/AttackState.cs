using UnityEngine;
using System.Collections.Generic;

public class AttackState : PlayerState
{
    private float attackDuration = .8f;
    private float timer = 0f;

    private bool attackHitDone = false;
    
    private Vector3 hitboxCenter = new Vector3(0, .025f, 1.8f);
    private Vector3 hitboxSize = new Vector3(3f, 1f, 2f);
    private LayerMask enemyLayer;
   

    public AttackState(PlayerController controller) : base(controller)
    {
        animatorController = controller.attackOverride;
        enemyLayer = controller.enemyLayer; 
        
    }

    public override void Enter()
    {
        controller.SetAnimation("Attack");
        timer = 0f;
        attackHitDone = false;
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (!attackHitDone && timer >= 0.4f)
        {
            PerformAttackHit();
        }
        
        if (timer >= attackDuration)
        {
            controller.ChangeState(new IdleState(controller));
        }
    }
    
    private void PerformAttackHit()
    {
        attackHitDone = true;

        Vector3 boxCenter = controller.transform.position + controller.transform.TransformDirection(hitboxCenter);
    
        controller.attackGizmoCenter = hitboxCenter;
        controller.attackGizmoSize = hitboxSize;
        controller.showAttackGizmo = true;

        Collider[] hits = Physics.OverlapBox(boxCenter, hitboxSize / 2f, controller.transform.rotation, enemyLayer);

        HashSet<GameObject> damagedEnemies = new();
        
        foreach (Collider hit in hits)
        {
            GameObject enemy = hit.gameObject;
            
            if (!damagedEnemies.Contains(enemy))
            {
                if (enemy.TryGetComponent<ICombat>(out var combatTarget))
                {
                    combatTarget.TakeDamage(10); 
                    damagedEnemies.Add(enemy);
                }
            }
        }
    }

    
    public override void Exit()
    {
        controller.showAttackGizmo = false;
    }

}

