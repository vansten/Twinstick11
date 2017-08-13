using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastBasedWeapon : BaseWeapon
{
    #region Variables

    [SerializeField]
    protected float _shootDelay;
    [SerializeField]
    protected uint _raycastCount;
    [Tooltip("Ignored when RaycastCount is less or equal to 1")]
    [SerializeField]
    protected float _shootAngle;
    [SerializeField]
    protected Transform _raycastOrigin;
    [Tooltip("0 or less means no overheat at all")]
    [SerializeField]
    protected int _shootsBeforeOverheat = -1;
    [Tooltip("Ignored when ShootsBeforeOverheat is greater than 0")]

    protected float _lastShootTime;
    protected int _shoots;
    protected bool _overheated;

    #endregion

    #region Unity Methods

    protected void OnDrawGizmos()
    {
        if (_raycastOrigin == null)
        {
            return;
        }

        float initAngle = -_shootAngle * 0.5f;
        float angleDelta = _shootAngle / (float)_raycastCount;
        if (_raycastCount == 1)
        {
            initAngle = 0.0f;
        }
        for (int i = 0; i < _raycastCount; ++i)
        {
            Vector3 direction = Quaternion.Euler(0.0f, initAngle + i * angleDelta, 0.0f) * _raycastOrigin.forward;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_raycastOrigin.position, _raycastOrigin.position + direction * 2.0f);
        }
    }

    protected void Update()
    {
        if(_shootsBeforeOverheat <= 0)
        {
            return;
        }
        
        if (!InputManager.IsShooting())
        {
            _shoots -= 1;
        }

        _overheated = _shoots >= _shootsBeforeOverheat;
    }

    #endregion

    #region Methods

    public override void Shoot()
    {
        if (_raycastCount == 0)
        {
            return;
        }

        float currentTime = Time.realtimeSinceStartup;

        if(currentTime - _lastShootTime <= _shootDelay)
        {
            return;
        }

        _lastShootTime = currentTime;

        if(_overheated)
        {
            return;
        }

        SpawnShootEffects();
        
        _shoots += 1;

        float initAngle = -_shootAngle * 0.5f;
        float angleDelta = _shootAngle / (float)_raycastCount;
        if(_raycastCount == 1)
        {
            initAngle = 0.0f;
        }
        for (int i = 0; i < _raycastCount; ++i)
        {
            Vector3 direction = Quaternion.Euler(0.0f, initAngle + i * angleDelta, 0.0f) * _raycastOrigin.forward;
            TryKillByRaycast(_raycastOrigin.position, direction);
        }
    }

    protected void TryKillByRaycast(Vector3 startPosition, Vector3 direction)
    {
        int layerMask = 1 << LayerManager.Enemies;
        layerMask |= 1 << LayerManager.Obstacles;
        Ray ray = new Ray(_raycastOrigin.position, direction);
        Debug.DrawRay(ray.origin, ray.direction, Color.red, 5.0f);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, layerMask))
        {
            if (hit.collider == null)
            {
                return;
            }

            IDamagable damagable = hit.collider.gameObject.GetComponent<IDamagable>();
            if(damagable != null)
            {
                damagable.TakeDamage(_damage);
            }
        }
    }

    #endregion
}
