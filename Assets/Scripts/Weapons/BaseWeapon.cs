using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    None,
    Pistol,
    Shotgun,
    RocketLauncher,
    Minigun,

    Count
}

public abstract class BaseWeapon : MonoBehaviour
{
    #region Variables

    public WeaponType Type;
    [SerializeField]
    protected AudioSource _shootSound;
    [SerializeField]
    protected ParticleSystem _shootParticles;
    [SerializeField]
    protected float _damage;

    #endregion

    #region Methods

    public abstract void Shoot();

    protected void SpawnShootEffects()
    {
        if(_shootSound != null)
        {
            _shootSound.Play();
        }

        if(_shootParticles != null)
        {
            _shootParticles.Play();
        }
    }

    #endregion
}
