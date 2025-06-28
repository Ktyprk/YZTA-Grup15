using UnityEngine;
public class IdleState : PlayerState
{
    public IdleState(PlayerController controller) : base(controller)
    {
        animatorController = controller.idleOverride;
        
    }

    public override void Enter()
    {
        controller.SetAnimation("Idle");
    }

    public override void Update()
    {
        if (controller.MoveInput.magnitude > 0.1f)
        {
            controller.ChangeState(new MoveState(controller));
        }
    }
}