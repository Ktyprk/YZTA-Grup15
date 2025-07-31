using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class FireBallDamage : MonoBehaviour , IProjectileDamageDealer
{
    [SerializeField] private int damageAmount = 20;
    [SerializeField] private int fireDamageAmount = 5;
    [SerializeField] private GameObject efect;
    public bool isSummonByBoss = false;
    private BossEnemyController bossEnemyController;
    private float timer;
    public EnemyData enemyData;
    private string skillName = "RangedAttack";
    public void Start()
    {
        timer = 0;
        bossEnemyController = FindAnyObjectByType<BossEnemyController>();
        if(isSummonByBoss)
        {
            bossEnemyController.missAttackNumber++;
            
        }
        Destroy(gameObject, 4);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ICombat Icombat = other.gameObject.GetComponent<ICombat>();
            if (Icombat != null)
            {
                if(isSummonByBoss)
                {
                    bossEnemyController.missAttackNumber = 0;
                }
             
                StartCoroutine(giveDamage(Icombat));

            }
        }
    }
    public void Update()
    {
        
        
    }
    public IEnumerator giveDamage(ICombat Icombat)
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;
        if(efect!=null)
        efect.SetActive(false);
        PlayerController playerController = FindAnyObjectByType<PlayerController>();
        Icombat.TakeDamage(damageAmount);
        playerController.AddAttack(enemyData.name, skillName, damageAmount);
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 3; i++)
        {
            Icombat.TakeDamage(fireDamageAmount);
            Debug.Log("damage Verildi");
            if(playerController!=null)
            playerController.AddAttack(enemyData.name, skillName, fireDamageAmount);
            yield return new WaitForSeconds(1f);

        }
        Destroy(gameObject);
    }
   /* public IEnumerator destroyobject()
    {
        yield return new WaitForSeconds(4);
        bossEnemyController.missAttackNumber++;
        Destroy(gameObject);
    }*/
}
