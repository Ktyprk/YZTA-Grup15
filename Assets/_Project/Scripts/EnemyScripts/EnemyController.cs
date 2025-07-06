using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, ICombat
{
    public EnemyData EnemyData => enemyData; 
    [Header("Enemy Settings")]
    [SerializeField] private EnemyData enemyData;
    private IEnemyAttackBehavior attackBehavior;

    [Header("Damage Flash Settings")]
    [SerializeField] private List<SkinnedMeshRenderer> renderers;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("Target Settings")]
    [SerializeField] private Transform target;

    public event Action OnWaitForAttack;
    public event Action OnAttack;
    public event Action OnIdle;
    public event Action OnDie;
    public event Action<int> OnDamageTaken;

    private EnemyAnimatorController animController;
    private Coroutine flashRoutine;

    private float attackTimer;
    private bool waitingForAttack;
    public int currentHealth;

    private void Awake()
    {
        animController = GetComponent<EnemyAnimatorController>();
    }

    private void OnEnable()
    {
        ResetEnemy();
        InitializeAttackBehavior();
    }

    private void FixedUpdate()
    {
        if (target == null || !target.gameObject.activeSelf)
        {
            target = null;
            WanderSimple();
            //animController.Idle();
            if (waitingForAttack)
            {
                waitingForAttack = false;
                OnIdle?.Invoke();
            }
            return;
        }

        RotateTowardsTarget();

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > enemyData.attackDistance)
        {
            MoveTowardsTarget();
        }

        HandleCombat(distance);
    }

    
    private Vector3 wanderTarget;
    private float wanderTimer;
    [SerializeField] private float wanderRadius = 5f; 


    
    private void WanderSimple()
    {
        
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0f || Vector3.Distance(transform.position, wanderTarget) < 0.5f)
        {
            Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * wanderRadius;
            wanderTarget = new Vector3(transform.position.x + randomPoint.x, transform.position.y, transform.position.z + randomPoint.y);
            wanderTimer = UnityEngine.Random.Range(3f, 6f); 
        }

        animController.Walk();
        
        Vector3 direction = (wanderTarget - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
        
        transform.position += direction * enemyData.moveSpeed * 0.5f * Time.deltaTime;
    }




    
    public void InitializeAttackBehavior()
    {
        switch (enemyData.attackType)
        {
            case AttackType.Melee:
                InitializeAttackBehavior(new MeleeAttackBehavior());
                break;

            case AttackType.Ranged:
                InitializeAttackBehavior(new RangedAttackBehavior(enemyData.projectilePrefab, enemyData.projectileSpeed)); 
                break;
        }
    }
    private void InitializeAttackBehavior(IEnemyAttackBehavior behavior)
    {
        attackBehavior = behavior;
    }


    private void HandleCombat(float distanceToTarget)
    {
        if (distanceToTarget > enemyData.attackDistance)
        {
            waitingForAttack = false;
            attackTimer = 0f;
            return;
        }

        attackTimer += Time.deltaTime;

        if (attackTimer >= enemyData.attackCooldown || !waitingForAttack)
        {
            waitingForAttack = true;
            attackTimer = 0f;

            animController.Attack();
            OnAttack?.Invoke();
            attackBehavior?.Attack(this, target);
        }
    }


    private void RotateTowardsTarget()
    {
        if (target == null) return; 
        
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void MoveTowardsTarget()
    {
        if (target == null) return; 
        
        animController.Walk();
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * enemyData.moveSpeed * Time.deltaTime;
    }
    
    public void AnimationEvent_DealDamage()
    {
        if (target != null && target.TryGetComponent(out ICombat combatTarget))
        {
            combatTarget.TakeDamage(enemyData.damage);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        OnDamageTaken?.Invoke(damage);
        
        DamagePopUpGenerator.instance.CreatePopUp(transform.position + Vector3.up * 2, damage.ToString());

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashEffect());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashEffect()
    {
        SetMaterials(flashMaterial);
        yield return new WaitForSeconds(flashDuration);
        ResetMaterials();
    }

    private void SetMaterials(Material mat)
    {
        foreach (var renderer in renderers)
        {
            if (renderer != null)
                renderer.material = mat;
        }
    }

    private void ResetMaterials()
    {
        SetMaterials(normalMaterial);
    }

    private void Die()
    {
        OnDie?.Invoke();
        Debug.Log($"{enemyData.enemyName} died.");
        
        EntityPoolManager.Instance.ReleaseEntityToPool(enemyData.enemyPrefab, gameObject);
    }

    public Transform GetTransform() => transform;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    public void ResetEnemy()
    {
        currentHealth = enemyData.health;
        attackTimer = UnityEngine.Random.Range(0, enemyData.attackTime * 0.6f);
        waitingForAttack = false;
        target = null;

        animController.ResetAnimator();
        ResetMaterials();
        animController.Idle(); 
    }
}