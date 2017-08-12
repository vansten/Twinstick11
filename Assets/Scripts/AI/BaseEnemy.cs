using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour, IDamagable
{
    #region Variables

    [SerializeField]
    protected float _baseHP;

    protected float _currentHP;
    
    #endregion

    #region Unity Methods

    protected void OnEnable()
    {
        _currentHP = _baseHP;
    }

    #endregion

    #region Methods

    public void TakeDamage(float damage)
    {
        _currentHP -= damage;
        if (_currentHP <= 0.0f)
        {
            GameController.Instance.EnemySpawner.EnemyKilled(this);
        }
    }

    #endregion
}
