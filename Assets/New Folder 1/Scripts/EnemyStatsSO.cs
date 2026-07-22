using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Zombie Smack/Enemy Stats")]
public class EnemyStatsSO : ScriptableObject
{
    [Header("Health")]
    public int maxHealth = 3;

    [Header("Movement")]
    public float moveSpeed = 3.5f;

    [Header("Combat")]
    public int attackDamage = 10;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float detectionRange = 15f;
}
