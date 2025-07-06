using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyData enemyData;

    private bool isIdling;
    private bool isWalking;

    private void Awake()
    {

        if (enemyData != null && animator != null)
        {
            animator.runtimeAnimatorController = enemyData.animatorController;
        }
    }

    public void PlayAnim(string name, int layer = 0)
    {
        if (!gameObject.activeInHierarchy || animator == null) return;
        animator.CrossFadeInFixedTime(name, 0.1f, layer, 0);
    }
    private bool IsCurrentAnimation(string animName)
    {
        if (animator == null) return false;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animName);
    }

    public void Idle()
    {
        if (isIdling) return;

        isIdling = true;
        isWalking = false;

    
            PlayAnim(enemyData.idleAnim);
    }

    public void Walk()
    {
        if (isWalking) return;

        isWalking = true;
        isIdling = false;

            PlayAnim(enemyData.walkAnim);
    }

    public void Attack()
    {
    
        isWalking = false;
        isIdling = false;

        PlayAnim(enemyData.attackAnim);
    }

    public void Hit()
    {
        PlayAnim(enemyData.hitAnim);
    }

    public void Die()
    {
        PlayAnim(enemyData.dieAnim);
    }

    // Animation Event çağrısı
    public void OnAttackAnimationHit()
    {
        //GetComponent<EnemyCombat>()?.OnAttackHit();
    }
    
    
    public void ResetAnimator()
    {
        if (animator == null) return;
        animator.Rebind();  
        animator.Update(0f); 
        
    }
}