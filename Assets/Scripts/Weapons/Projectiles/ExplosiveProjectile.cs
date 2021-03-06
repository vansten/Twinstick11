﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectile : BaseProjectile
{
    #region Variables

    [SerializeField]
    protected ParticleSystem _explosionParticles;
    [SerializeField]
    protected AudioSource _explosionSound;
    [SerializeField]
    protected MeshRenderer _meshRenderer;
    [SerializeField]
    protected float _explosionRange;
    [SerializeField]
    protected LayerMask _layersToAffect;

    protected Collider _collider;

    #endregion

    #region Unity Methods

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRange);
    }

    protected void OnEnable()
    {
        if(_meshRenderer != null)
        {
            _meshRenderer.enabled = true;
        }

        if (_collider == null)
        {
            _collider = GetComponent<Collider>();
        }
        _collider.enabled = true;
    }

    #endregion

    #region Methods

    protected override void HandleCollision(Collider collider)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRange, _layersToAffect.value, QueryTriggerInteraction.Ignore);
        if(colliders.Length > 0)
        {
            foreach (Collider col in colliders)
            {
                IDamagable damagableObject = col.gameObject.GetComponent<IDamagable>();
                if (damagableObject != null)
                {
                    damagableObject.TakeDamage(_damage, col.transform.position);
                }
            }
        }

        if(_explosionParticles != null)
        {
            _explosionParticles.Play();
        }

        if(_explosionSound != null)
        {
            _explosionSound.Play();
        }

        _direction = Vector3.zero;

        if(_explosionParticles != null && _explosionSound != null)
        {
            StartCoroutine(DisableAfterEffects());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    protected IEnumerator DisableAfterEffects()
    {
        if (_meshRenderer != null)
        {
            _meshRenderer.enabled = false;
        }

        if(_collider != null)
        {
            _collider.enabled = false;
        }

        while (_explosionParticles.isPlaying || _explosionSound.isPlaying)
        {
            yield return null;
        }

        gameObject.SetActive(false);
    }

    #endregion
}
