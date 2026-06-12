using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Data", fileName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Патрулирование")]

    public float patrolSpeed = 2f;
    public float patrolWaitTime = 1f;

    [Header("Обнаружение")]

    public float detectionRange = 4f;
    public float chaseRange = 6f;

    [Header("Преследование")]

    public float chaseSpeed = 4f;

    [Header("Возвращение")]

    public float returnSpeed = 2f;
    public float arrivalThreshold = 0.1f;
}