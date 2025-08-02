using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class TeleportAttackBehavior : IBossEnemyAttackBehavior
{
    private GameObject oldman;
    private Vector3 offset = new Vector3(1.5f, 0, 0);

    public TeleportAttackBehavior(GameObject oldman)
    {
        this.oldman = oldman;
    }

    public void Attack(BossEnemyController enemy, Transform target)
    {
        Debug.Log("TeleportAttackBehavior: teleporting...");
        Vector3 teleportTarget = target.position + offset;

        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
           
            agent.Warp(teleportTarget);
        }
        else
        {
            enemy.transform.position = teleportTarget;
        }

        if (oldman != null)
        {
            oldman.transform.position = teleportTarget;
        }

    }

    private IEnumerator ReEnableNavMesh(NavMeshAgent agent)
    {
        if (agent != null)
        {
            agent.enabled = false;
            yield return new WaitForSeconds(0.1f);
            agent.enabled = true;
        }
    }
}
