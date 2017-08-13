using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : BaseEnemy
{
    #region Const and statics

    public static string AttackTriggerName = "Attack";
    public static string SpeedName = "Speed";

    #endregion

    #region Variables

    public float Damage;
    public float Speed;
    public float ChasingDistance;
    public List<EnemyAttackPoint> AttackPoints;
    
    #endregion

    #region Properties

    protected UnityEngine.AI.NavMeshAgent _navMeshAgent;
    public UnityEngine.AI.NavMeshAgent NavMeshAgent
    {
        get { return _navMeshAgent; }
    }

    protected AI.StateMachine<MeleeEnemy> _stateMachine;
    public AI.StateMachine<MeleeEnemy> StateMachine
    {
        get { return _stateMachine; }
    }

    protected Animator _animator;
    public Animator Animator
    {
        get { return _animator; }
    }

    #endregion

    #region Unity methods

    protected override void OnEnable()
    {
        base.OnEnable();

        _animator = GetComponent<Animator>();

        _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _navMeshAgent.speed = Speed;

        if (_stateMachine == null)
        {
            _stateMachine = new AI.StateMachine<MeleeEnemy>();
            _stateMachine.Init(this);
        }
        _stateMachine.ChangeState<MeleeEnemyStates.ChasePlayer>();
    }

    protected void Awake()
    {
        foreach(EnemyAttackPoint attackPoint in AttackPoints)
        {
            attackPoint.SetEnemy(this);
        }
    }

    protected void Update()
    {
        if(Animator != null)
        {
            Animator.SetFloat(SpeedName, NavMeshAgent.velocity.magnitude);
        }

        if(_stateMachine != null)
        {
            _stateMachine.Update(Time.deltaTime);
        }
    }

    #endregion

    #region Methods

    public void NotifyAttackPlayer(PlayerController player, Vector3 hitPosition)
    {
        if(player != null)
        {
            player.TakeDamage(Damage, hitPosition);
        }
    }

    public void NotifyEnableAttackPoints()
    {
        foreach(EnemyAttackPoint attackPoint in AttackPoints)
        {
            attackPoint.enabled = true;
        }
    }

    public void NotifyDisableAttackPoints()
    {
        foreach (EnemyAttackPoint attackPoint in AttackPoints)
        {
            attackPoint.enabled = false;
        }
    }

    #endregion
}

namespace MeleeEnemyStates
{
    public class ChasePlayer : AI.StateMachine<MeleeEnemy>.State
    {
        public override void OnEnter(MeleeEnemy owner)
        {
            owner.NavMeshAgent.SetDestination(GameController.Instance.Player.transform.position);
            owner.NavMeshAgent.speed = owner.Speed;
            owner.NavMeshAgent.stoppingDistance = owner.ChasingDistance;
            owner.NavMeshAgent.isStopped = false;

            owner.NotifyDisableAttackPoints();
        }

        public override void OnExit(MeleeEnemy owner)
        {
            owner.NavMeshAgent.velocity = Vector3.zero;
            owner.NavMeshAgent.isStopped = true;
        }

        public override void Update(MeleeEnemy owner, float deltaSeconds)
        {
            owner.NavMeshAgent.SetDestination(GameController.Instance.Player.transform.position);

            Vector3 difference = GameController.Instance.Player.transform.position - owner.transform.position;
            difference.y = 0.0f;
            if (difference.magnitude < owner.ChasingDistance)
            {
                owner.StateMachine.ChangeState<Attack>();
            }
        }
    }

    public class Attack : AI.StateMachine<MeleeEnemy>.State
    {
        public override void OnEnter(MeleeEnemy owner)
        {
            owner.NavMeshAgent.speed = 0.0f;
            owner.Animator.SetBool(MeleeEnemy.AttackTriggerName, true);
        }

        public override void OnExit(MeleeEnemy owner)
        {
            owner.Animator.SetBool(MeleeEnemy.AttackTriggerName, false);
        }

        public override void Update(MeleeEnemy owner, float deltaSeconds)
        {
            Vector3 direction = GameController.Instance.Player.transform.position - owner.transform.position;
            direction.y = 0.0f;
            if(direction.magnitude > owner.ChasingDistance * 1.2f)
            {
                owner.StateMachine.ChangeState<ChasePlayer>();
            }
            owner.transform.forward = direction.normalized;
        }
    }
}