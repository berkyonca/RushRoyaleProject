using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hero : MonoBehaviour, ICharacter
{
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected LayerMask opponentLayer;
    [SerializeField] protected float range;
    [SerializeField] protected float attackInterval;
    [SerializeField] protected float bulletSpeed;
    [SerializeField] protected int damage;

    protected Opponent _targetOpponent;
    protected SpecialAbility _specialAbility;
    protected EnemySpawner _enemySpawner;
    protected string _unityType;
    protected string _target;

    private State _currentState;
    private float _attackCoolDown;

    #region Properties

    public Opponent TargetOpponent { get; protected set; }
    public IEnumerable<PathFollower> PathFollowers { get; set; }
    public Collider2D[] OpponentColliders { get; set; }
    public int Damage { get => damage; }

    #endregion


    protected virtual void Start()
    {
        _attackCoolDown = attackInterval;
        _enemySpawner = FindObjectOfType<EnemySpawner>();
        SetState(new HeroIdleState(this));
    }

    protected virtual void Update()
    {
        SetOpponentsInRange();
        _currentState.Tick();
    }

    public void SetState(State state)
    {
        if (_currentState != null)
            _currentState.OnStateExit();

        _currentState = state;

        if (_currentState != null)
            _currentState.OnStateEnter();
    }

    private void SetOpponentsInRange()
    {
        OpponentColliders = Physics2D.OverlapCircleAll(transform.position, range, opponentLayer);
        if (OpponentColliders.Length == 0)
            return;

        PathFollowers = OpponentColliders.Select(opponent => opponent.GetComponent<PathFollower>());
    }

    public IEnumerator FindLeadingOpponent()
    {
        while (true)
        {
            if (PathFollowers != null)
            {
                PathFollower leadingOpponent = PathFollowers.Single(opponent => opponent.DistanceTravelled == PathFollowers.Max(opponent => opponent.DistanceTravelled));
                TargetOpponent = leadingOpponent.GetComponent<Opponent>();
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position, range);
        if (TargetOpponent != null)
            Gizmos.DrawWireSphere(TargetOpponent.transform.position, 1);
    }

    public void Attack()
    {
        _attackCoolDown += Time.deltaTime;

        if (_attackCoolDown >= attackInterval)
        {
            GameObject bulletObject = Instantiate(projectilePrefab, (Vector2)transform.position, Quaternion.identity);
            Projectile bullet = bulletObject.GetComponent<Projectile>();
            bullet.StartCoroutine(bullet.MoveToTarget(this, TargetOpponent, bulletSpeed));
            _attackCoolDown = 0;
           // MultiplayerManager.Instance.Send(MultiplayerManager.Code.Attack, );
            MultiplayerManager.Instance.Send(MultiplayerManager.Code.Attack, );
        }
    }

    public void SetPosition(Vector3 position)
    {
        // To be filled later
    }
}
