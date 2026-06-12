using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    private enum EnemyState
    {
        Patrol,
        Chase,
        Return
    }

    [SerializeField] private EnemyData _data;
    [SerializeField] private Transform _player;

    [SerializeField] private Transform[] _patrolPoints;
    [SerializeField] private LayerMask _groundLayer;

    private Rigidbody2D _rb;
    private EnemyState _currentState;

    private int _currentPatrolIndex;
    private float _waitTimer;

    private int _facingDirection = 1;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _currentState = EnemyState.Patrol;

        if ( _patrolPoints == null || _patrolPoints.Length == 0 )
        {
            Debug.LogWarning($"{gameObject.name}: точки патруля не заданы", this);
        }
    }
}