using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    #region Variables

    [SerializeField]
    protected Transform _weaponSpawnTransform;
    [SerializeField]
    protected RangeFloat _cooldownRange;
    [Tooltip("Set this to spawn specific weapons")]
    [SerializeField]
    protected WeaponType _weaponTypeToSpawn;
    [SerializeField]
    protected Vector3 _weaponRotationAxis;
    [SerializeField]
    protected float _weaponRotationSpeed;

    protected BaseWeapon _currentSpawnedWeapon;
    protected float _cooldown;
    protected float _timer;
    protected bool _playerIn;

    #endregion

    #region Unity methods

    protected void Awake()
    {
        GameController.Instance.OnGamePhaseChanged += OnGamePhaseChangedCallback;
    }

    protected void Start()
    {
        _timer = 0.0f;
        _cooldown = _cooldownRange.Rand();
    }

    protected void Update()
    {
        if(_currentSpawnedWeapon != null)
        {
            _currentSpawnedWeapon.transform.Rotate(_weaponRotationAxis * _weaponRotationSpeed * Time.deltaTime);
            return;
        }

        if(_playerIn)
        {
            return;
        }

        _timer += Time.deltaTime;

        if(_timer >= _cooldown)
        {
            SpawnWeapon();
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerManager.Player)
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            if(player != null)
            {
                _playerIn = true;
                CollectWeapon(player);
            }
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerManager.Player)
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                _playerIn = false;
            }
        }
    }

    #endregion

    #region Methods

    protected void OnGamePhaseChangedCallback(GamePhase gamePhase)
    {
        enabled = gamePhase == GamePhase.Game;
        _timer = 0.0f;

        if (enabled && _currentSpawnedWeapon != null)
        {
            _currentSpawnedWeapon.gameObject.SetActive(false);
        }
    }

    protected void SpawnWeapon()
    {
        _timer = 0.0f;
        _cooldown = _cooldownRange.Rand();

        if(_weaponTypeToSpawn != WeaponType.None)
        {
            _currentSpawnedWeapon = GameController.Instance.CreateWeapon(_weaponTypeToSpawn, _weaponSpawnTransform);
        }
        else
        {
            _currentSpawnedWeapon = GameController.Instance.CreateWeapon(_weaponSpawnTransform);
        }
    }

    protected void CollectWeapon(PlayerController player)
    {
        if(_currentSpawnedWeapon == null)
        {
            return;
        }

        player.EquipWeapon(_currentSpawnedWeapon);
        _currentSpawnedWeapon = null;
    }

    #endregion
}
