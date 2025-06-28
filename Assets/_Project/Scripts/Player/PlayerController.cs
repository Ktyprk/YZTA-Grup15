using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public AnimatorController animator;
    private PlayerState _currentState;

    public Vector2 MoveInput { get; private set; }
    
    public AnimatorOverrideController idleOverride;
    public AnimatorOverrideController moveOverride;
    public AnimatorOverrideController attackOverride;
    
    public Vector3 attackGizmoCenter;
    public Vector3 attackGizmoSize;
    public bool showAttackGizmo = false;
    
    public System.Action<PlayerState> OnStateChange;

    private void Start()
    {
        ChangeState(new IdleState(this));
        
        ControlsManager.Controls.Player.Attack.performed += ctx =>
        {
            if (!(_currentState is AttackState))
                ChangeState(new AttackState(this));
        };
    }

    private void Update()
    {
        MoveInput = ControlsManager.Controls.Player.Move.ReadValue<Vector2>();
        _currentState?.Update();
    }

    private void FixedUpdate()
    {
        _currentState?.FixedUpdate();
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
}
