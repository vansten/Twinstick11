using System;
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
    protected float _explosionRange;

    #endregion

    #region Unity Methods

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRange);
    }

    #endregion

    #region Methods

    protected override void HandleCollision(Collision collision)
    {
        Instantiate(_explosionParticles);
        int layerMask = 1 << LayerManager.Enemies;
        layerMask |= 1 << LayerManager.Player;
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, _explosionRange, Vector3.zero, _explosionRange, layerMask, QueryTriggerInteraction.Ignore);
        if(hits.Length > 0)
        {
            foreach(RaycastHit hit in hits)
            {
                if(hit.collider == null)
                {
                    continue;
                }

                IDamagable damagableObject = hit.collider.gameObject.GetComponent<IDamagable>();
                if(damagableObject != null)
                {
                    damagableObject.TakeDamage(_damage);
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

        gameObject.SetActive(false);
    }

    #endregion
}
