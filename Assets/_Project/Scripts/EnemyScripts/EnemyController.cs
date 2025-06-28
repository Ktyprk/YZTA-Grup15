using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private EnemyData enemyData;

    private Transform target;

    private float stanceTimer;
    private float attackTimer;
    private bool waitForAttack;

    public System.Action OnWaitForAttack, OnAttack, OnIdle;

    public EnemyAnimatorController animController;
    private int currentHealth;

    private void Awake()
    {
        if (enemyData == null)
        {
            Debug.LogError("EnemyData eksik: " + gameObject.name);
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
            animController.Walk();
            MoveToTarget();
        }

        HandleCombat(dist);
    }

    private void HandleCombat(float distanceToTarget)
    {
        attackTimer += Time.deltaTime;
        stanceTimer += Time.deltaTime;

        if (!waitForAttack && enemyData.attackTime - attackTimer < 0.6f)
        {
            waitForAttack = true;
            OnWaitForAttack?.Invoke();
        }

        if (attackTimer >= enemyData.attackTime && distanceToTarget <= enemyData.attackDistance)
        {
            attackTimer = 0;
            stanceTimer = 0;
            waitForAttack = false;

            Attack();
            OnAttack?.Invoke();
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
       
            
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * enemyData.moveSpeed * Time.deltaTime;
    }

    private void Attack()
    {
        Debug.Log($"{enemyData.enemyName} attacks with {enemyData.attackType}");

        if (animController != null)
            animController.Attack();
        
        ICombat combatTarget = target.GetComponent<ICombat>();
        if (combatTarget != null)
        {
            combatTarget.TakeDamage(enemyData.damage);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{enemyData.enemyName} died.");
        Destroy(gameObject);
    }
}
