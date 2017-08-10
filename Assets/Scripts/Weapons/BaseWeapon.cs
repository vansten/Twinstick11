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
    public AudioSource ShootSound;
    public ParticleSystem ShootParticles;

    #endregion

    #region Methods

    public abstract void Shoot();

    protected void SpawnShootEffects()
    {
        if(ShootSound != null)
        {
            ShootSound.Play();
        }

        if(ShootParticles != null)
        {
            ShootParticles.Play();
        }
    }

    #endregion
}
