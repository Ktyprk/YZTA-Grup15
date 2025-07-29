using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    [SerializeField] public Animator animator;
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private EnemyBossData enemyBossData;
    private BossEnemyController bossEnemyController;

    private bool isIdling;
    private bool isWalking;
    private bool isBoss=false;
    private float timer = 0;

    private int attackCounter=0;
    public bool RageAttack=false;
   

    private void Awake()
    {
        bossEnemyController = GetComponent<BossEnemyController>();

        RageAttack = false;
        if (bossEnemyController != null && enemyBossData != null)
        {
            isBoss = true;
        }
        else
        {
            isBoss = false;
        }
        if (enemyBossData!=null)
        {
            isBoss = true;
        }
            
        if (enemyData != null && animator != null)
        {
            animator.runtimeAnimatorController = enemyData.animatorController;
        }
    }
    private void Update()
    {
       
        if (RageAttack) 
        {
            timerForRage();
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

    
        if(isBoss)
        {
            PlayAnim(enemyBossData.idleAnim);
        }
        else
        {
            PlayAnim(enemyData.idleAnim);
        }
           
    }

    public void Walk()
    {
        if (isWalking) return;

        isWalking = true;
        isIdling = false;


        if (isBoss)
        {
            PlayAnim(enemyBossData.walkAnim);
        }
        else
        {
            PlayAnim(enemyData.walkAnim);
        }
        
    }
    
    public void replicaSpecialAttack()
    {
        PlayAnim(enemyBossData.attackAnim2WithOutAttack);
    }
    public void Attack()
    {
    
        isWalking = false;
        isIdling = false;

        if(isBoss)
        {   if(bossEnemyController.vanished)
            {
                bossEnemyController.SpecialAttack = false;
                bossEnemyController.AttackCount = 0;
                attackCounter = 0;
                RageAttack = false;
                enemyBossData.attackType = AttackType.SummonAttack;
                bossEnemyController.InitializeAttackBehavior();
                PlayAnim(enemyBossData.Summon);
            }
            else
            {
                if (bossEnemyController.AttackCount > 4 && !RageAttack && attackCounter < 3)
                {
                    bossEnemyController.SpecialAttack = true;
                    bossEnemyController.AttackCount = 0;
                    enemyBossData.attackType = AttackType.specialRangedAttack;
                    bossEnemyController.InitializeAttackBehavior();
                    PlayAnim(enemyBossData.attackAnim2);
                    attackCounter++;


                    Debug.Log("special attack");


                }
                else if (bossEnemyController.AttackCount <= 4 && !RageAttack && attackCounter < 3)
                {
                    Debug.Log("normal attack");
                    enemyBossData.attackType = AttackType.Ranged;
                    bossEnemyController.InitializeAttackBehavior();
                    PlayAnim(enemyBossData.attackAnim);
                }
                else if (attackCounter >= 3)
                {
                    attackCounter = 0;
                    bossEnemyController.SpecialAttack = false;
                    Debug.Log("rage attack");
                    RageAttack = true;
                    enemyBossData.attackType = AttackType.ArcRanged;
                    bossEnemyController.InitializeAttackBehavior();
                }
                else if (RageAttack && attackCounter < 3)
                {
                    Debug.Log("rage attack2");
                    enemyBossData.attackType = AttackType.ArcRanged;
                    bossEnemyController.InitializeAttackBehavior();
                    bossEnemyController.AttackCount = 0;
                    PlayAnim(enemyBossData.attackAnim3);
                }
            }
            

        }
        else
        {
            PlayAnim(enemyData.attackAnim);
        }
           
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
    private void timerForRage()
    {
       
        timer += Time.deltaTime;
        if(timer>20f)
        {
            RageAttack = false;
            timer =0;
        }
    }
}