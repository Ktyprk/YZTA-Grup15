using UnityEngine;

public class MushroomTurret : MonoBehaviour
{
    [SerializeField] private GameObject projectile;
    [SerializeField] private Animation anim;
    [SerializeField] private int coolDown;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private EnemyData enemyData;

    private float timer = 0 ;
    private Vector3 spawnLocation;
    void Start()
    {
        timer = 0;
        spawnLocation = transform.position + gameObject.transform.forward + new Vector3(0f,0.30f,0f);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(coolDown<timer)
        {
            timer = 0;
            anim.CrossFade("Damage");
            Fire();
           
        }
    }
    private void Fire()
    {
     GameObject spawnedProjectile = GameObject.Instantiate(projectile, spawnLocation, Quaternion.identity);
        FireBallDamage fireball = spawnedProjectile.GetComponent<FireBallDamage>();
        if(fireball!=null )
        {
            fireball.enemyData = enemyData;
        }

        Vector3 dir = gameObject.transform.forward;
        spawnedProjectile.GetComponent<Rigidbody>().linearVelocity = dir * projectileSpeed;
    }
}
