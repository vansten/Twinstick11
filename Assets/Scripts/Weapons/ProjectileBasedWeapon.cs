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
    [SerializeField]
    protected uint _projectileCount;

    protected ObjectPool<BaseProjectile> _projectilesPool = new ObjectPool<BaseProjectile>();
    protected float _lastShootTime;
    protected uint _projectilesLeft;

    #endregion

    #region Unity Methods

    protected void Awake()
    {
        _projectilesPool.Init(_projectilePrefab, _projectileSpawnOrigin, GrowthStrategy.DoubleSize, (int)_projectileCount);
    }

    protected void OnEnable()
    {
        _projectilesLeft = _projectileCount;
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
        projectile.transform.localScale = _projectilePrefab.transform.localScale;
        projectile.Shoot(_projectileSpawnOrigin.forward, _projectileSpeed, _damage);

        --_projectilesLeft;
        if(_projectilesLeft == 0)
        {
            GameController.Instance.Player.EquipBaseWeapon();
        }
    }

    public override float GetReadyPercent()
    {
        return (float)_projectilesLeft / (float)_projectileCount;
    }

    #endregion
}
