public class MoveState : PlayerState
{
    public MoveState(PlayerController controller) : base(controller)
    {
        animatorController = controller.moveOverride;
    }

    public override void Enter()
    {
        controller.SetAnimation("Move");
    }

    public override void FixedUpdate()
    {
        controller.Move(controller.MoveInput);
    }

    public override void Update()
    {
        if (controller.MoveInput.magnitude <= 0.1f)
        {
            controller.ChangeState(new IdleState(controller));
        }
    }
}