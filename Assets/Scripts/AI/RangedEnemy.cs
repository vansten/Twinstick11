using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : BaseEnemy
{
    #region Consts and statics

    public static string SpeedName = "Speed";

    #endregion

    #region Variables

    public ParticleSystem ShootParticles;
    public AudioSource ShootSound;
    public Transform Tower;
    public Transform ProjectileSpawnOrigin;
    public BaseProjectile ProjectilePrefab;
    public float Damage;
    public float AttackRate;
    public float Speed;
    public float TargetRangeDistance;
    public float ProjectileSpeed;
    public float MaxTowerAngleDifference;
    public float TowerRotationSpeed;

    #endregion

    #region Properties

    protected UnityEngine.AI.NavMeshAgent _navMeshAgent;
    public UnityEngine.AI.NavMeshAgent NavMeshAgent
    {
        get { return _navMeshAgent; }
    }

    protected Animator _animator;
    public Animator Animator
    {
        get { return _animator; }
    }

    protected AI.StateMachine<RangedEnemy> _stateMachine;
    public AI.StateMachine<RangedEnemy> StateMachine
    {
        get { return _stateMachine; }
    }

    protected ObjectPool<BaseProjectile> _projectilesPool;
    public ObjectPool<BaseProjectile> ProjectilesPool
    {
        get { return _projectilesPool; }
    }

    protected Quaternion _towerInitialLocalRotation;
    public Quaternion TowerInitialLocalRotation
    {
        get { return _towerInitialLocalRotation; }
    }

    #endregion

    #region Unity Methods

    protected override void OnEnable()
    {
        base.OnEnable();

        _animator = GetComponent<Animator>();

        _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _navMeshAgent.speed = Speed;

        _towerInitialLocalRotation = Tower.localRotation;

        _projectilesPool = new ObjectPool<BaseProjectile>();
        _projectilesPool.Init(ProjectilePrefab, ProjectileSpawnOrigin, GrowthStrategy.DoubleSize, 8);

        if (_stateMachine == null)
        {
            _stateMachine = new AI.StateMachine<RangedEnemy>();
            _stateMachine.Init(this);
            _stateMachine.ChangeState<RangedEnemyStates.MoveToPosition>();
        }
    }

    protected void Update()
    {
        if (Animator != null)
        {
            Animator.SetFloat(SpeedName, NavMeshAgent.velocity.magnitude);
        }

        if (_stateMachine != null)
        {
            _stateMachine.Update(Time.deltaTime);
        }
    }

    #endregion

    #region Methods

    public Vector3 GetTargetPosition()
    {
        Vector3 playerPosition = GameController.Instance.Player.transform.position;
        Vector3 playerToMe = transform.position - playerPosition;
        playerToMe.y = 0.0f;
        bool done = false;
        int repeatCount = 0;
        Ray ray = new Ray(playerPosition, playerToMe.normalized);
        int[] multipliers = new int[13] { 0, 1, -1, 2, -2, 3, -3, 4, -4, 5, -5, 6, -6 };
        while(!done && repeatCount < multipliers.Length)
        {
            ray.direction = (Quaternion.Euler(0.0f, multipliers[repeatCount] * 30.0f, 0.0f) * playerToMe).normalized;
            if(!Physics.Raycast(ray, playerToMe.magnitude, 1 << LayerManager.Obstacles, QueryTriggerInteraction.Ignore))
            {
                done = true;
            }
            ++repeatCount;
        }

        return ray.origin + ray.direction * TargetRangeDistance;
    }

    public bool IsTargetVisible()
    {
        Vector3 playerPosition = GameController.Instance.Player.transform.position;
        Vector3 playerToMe = transform.position - playerPosition;
        playerToMe.y = 0.0f;
        Ray ray = new Ray(playerPosition, playerToMe.normalized);
        return !Physics.Raycast(ray, playerToMe.magnitude, 1 << LayerManager.Obstacles, QueryTriggerInteraction.Ignore);
    }

    #endregion
}

namespace RangedEnemyStates
{
    public class MoveToPosition : AI.StateMachine<RangedEnemy>.State
    {
        public override void OnEnter(RangedEnemy owner)
        {
            owner.NavMeshAgent.SetDestination(owner.GetTargetPosition());
            owner.NavMeshAgent.speed = owner.Speed;
            owner.NavMeshAgent.isStopped = false;
        }

        public override void OnExit(RangedEnemy owner)
        {
            owner.NavMeshAgent.velocity = Vector3.zero;
            owner.NavMeshAgent.isStopped = true;
        }

        public override void Update(RangedEnemy owner, float deltaSeconds)
        {
            owner.NavMeshAgent.SetDestination(owner.GetTargetPosition());

            owner.Tower.localRotation = Quaternion.RotateTowards(owner.Tower.localRotation, owner.TowerInitialLocalRotation, deltaSeconds * owner.TowerRotationSpeed);

            Vector3 toPlayer = GameController.Instance.Player.transform.position - owner.transform.position;
            toPlayer.y = 0.0f;
            if (toPlayer.magnitude <= owner.TargetRangeDistance && toPlayer.magnitude >= owner.TargetRangeDistance * 0.6f && owner.IsTargetVisible())
            {
                owner.StateMachine.ChangeState<RotateTowerToPlayer>();
            }
        }
    }

    public class RotateTowerToPlayer : AI.StateMachine<RangedEnemy>.State
    {
        public override void OnEnter(RangedEnemy owner)
        {
        }

        public override void OnExit(RangedEnemy owner)
        {
        }

        public override void Update(RangedEnemy owner, float deltaSeconds)
        {
            Vector3 toPlayer = GameController.Instance.Player.transform.position - owner.Tower.position;
            toPlayer.y = 0.0f;
            toPlayer.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(toPlayer, Vector3.up);
            owner.Tower.rotation = Quaternion.RotateTowards(owner.Tower.rotation, targetRotation, deltaSeconds * owner.TowerRotationSpeed);

            if(Quaternion.Angle(owner.Tower.rotation, targetRotation) <= owner.MaxTowerAngleDifference)
            {
                owner.StateMachine.ChangeState<Attack>();
            }
        }
    }

    public class Attack : AI.StateMachine<RangedEnemy>.State
    {
        protected float _timer;

        public override void OnEnter(RangedEnemy owner)
        {
            _timer = 0.0f;
        }

        public override void OnExit(RangedEnemy owner)
        {
        }

        public override void Update(RangedEnemy owner, float deltaSeconds)
        {
            _timer += deltaSeconds;
            if(_timer >= owner.AttackRate)
            {
                _timer = 0.0f;

                Shoot(owner);

                Vector3 toPlayer = GameController.Instance.Player.transform.position - owner.Tower.position;
                toPlayer.y = 0.0f;
                if(toPlayer.magnitude > owner.TargetRangeDistance * 1.2f || toPlayer.magnitude < owner.TargetRangeDistance * 0.6f)
                {
                    owner.StateMachine.ChangeState<MoveToPosition>();
                    return;
                }

                toPlayer.Normalize();
                Quaternion targetRotation = Quaternion.LookRotation(toPlayer, Vector3.up);
                if (Quaternion.Angle(owner.Tower.rotation, targetRotation) > owner.MaxTowerAngleDifference)
                {
                    owner.StateMachine.ChangeState<RotateTowerToPlayer>();
                    return;
                }

                if(!owner.IsTargetVisible())
                {
                    owner.StateMachine.ChangeState<MoveToPosition>();
                    return;
                }
            }
        }

        protected void Shoot(RangedEnemy owner)
        {
            if (owner.ShootParticles != null)
            {
                owner.ShootParticles.Play();
            }

            if (owner.ShootSound != null)
            {
                owner.ShootSound.Play();
            }

            BaseProjectile projectile = owner.ProjectilesPool.GetObject(owner.ProjectileSpawnOrigin);
            projectile.transform.localScale = owner.ProjectilePrefab.transform.localScale;
            projectile.Shoot(owner.ProjectileSpawnOrigin.forward, owner.ProjectileSpeed, owner.Damage);
        }
    }
}