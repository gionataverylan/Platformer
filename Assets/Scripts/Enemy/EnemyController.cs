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

    private enum PatrolDirection { Left = -1, Right = 1 };

    private Vector2 _spawnPosition;
    private PatrolDirection _patrolDir;

    [SerializeField] private EnemyData _data;
    [SerializeField] private Transform _player;
    [SerializeField] private LayerMask _groundLayer;

    private Rigidbody2D _rb;
    private EnemyState _currentState;

    private float _waitTimer;

    private int _facingDirection = 1;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _currentState = EnemyState.Patrol;

        _spawnPosition = transform.position;

        _patrolDir = transform.localScale.x >= 0
            ? PatrolDirection.Right
            : PatrolDirection.Left;
    }

    private void Update()
    {
        CheckStateTransitions();

        switch (_currentState)
        {
            case EnemyState.Patrol:
                PatrolBehaviour();
                break;
            case EnemyState.Chase:
                ChaseBehaviour();
                break;
            case EnemyState.Return:
                ReturnBehaviour();  
                break;
        }

        UpdateFacingDirection();
    }

    private float DistanceToPlayer()
    {
        return Vector2.Distance(transform.position, _player.position);
    }

    private bool CanSeePlayer(float range)
    {
        return DistanceToPlayer() <= range;
    }

    private void CheckStateTransitions()
    {
        switch (_currentState)
        {
            case EnemyState.Patrol:
                if (CanSeePlayer(_data.detectionRange))
                {
                    ChangeState(EnemyState.Chase);
                }
                break;

            case EnemyState.Chase:
                if (!CanSeePlayer(_data.chaseRange))
                {
                    ChangeState(EnemyState.Return);
                }
                break;

            case EnemyState.Return:
                if (CanSeePlayer(_data.detectionRange))
                {
                    ChangeState(EnemyState.Chase);
                    break;
                }

                if (HasReachedPatrolPoint())
                {
                    ChangeState(EnemyState.Patrol);
                }
                break;
        }
    }

    private void ChangeState(EnemyState newState)
    {
        _waitTimer = 0;
        _currentState = newState;
    }

    private void PatrolBehaviour()
    {
        // Граница патруля в ТЕКУЩЕМ направлении движения.
        // (int)_patrolDir даёт нам -1 или 1
        float boundary = _spawnPosition.x + (int)_patrolDir * _data.patrolDistance;

        float distanceToBoundary = Mathf.Abs(boundary - transform.position.x);

        bool reachedBoundary = distanceToBoundary <= _data.arrivalThreshold;
        bool edgeAhead = !IsGroundAhead();

        bool wallAhead = IsWallAhead();

        if (reachedBoundary || edgeAhead || wallAhead)
        {
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
            _waitTimer += Time.deltaTime;

            if (_waitTimer >= _data.patrolWaitTime)
            {
                // Инвертируем направление: Right(1) <-> Left(-1)
                _patrolDir = (PatrolDirection)(-(int)_patrolDir);
                _waitTimer = 0f;
            }
            return;
        }

        _rb.linearVelocity = new Vector2(
            (int)_patrolDir * _data.patrolSpeed,
            _rb.linearVelocity.y
        );
    }

    private bool IsGroundAhead()
    {
        Vector2 checkPosition = (Vector2)transform.position +
            new Vector2((int)_patrolDir * _data.edgeCheckDistance, 0);

        RaycastHit2D hit = Physics2D.Raycast(
            checkPosition,
            Vector2.down,
            _data.edgeCheckLength,
            _groundLayer
        );

        return hit.collider != null;
    }

    private bool IsWallAhead()
    {
        Vector2 origin = (Vector2)transform.position + new Vector2(0, _data.wallCheckHeight);

        Vector2 direction = Vector2.right * (int)_patrolDir;

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            direction,
            _data.wallCheckDistance,
            _groundLayer
        );

        return hit.collider != null;
    }

    private void ChaseBehaviour()
    {
        float direction = Mathf.Sign(_player.position.x - transform.position.x);

        _rb.linearVelocity = new Vector2(
            direction * _data.chaseSpeed,
            _rb.linearVelocity.y
        );
    }

    private void ReturnBehaviour()
    {
            // "Дом" — теперь просто _spawnPosition
        float distanceX = Mathf.Abs(_spawnPosition.x - transform.position.x);

        if (distanceX <= _data.arrivalThreshold)
        {
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
            return;
        }

        float direction = Mathf.Sign(_spawnPosition.x - transform.position.x);

        _rb.linearVelocity = new Vector2(
            direction * _data.returnSpeed,
            _rb.linearVelocity.y
        );
    }

    private bool HasReachedPatrolPoint()
    {
        float distanceX = Mathf.Abs(_spawnPosition.x - transform.position.x);
        return distanceX <= _data.arrivalThreshold;
    }

    private void UpdateFacingDirection()
    {
        float velocityX = _rb.linearVelocity.x;

        const float threshold = 0.05f;

        if (velocityX > threshold)
        {
            _facingDirection = 1;
        } 
        else if (velocityX < -threshold)
        {
            _facingDirection = -1;
        }

        transform.localScale = new Vector3(
            _facingDirection,
            transform.localScale.y,
            transform.localScale.z
        );
    }

    private void OnDrawGizmos()
    {
    // Зоны обнаружения — два круга разного цвета
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _data != null ? _data.detectionRange : 4f);

        Gizmos.color = new Color(1f, 0.5f, 0f); // оранжевый
        Gizmos.DrawWireSphere(transform.position, _data != null ? _data.chaseRange : 6f);

        // Зона патруля — линия от spawn-точки в обе стороны
        Gizmos.color = Color.cyan;

        if (_data != null && Application.isPlaying)
        {
            Vector2 checkPosition = (Vector2)transform.position +
            new Vector2((int)_patrolDir * _data.edgeCheckDistance, 0);

            bool groundAhead = IsGroundAhead();

            Gizmos.color = groundAhead ? Color.green : Color.red;
            Gizmos.DrawLine(checkPosition, checkPosition + Vector2.down * _data.edgeCheckLength);
            Gizmos.DrawWireSphere(checkPosition + Vector2.down * _data.edgeCheckLength, 0.05f);
        }

        if (_data != null && Application.isPlaying)
        {
            Vector2 wallOrigin = (Vector2)transform.position + new Vector2(0, _data.wallCheckHeight);
            Vector2 wallDir = Vector2.right * (int)_patrolDir;

            bool wallAhead = IsWallAhead();

            Gizmos.color = wallAhead ? Color.red : Color.green;
            Gizmos.DrawLine(wallOrigin, wallOrigin + wallDir * _data.wallCheckDistance);
            Gizmos.DrawWireSphere(wallOrigin + wallDir * _data.wallCheckDistance, 0.05f);
        }

        // В редакторе (до запуска игры) _spawnPosition ещё не задана —
        // используем текущую позицию transform как приближение
        Vector3 spawnPos = Application.isPlaying ? (Vector3)_spawnPosition : transform.position;

        Vector3 leftBound  = spawnPos + Vector3.left  * (_data != null ? _data.patrolDistance : 3f);
        Vector3 rightBound = spawnPos + Vector3.right * (_data != null ? _data.patrolDistance : 3f);

        Gizmos.DrawLine(leftBound, rightBound);
        Gizmos.DrawWireSphere(leftBound, 0.15f);
        Gizmos.DrawWireSphere(rightBound, 0.15f);
        Gizmos.DrawWireSphere(spawnPos, 0.1f);

        // Цвет текущего состояния — рисуем прямо над врагом
        Gizmos.color = _currentState switch
        {
            EnemyState.Patrol => Color.green,
            EnemyState.Chase  => Color.red,
            EnemyState.Return => Color.blue,
            _ => Color.white
        };
        Gizmos.DrawWireCube(transform.position + Vector3.up * 1.2f, Vector3.one * 0.3f);
    }
}