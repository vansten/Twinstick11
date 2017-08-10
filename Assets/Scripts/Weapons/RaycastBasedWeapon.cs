using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastBasedWeapon : BaseWeapon
{
    #region Variables

    public float ShootDelay;
    public uint RaycastCount;
    [Tooltip("Ignored when RaycastCount is less or equal to 1")]
    public float ShootAngle;
    public Transform RaycastOrigin;
    [Tooltip("0 or less means no overheat at all")]
    public int ShootsBeforeOverheat = -1;
    [Tooltip("Ignored when ShootsBeforeOverheat is greater than 0")]
    public float CoolingDownTime;

    protected float _lastShootTime;
    protected int _shoots;
    protected bool _overheated;

    #endregion

    #region Unity Methods

    protected void OnDrawGizmos()
    {
        if (RaycastOrigin == null)
        {
            return;
        }

        float initAngle = -ShootAngle * 0.5f;
        float angleDelta = ShootAngle / (float)RaycastCount;
        if (RaycastCount == 1)
        {
            initAngle = 0.0f;
        }
        for (int i = 0; i < RaycastCount; ++i)
        {
            Vector3 direction = Quaternion.Euler(0.0f, initAngle + i * angleDelta, 0.0f) * RaycastOrigin.forward;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(RaycastOrigin.position, RaycastOrigin.position + direction * 2.0f);
        }
    }

    protected void Update()
    {
        if(_overheated && (Time.realtimeSinceStartup - _lastShootTime) > CoolingDownTime)
        {
            _overheated = false;
            _shoots = 0;
        }
    }

    #endregion

    #region Methods

    public override void Shoot()
    {
        if (RaycastCount == 0)
        {
            return;
        }

        float currentTime = Time.realtimeSinceStartup;

        if(currentTime - _lastShootTime <= ShootDelay)
        {
            return;
        }

        _lastShootTime = currentTime;

        if(_overheated)
        {
            return;
        }

        SpawnShootEffects();

        if (ShootsBeforeOverheat > 0)
        {
            _shoots += 1;
            _overheated = _shoots >= ShootsBeforeOverheat;
        }

        float initAngle = -ShootAngle * 0.5f;
        float angleDelta = ShootAngle / (float)RaycastCount;
        if(RaycastCount == 1)
        {
            initAngle = 0.0f;
        }
        for (int i = 0; i < RaycastCount; ++i)
        {
            Vector3 direction = Quaternion.Euler(0.0f, initAngle + i * angleDelta, 0.0f) * RaycastOrigin.forward;
            TryKillByRaycast(RaycastOrigin.position, direction);
        }
    }

    protected void TryKillByRaycast(Vector3 startPosition, Vector3 direction)
    {
        int layerMask = 1 << LayerManager.Enemies;
        Ray ray = new Ray(RaycastOrigin.position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, layerMask))
        {
            if (hit.collider == null)
            {
                return;
            }

            Debug.Log(hit.collider.gameObject.name);
        }
    }

    #endregion
}
