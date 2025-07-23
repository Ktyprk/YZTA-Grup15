using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossEnemyController : MonoBehaviour, ICombat
{
    public EnemyBossData BoosEnemyData => BossenemyData;
    [Header("Enemy Settings")]
    [SerializeField] private EnemyBossData BossenemyData;
    private IBossEnemyAttackBehavior attackBehavior;

    [Header("Damage Flash Settings")]
    [SerializeField] private List<SkinnedMeshRenderer> renderers;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float flashDuration = 0.1f;
    [Header("Gizmos")]
    public Vector3 attackGizmoCenter;
    public Vector3 attackGizmoSize;
    public bool showAttackGizmo = false;

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

    [Header("special skill Settings")]
    public int AttackCount = 0;
    private bool Ragemood = false;
    public float angleOffset = 0f;
    public bool SpecialAttack = false;
    public bool SpecialAttackAnimState = false;
    private void Awake()
    {
        SpecialAttack = false;
        animController = GetComponent<EnemyAnimatorController>();
        BossenemyData.attackType = AttackType.Ranged;
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
        if (distance > BossenemyData.attackDistance && SpecialAttack == false)
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

        transform.position += direction * BossenemyData.moveSpeed * 0.5f * Time.deltaTime;
    }





    public void InitializeAttackBehavior()
    {
        switch (BossenemyData.attackType)
        {
            case AttackType.Melee:
               // InitializeAttackBehavior(new MeleeAttackBehavior());
                break;

            case AttackType.Ranged:
                InitializeAttackBehavior(new BossRangedAttackBehavior(BossenemyData.BasicAttackProjectile, BossenemyData.projectileSpeed));
                break;
            case AttackType.ArcRanged:
                InitializeAttackBehavior(new BossArcAttackBehavior(BossenemyData.SecondAttackProjectile, BossenemyData.projectileSpeed));
                break;
            case AttackType.JumpAttack:
              //  InitializeAttackBehavior(new JumpAttackBehavior(3f, 0.6f, 2f, 0.3f));
                break;
            case AttackType.specialRangedAttack:
                InitializeAttackBehavior(new BossSpecialAttackBehavior(BossenemyData.BasicAttackProjectile,3f,1,this, animController, this));
                break;
        }
    }
    private void InitializeAttackBehavior(IBossEnemyAttackBehavior behavior)
    {
        attackBehavior = behavior;
    }

    private void OnDrawGizmos()
    {
        if (showAttackGizmo)
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(attackGizmoCenter, attackGizmoSize);
        }
    }

    private void HandleCombat(float distanceToTarget)
    {
        if (distanceToTarget > BossenemyData.attackDistance && SpecialAttack == false)
        {
            waitingForAttack = false;
            showAttackGizmo = false;
            attackTimer = 0f;
            return;
        }

        attackTimer += Time.deltaTime;
        //  showAttackGizmo = false;
        if ((attackTimer >= BossenemyData.attackCooldown || !waitingForAttack) && SpecialAttack == false)
        {

            waitingForAttack = true;
            attackTimer = 0f;

            animController.Attack();
            OnAttack?.Invoke();
           // attackBehavior?.Attack(this, target);
            AttackCount++;
        }
    }

    private void BossSpecialAttackState()
    {
        SpecialAttackAnimState = true;
    }
    public IEnumerator doAttack()
    {
        if (BossenemyData.attackType == AttackType.specialRangedAttack)
        {
            angleOffset = 0f;

            for (int i = 0; i < 3; i++)
            {
                SpecialAttackAnimState = false;

                if (i > 0)
                {
                    animController.PlayAnim(BossenemyData.attackAnim2WithOutAttack);
                }

         
                Debug.Log("animasyon baþlatýldý");


                yield return new WaitUntil(() => SpecialAttackAnimState);

  
                attackBehavior?.Attack(this, target);

                angleOffset += 20f;
            }
            yield return new WaitForSeconds(1f);
            SpecialAttack = false;
            angleOffset = 0f;
        }
        else
        {
            attackBehavior?.Attack(this, target);
            yield return null;
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
        transform.position += direction * BossenemyData.moveSpeed * Time.deltaTime;
    }

    public void AnimationEvent_DealDamage()
    {
        if (target != null && target.TryGetComponent(out ICombat combatTarget))
        {
            combatTarget.TakeDamage(BossenemyData.damage);
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
        Debug.Log($"{BossenemyData.enemyName} died.");

        EntityPoolManager.Instance.ReleaseEntityToPool(BossenemyData.enemyPrefab, gameObject);
    }

    public Transform GetTransform() => transform;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ResetEnemy()
    {
        currentHealth = BossenemyData.health;
        attackTimer = UnityEngine.Random.Range(0, BossenemyData.attackTime * 0.6f);
        waitingForAttack = false;
        target = null;

        animController.ResetAnimator();
        ResetMaterials();
        animController.Idle();
    }
}
