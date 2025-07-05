using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    [SerializeField] Animator animator;
    
    private bool isIdling;
    
    private void Start()
    {
        GetComponent<PlayerController>().OnStateChange += HandleStateChange;
    }

    private void HandleStateChange(PlayerState obj)
    {
        animator.runtimeAnimatorController = obj.animatorController;
    }

    public void PlayAnim(string name)
    {
        if (!gameObject.activeInHierarchy) return;

        animator.CrossFadeInFixedTime(name, 0.1f, 0, 0);
    }
    
    public void Walk()
    {
        isIdling = false;
        
        PlayAnim("Walk");
        
    }
    
    public void Idle()
    {
        if (isIdling) return;

        PlayAnim("Idle");
    }

    public void Dash()
    {
        PlayAnim("Dash");
    }
    
    public void JumpStart()
    {
        PlayAnim("JumpStart");
    }
    
    public void JumpIdle()
    {
        PlayAnim("JumpIdle");
    }
    
    public void JumpFinish()
    {
        PlayAnim("JumpFinish");
    }
}
