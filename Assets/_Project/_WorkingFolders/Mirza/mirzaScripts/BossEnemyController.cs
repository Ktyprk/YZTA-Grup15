using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


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
    [SerializeField] private GameObject endingScene;
    [SerializeField] private GameObject HealthBar;
    [Header("Gizmos")]
    public Vector3 attackGizmoCenter;
    public Vector3 attackGizmoSize;
    public bool showAttackGizmo = false;

    [Header("Target Settings")]
    [SerializeField] private Transform target;
    private PlayerController playerController;

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
    public int maxHealth;

    [Header("special skill Settings")]
    public int AttackCount = 0;
    private bool Ragemood = false;
    public float angleOffset = 0f;
    public bool SpecialAttack = false;
    public bool SpecialAttackAnimState = false;
    public bool summon = false;
    public bool vanished = false;
    public bool teleport = false;
    public GameObject[] summons;
    public int summonCount = 3;
    public int summonCountCheck;
    private float Timer = 0 ;
    public GameObject oldmanprefab;
    public GameObject summonsmoke;
    public GameObject summoncharactersSmoke;
    public Collider oldmanCollider;
    public GameObject chargeEffect;
    public int missAttackNumber = 0;
    public GameObject oldman;
    
    [SerializeField] private Image healthFillImage; 

    private void Awake()
    {
        teleport = false;
        playerController = FindAnyObjectByType<PlayerController>();
        SpecialAttack = false;
        animController = GetComponent<EnemyAnimatorController>();
        BossenemyData.attackType = AttackType.Ranged;
    }

    private void OnEnable()
    {
        summonCountCheck = summonCount;
        ResetEnemy();
        InitializeAttackBehavior();
        
    }

    private void FixedUpdate()
    {
        if(  vanished == false )
        {
            if (target == null || !target.gameObject.activeSelf)
            {
                target = null;
                WanderSimple();
                animController.Idle();
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
     
        if(summonCountCheck<=0 && missAttackNumber < 6)
        {

            Timer += Time.deltaTime;
            vanished = false;
            oldmanprefab.SetActive(true);
            oldmanCollider.enabled = true;
            if (Timer>30f)
            {
                Timer = 0;
                summon = false;
                TakeDamage(0);
            }
        }
  
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
            case AttackType.SummonAttack:
                InitializeAttackBehavior(new BossSummonAttackBehavior(summons, summoncharactersSmoke, summonCount));
                break;
            case AttackType.Teleport:
                InitializeAttackBehavior(new TeleportAttackBehavior(oldman));
                break;
            case AttackType.TeleportAttack:
                InitializeAttackBehavior(new afterTeleportAttackBehavior());
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
        if (distanceToTarget >= BossenemyData.attackDistance && SpecialAttack == false)
        {

            waitingForAttack = false;
            showAttackGizmo = false;
            attackTimer = 0f;
            return;
        }

        attackTimer += Time.deltaTime;
        //  showAttackGizmo = false;
        if ((attackTimer >= BossenemyData.attackCooldown || !waitingForAttack) && SpecialAttack == false && teleport ==false)
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


                Debug.Log("animasyon ba�lat�ld�");


                yield return new WaitUntil(() => SpecialAttackAnimState);


                attackBehavior?.Attack(this, target);

                angleOffset += 20f;
            }
            yield return new WaitForSeconds(1f);
            SpecialAttack = false;
            angleOffset = 0f;
        }
        else if (BossenemyData.attackType == AttackType.SummonAttack)
        {

            summonCountCheck = summonCount;
            attackBehavior?.Attack(this, target);
            GameObject smokeGreen = Instantiate(summonsmoke, transform.position, Quaternion.identity);
            oldmanprefab.SetActive(false);
            oldmanCollider.enabled = false;
            Destroy(smokeGreen, 2f);

        }
        else if (BossenemyData.attackType == AttackType.Teleport)
        {

            attackBehavior?.Attack(this, target);

            teleport = true;

            animController.Attack();
            OnAttack?.Invoke();
           


        }
        else if (BossenemyData.attackType == AttackType.TeleportAttack)
        {
            attackBehavior?.Attack(this, target);
          

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

        float normalizedHealth = (float)currentHealth / maxHealth;
        healthFillImage.fillAmount = Mathf.Clamp01(normalizedHealth);

        DamagePopUpGenerator.instance.CreatePopUp(transform.position + Vector3.up * 2, damage.ToString());

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashEffect());

        if (currentHealth <= 0)
        {
       
            Die();
        }

        if(currentHealth <= maxHealth / 2 && summon == false)
        {
            summon = true;
            vanished = true;
            waitingForAttack = true;
            attackTimer = 0f;

            animController.Attack();
            OnAttack?.Invoke();
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
        HealthBar.SetActive(false);
        endingScene.SetActive(true);
        OnDie?.Invoke();
        Debug.Log($"{BossenemyData.enemyName} died.");

        EntityPoolManager.Instance.ReleaseEntityToPool(BossenemyData.enemyPrefab, gameObject);
        if (healthFillImage != null)
            healthFillImage.fillAmount = 0f;
    }

    public Transform GetTransform() => transform;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ResetEnemy()
    {
        maxHealth = BossenemyData.Maxhealth;
        currentHealth = BossenemyData.health;
        attackTimer = UnityEngine.Random.Range(0, BossenemyData.attackTime * 0.6f);
        waitingForAttack = false;
        target = null;

        animController.ResetAnimator();
        ResetMaterials();
        animController.Idle();
    }
    public IEnumerator warningEffectTriggered()
    {
        GameObject smoke = Instantiate(chargeEffect, transform.position, Quaternion.identity);
        
        Destroy(smoke, 1f);
        playerController.warningMark.SetActive(true);
         yield return new WaitForSeconds(1);
        playerController.warningMark.SetActive(false);

    }
    public void teleportToEnemyFinished()
    {
        missAttackNumber = 0;
        teleport = false;
    }
    public void close()
    {
        endingScene.SetActive(false);
    }
}
