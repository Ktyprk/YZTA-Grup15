using UnityEngine;

public enum EnemyType { Aggressive, Defensive, Passive }
public enum AttackType { Melee, Heavy, Ranged }

[CreateAssetMenu(menuName = "Enemy/Create New Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("General")]
    public string enemyName;
    public EnemyType enemyType;
    public AttackType attackType;
    public GameObject enemyPrefab;

    [Header("Stats")]
    public int health = 100;
    public int damage = 10;
    public float moveSpeed = 2f;

    [Header("Combat")]
    public float attackTime = 1.2f; 
    public float attackCooldown = 2f;
    public float attackDistance = 1.5f;
    public float idleTime = 1f;
    public float blockTime = 0.5f;
    public float enemyDistance = 1f;

    [Header("Animations")]
    public RuntimeAnimatorController animatorController;

    [Header("Animation Names")]
    public string idleAnim = "Idle";
    public string walkAnim = "Walk";
    public string attackAnim = "Attack";
    public string hitAnim = "Hit";
    public string dieAnim = "Die";
}