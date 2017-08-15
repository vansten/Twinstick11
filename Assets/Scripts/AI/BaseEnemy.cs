using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour, IDamagable
{
    #region Consts and statics

    protected static string MixName = "_Mix";

    #endregion

    #region Variables

    [SerializeField]
    protected float _baseHP;
    [SerializeField]
    protected HitType _hitParticlesType;
    [SerializeField]
    protected MeshRenderer[] _renderers;

    protected float _currentHP;
    
    #endregion

    #region Unity Methods

    protected virtual void OnEnable()
    {
        _currentHP = _baseHP;
        SetMaterialMix(0.0f);
    }

    #endregion

    #region Methods

    public void TakeDamage(float damage, Vector3 hitPosition)
    {
        _currentHP -= damage;
        GameController.Instance.SpawnHitParticlesAtPosition(_hitParticlesType, hitPosition);
        if (_currentHP <= 0.0f)
        {
            GameController.Instance.EnemySpawner.EnemyKilled(this);
        }

        float percent = _currentHP / _baseHP;
        SetMaterialMix(1.0f - percent);
    }

    public virtual void SetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    protected void SetMaterialMix(float mix)
    {
        foreach (MeshRenderer mr in _renderers)
        {
            mr.material.SetFloat(MixName, mix);
        }
    }

    [ContextMenu("Set renderers")]
    protected void SetRenderers()
    {
        _renderers = GetComponentsInChildren<MeshRenderer>();
    }

    #endregion
}
