using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : BaseEnemy
{
    #region Const and statics

    public static float ChasingDistance = 3.0f;

    #endregion

    #region Variables

    public float Damage;
    public float Speed;
    
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

    #endregion

    #region Unity methods

    protected void Start()
    {
        _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _navMeshAgent.speed = Speed;

        _stateMachine = new AI.StateMachine<MeleeEnemy>();
        _stateMachine.Init(this);
        _stateMachine.ChangeState<MeleeEnemyStates.ChasePlayer>();
    }

    protected void Update()
    {
        _stateMachine.Update(Time.deltaTime);
    }

    #endregion

    #region Methods

    public void NotifyAttackPlayer(PlayerController player)
    {
        player.TakeDamage(Damage);
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
            owner.NavMeshAgent.isStopped = false;
        }

        public override void OnExit(MeleeEnemy owner)
        {
            owner.NavMeshAgent.isStopped = true;
        }

        public override void Update(MeleeEnemy owner, float deltaSeconds)
        {
            owner.NavMeshAgent.SetDestination(GameController.Instance.Player.transform.position);
            if(owner.NavMeshAgent.remainingDistance < MeleeEnemy.ChasingDistance)
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
            owner.NotifyAttackPlayer(GameController.Instance.Player);
        }

        public override void OnExit(MeleeEnemy owner)
        {
        }

        public override void Update(MeleeEnemy owner, float deltaSeconds)
        {
            Vector3 direction = owner.transform.position - GameController.Instance.Player.transform.position;
            direction.y = 0.0f;
            if(direction.magnitude > MeleeEnemy.ChasingDistance)
            {
                owner.StateMachine.ChangeState<ChasePlayer>();
            }
        }
    }
}