using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour,ICombat
{
    [Header("Enemy Settings")]
    [SerializeField] private EnemyData enemyData;
    
    [Header("Damage Flash")]
    [SerializeField] private List<SkinnedMeshRenderer> renderers;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float flashDuration = 0.1f;

    private Coroutine flashRoutine;

    

    private Transform target;

    private float stanceTimer;
    private float attackTimer;
    private bool waitForAttack;

    public System.Action OnWaitForAttack, OnAttack, OnIdle;

    public EnemyAnimatorController animController;
    public int currentHealth;

    private void Awake()
    {
        if (enemyData == null)
        {
            enabled = false;
            return;
        }

        currentHealth = enemyData.health;

        animController = GetComponent<EnemyAnimatorController>();
        attackTimer = Random.Range(0, enemyData.attackTime * 0.6f);
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            animController.Idle();
            if (waitForAttack)
            {
                waitForAttack = false;
                OnIdle?.Invoke();
            }
            return;
        }

        RotateToTarget();

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist > enemyData.attackDistance)
        {
            
            MoveToTarget();
        }

        HandleCombat(dist);
    }

    private void HandleCombat(float distanceToTarget)
    {
        if (distanceToTarget > enemyData.attackDistance)
        {
            waitForAttack = false;
            return;
        }

        attackTimer += Time.deltaTime;

        if (!waitForAttack)
        {
            waitForAttack = true;
            OnWaitForAttack?.Invoke(); 
        }

        if (attackTimer >= enemyData.attackCooldown)
        {
            attackTimer = 0f;
            Attack();
            OnAttack?.Invoke();
            animController.Attack(); 
        }
    }




    private void RotateToTarget()
    {
        Vector3 dir = (target.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }
    }

    private void MoveToTarget()
    {
       
        animController.Walk();
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * enemyData.moveSpeed * Time.deltaTime;
    }

    private void Attack()
    {

        if (target == null)
        {
            return;
        }

        ICombat combatTarget = target.GetComponentInParent<ICombat>();

        if (combatTarget != null)
        {
            combatTarget.TakeDamage(enemyData.damage);
        }

        if (target.gameObject.activeInHierarchy  == false)
        {
            target = null;
        }
       
    }



    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashEffect());

        if (currentHealth <= 0)
            Die();
    }


    public Transform GetTransform()
    {
        return transform;
    }

    private void Die()
    {
        Debug.Log($"{enemyData.enemyName} died.");
        Destroy(gameObject);
    }
    
    private IEnumerator FlashEffect()
    {
        foreach (var r in renderers)
        {
            if (r != null)
                r.material = flashMaterial;
        }

        yield return new WaitForSeconds(flashDuration);

        foreach (var r in renderers)
        {
            if (r != null)
                r.material = normalMaterial;
        }
    }

}


