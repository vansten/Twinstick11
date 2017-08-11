using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBasedWeapon : BaseWeapon
{
    #region Variables

    [SerializeField]
    protected BaseProjectile _projectilePrefab;
    [SerializeField]
    protected Transform _projectileSpawnOrigin;
    [SerializeField]
    protected float _projectileSpeed;
    [SerializeField]
    protected float _shootDelay;

    protected ObjectPool<BaseProjectile> _projectilesPool = new ObjectPool<BaseProjectile>();
    protected float _lastShootTime;

    #endregion

    #region Unity Methods

    protected void Awake()
    {
        _projectilesPool.Init(_projectilePrefab, _projectileSpawnOrigin, GrowthStrategy.DoubleSize, 16);
    }

    protected void Update()
    {
        _projectilesPool.CollectInactiveObjects();
    }

    #endregion

    #region Methods

    public override void Shoot()
    {
        float currentTime = Time.realtimeSinceStartup;

        if(currentTime - _lastShootTime <= _shootDelay)
        {
            return;
        }

        _lastShootTime = currentTime;

        SpawnShootEffects();
        BaseProjectile projectile = _projectilesPool.GetObject(_projectileSpawnOrigin);
        projectile.Shoot(_projectileSpawnOrigin.forward, _projectileSpeed, _damage);
    }

    #endregion
}
