using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour, ICombat
{
    public AnimatorController animator;
    private PlayerState _currentState;
    public LayerMask enemyLayer;

    public Vector2 MoveInput { get; private set; }
    
    public AnimatorOverrideController idleOverride;
    public AnimatorOverrideController moveOverride;
    public AnimatorOverrideController attackOverride;
    
    public Vector3 attackGizmoCenter;
    public Vector3 attackGizmoSize;
    public bool showAttackGizmo = false;
    
    public System.Action<PlayerState> OnStateChange;
    
    public int maxHealth = 100;
    private int currentHealth;

    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private bool isDashing = false;
    private float dashTime = 0f;
    private float lastDashTime = -Mathf.Infinity;
    private Vector3 dashDirection;

    [Header("Damage Flash")]
    [SerializeField] private List<SkinnedMeshRenderer> renderers;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float flashDuration = 0.1f;

    private Coroutine flashRoutine;
    
    [HideInInspector] public PlayerStatsController playerStats;

    private void Start()
    {
        playerStats = GetComponent<PlayerStatsController>();
        currentHealth = maxHealth;
        ChangeState(new IdleState(this));
        
        ControlsManager.Controls.Player.Attack.performed += ctx =>
        {
            if (!(_currentState is AttackState))
                ChangeState(new AttackState(this));
        };
        
        ControlsManager.Controls.Player.Dash.performed += ctx =>
        {
            TryDash();
        };
    }


    private void Update()
    {
        MoveInput = ControlsManager.Controls.Player.Move.ReadValue<Vector2>();

         if (isDashing)
        {
        dashTime += Time.deltaTime;
        if (dashTime >= dashDuration)
        {
            isDashing = false;

            if (MoveInput.sqrMagnitude > 0.01f)
            {
                ChangeState(new MoveState(this));
            }
            else
            {
                ChangeState(new IdleState(this));
            }
        }
        }
        else
         {
        _currentState?.Update();
         }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            transform.position += dashDirection * dashSpeed * Time.fixedDeltaTime;
        }
        else
        {
            _currentState?.FixedUpdate();
        }
    }

    public void ChangeState(PlayerState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();
        OnStateChange?.Invoke(_currentState);
    }

    public void Move(Vector2 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            Vector3 move = new Vector3(direction.x, 0, direction.y);
            transform.position += move * Time.fixedDeltaTime * 5f;
            
            transform.forward = move;
        }
    }

    public void SetAnimation(string stateName)
    {
        switch (stateName)
        {
            case "Idle":
                animator.Idle();
                break;
            case "Move":
                animator.Walk();
                break;
            case "Attack":
                animator.PlayAnim("Attack");
                break;
            default:
                animator.Idle();
                break;
        }
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

   public void TakeDamage(int amount)
    {
        playerStats.currentHealth = Mathf.Clamp(playerStats.currentHealth - amount, 0, playerStats.maxHealth);
        //currentHealth -= amount;

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashEffect());

        if ( playerStats.currentHealth  <= 0)
            Die();
    }

    private void Die()
    {
       gameObject.SetActive(false);
    }

    public Transform GetTransform()
    {
        return transform;
    }

    private void TryDash()
    {
        if (isDashing) return;
        if (Time.time < lastDashTime + dashCooldown) return;
        if (MoveInput.sqrMagnitude < 0.01f) return; 

        if (_currentState is AttackState)
        {
            ChangeState(new IdleState(this));
        }

        animator.Dash();
        isDashing = true;
        dashTime = 0f;
        lastDashTime = Time.time;
        dashDirection = new Vector3(MoveInput.x, 0, MoveInput.y).normalized;

    
        if (dashDirection.sqrMagnitude > 0.01f)
            transform.forward = dashDirection;
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
