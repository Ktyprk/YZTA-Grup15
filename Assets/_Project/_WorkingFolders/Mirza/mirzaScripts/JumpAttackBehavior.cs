using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JumpAttackBehavior : IEnemyAttackBehavior
{
    private float jumpHeight;
    private float jumpDuration;
    private float momentumDistance;
    private float momentumDuration;
    private Vector3 hitboxCenter = new Vector3(0, 1f, 0f);
    private Vector3 hitboxSize = new Vector3(1.5f, 1.5f, 1.5f);
    private LayerMask playerLayer;
    EnemyData enemyData;
    private string jumpAttack = "JumpAttack";

    public JumpAttackBehavior(EnemyData enemyData,    float jumpHeight, float jumpDuration, float momentumDistance, float momentumDuration)
    {
        this.jumpHeight = jumpHeight;
        this.jumpDuration = jumpDuration;
        this.momentumDistance = momentumDistance;
        this.momentumDuration = momentumDuration;
        this.enemyData = enemyData;
    }

    public void Attack( EnemyController enemy, Transform target)
    {
        Vector3 startPos = enemy.transform.position;
        Vector3 targetPos = target.position;
     
        
        Vector3 flatDirection = (targetPos - startPos);
        flatDirection.y = 0;

        Vector3 jumpTarget = startPos + flatDirection; 
        Vector3 momentumDir = flatDirection.normalized;

       
        enemy.StartCoroutine(PerformJump(enemy, jumpTarget, momentumDir));
    }

    private IEnumerator PerformJump(EnemyController enemy, Vector3 targetPosition, Vector3 momentumDirection)
    {
        Transform enemyTransform = enemy.transform;
        Vector3 startPos = enemyTransform.position;
        float elapsed = 0f;

        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / jumpDuration;
            t = Mathf.Clamp01(t);

            Vector3 horizontal = Vector3.Lerp(startPos, targetPosition, t);
            float height = Mathf.Sin(t * Mathf.PI) * jumpHeight;

            enemyTransform.position = new Vector3(horizontal.x, startPos.y + height, horizontal.z);

            yield return null;
        }



        giveDamage(enemy);
        Vector3 rayOrigin = enemyTransform.position + momentumDirection * 0.5f + Vector3.up * 1f;



        float rayDistance = 2.5f;
        Debug.DrawRay(rayOrigin, momentumDirection * rayDistance, Color.red, 1f);

        if (!Physics.Raycast(rayOrigin, momentumDirection, out RaycastHit hit, rayDistance))
        {
            Vector3 momentumStart = enemyTransform.position;
            Vector3 momentumEnd = momentumStart + momentumDirection * momentumDistance;
            float momentumElapsed = 0f;
           
            while (momentumElapsed < momentumDuration)
            {
                Debug.Log("kaydik");
                momentumElapsed += Time.deltaTime;
                float t = momentumElapsed / momentumDuration;
                t = Mathf.Clamp01(t);

                enemyTransform.position = Vector3.Lerp(momentumStart, momentumEnd, t);

                yield return null;
            }
        }

      

        
    }
    private void giveDamage(EnemyController controller)
    {
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
                    if (playerTarget != null&&playerTarget.currentHealth>0)
                        playerTarget.AddAttack(enemyData.enemyName, jumpAttack, playerDamage);
                    combatTarget.TakeDamage(playerDamage);
                    damagedEnemies.Add(enemy);
                }
            }
        }
    }
}
