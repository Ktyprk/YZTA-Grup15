using UnityEngine;
using System.Collections;

public class JumpAttackBehavior : IEnemyAttackBehavior
{
    private float jumpHeight;
    private float jumpDuration;
    private float momentumDistance;
    private float momentumDuration;

    public JumpAttackBehavior(float jumpHeight, float jumpDuration, float momentumDistance, float momentumDuration)
    {
        this.jumpHeight = jumpHeight;
        this.jumpDuration = jumpDuration;
        this.momentumDistance = momentumDistance;
        this.momentumDuration = momentumDuration;
    }

    public void Attack(EnemyController enemy, Transform target)
    {
        Vector3 startPos = enemy.transform.position;
        Vector3 targetPos = target.position;
     
        
        Vector3 flatDirection = (targetPos - startPos);
        flatDirection.y = 0;

        Vector3 jumpTarget = startPos + flatDirection; 
        Vector3 momentumDir = flatDirection.normalized;

       
        enemy.StartCoroutine(PerformJump(enemy.transform, jumpTarget, momentumDir));
    }

    private IEnumerator PerformJump(Transform enemyTransform, Vector3 targetPosition, Vector3 momentumDirection)
    {
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
}
