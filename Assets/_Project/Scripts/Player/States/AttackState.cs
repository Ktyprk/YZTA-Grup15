using UnityEngine;
using System.Collections.Generic;

public class AttackState : PlayerState
{
    private float attackDuration = 0.8f;
    private float timer = 0f;
    private bool attackHitDone = false;

    private Vector3 hitboxCenter = new Vector3(0, 1f, 1.2f);
    private Vector3 hitboxSize = new Vector3(2f, 1f, 1f);

    private LayerMask enemyLayer;
    private LayerMask interactableLayer;

    public AttackState(PlayerController controller) : base(controller)
    {
        animatorController = controller.attackOverride;
        enemyLayer = controller.enemyLayer;
        interactableLayer = LayerMask.GetMask("interactableObject");
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

        // Enemy ve Interactable layer'ları birleştiriliyor
        LayerMask combinedMask = enemyLayer | interactableLayer;

        Collider[] hits = Physics.OverlapBox(
            boxCenter,
            hitboxSize / 2f,
            controller.transform.rotation,
            combinedMask
        );

        HashSet<GameObject> damagedObjects = new();

        foreach (Collider hit in hits)
        {
            GameObject obj = hit.gameObject;

            if (damagedObjects.Contains(obj)) continue;

            // Enemy'ye hasar ver
            if (obj.TryGetComponent<ICombat>(out var combatTarget))
            {
                int minDamage = (int)controller.playerStats.Attack;
                int maxDamage = (int)controller.playerStats.Attack;

                int playerDamage = Random.Range(minDamage, maxDamage + 1);
                combatTarget.TakeDamage(playerDamage);
                damagedObjects.Add(obj);
            }
            // Patlayan varil tetikle
            else if (obj.TryGetComponent<explosiveBarrel>(out var explosiveBarrel))
            {
                controller.StartCoroutine(explosiveBarrel.blowupEffect());
                damagedObjects.Add(obj);
            }
        }
    }

    public override void Exit()
    {
        controller.showAttackGizmo = false;
    }
}
