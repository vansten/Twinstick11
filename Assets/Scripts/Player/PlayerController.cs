using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamagable
{
    #region Consts and statics

    protected static string SpeedName = "Speed";

    #endregion

    #region Variables

    [SerializeField]
    protected Transform _weaponParent;
    [SerializeField]
    protected float _speed;
    [SerializeField]
    protected float _baseHP;
    [SerializeField]
    protected WeaponType _startWeapon;

    protected Animator _animator;
    protected Vector3 _startPosition;

    #endregion

    #region Properties

    protected float _currentHP;
    public delegate void HPChangedDelegate(float hp, float percent);
    public event HPChangedDelegate OnHPChanged;
    public float CurrentHP
    {
        get { return _currentHP; }
        set
        {
            if (_currentHP != value)
            {
                _currentHP = value;

                if (_currentHP <= 0.0f)
                {
                    GameController.Instance.CurrentPhase = GamePhase.DeathScreen;
                }

                _currentHP = Mathf.Clamp(_currentHP, 0.0f, _baseHP);
                if (OnHPChanged != null)
                {
                    OnHPChanged(_currentHP, _currentHP / _baseHP);
                }
            }
        }
    }

    protected BaseWeapon _currentEquippedWeapon;
    public delegate void WeaponChangedDelegate(WeaponType weaponType);
    public event WeaponChangedDelegate OnWeaponChanged;
    public BaseWeapon CurrentEquippedWeapon
    {
        get
        {
            return _currentEquippedWeapon;
        }
        set
        {
            if(_currentEquippedWeapon != value)
            {
                _currentEquippedWeapon = value;

                if(OnWeaponChanged != null && _currentEquippedWeapon != null)
                {
                    OnWeaponChanged(_currentEquippedWeapon.Type);
                }
            }
        }
    }

    #endregion

    #region Unity methods

    protected void OnValidate()
    {
        if(_baseHP <= 0.0f)
        {
            _baseHP = 0.01f;
        }
    }

    protected void Awake()
    {
        GameController.Instance.OnGamePhaseChanged += OnGamePhaseChangedCallback;
        _startPosition = transform.position;
        _animator = GetComponent<Animator>();
    }

    protected void Start()
    {
        CurrentHP = _baseHP;
    }

    protected void Update()
    {
        ProcessTransform();

        if(_currentEquippedWeapon != null)
        {
            ProcessShoot();
        }
    }

    #endregion

    #region Methods
    
    public void EquipWeapon(BaseWeapon weapon)
    {
        if(CurrentEquippedWeapon != null)
        {
            CurrentEquippedWeapon.gameObject.SetActive(false);
            CurrentEquippedWeapon = null;
        }

        if (weapon == null)
        {
            return;
        }

        weapon.transform.parent = _weaponParent;
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        CurrentEquippedWeapon = weapon;
    }

    public void TakeDamage(float damage, Vector3 hitPosition)
    {
        GameController.Instance.SpawnHitParticlesAtPosition(HitType.Flesh, hitPosition);
        CurrentHP -= damage;
    }

    protected void ProcessTransform()
    {
        Vector3 translation = InputManager.GetMovementDirection() * _speed * Time.deltaTime;

        _animator.SetFloat(SpeedName, translation.magnitude);

        transform.position += translation;
        Vector3 forward = InputManager.GetForwardDirection() + InputManager.GetObjectToMouseDirection(transform.position);
        if(forward.magnitude > 0.001f)
        {
            transform.forward = forward;
        }
    }

    protected void ProcessShoot()
    {
        if(InputManager.IsShooting())
        {
            _currentEquippedWeapon.Shoot();
        }
    }

    protected void OnGamePhaseChangedCallback(GamePhase gamePhase)
    {
        enabled = gamePhase == GamePhase.Game;
        if(enabled)
        {
            transform.position = _startPosition;
            CurrentHP = _baseHP;
            BaseWeapon weapon = GameController.Instance.CreateWeapon(_startWeapon, _weaponParent);
            EquipWeapon(weapon);
        }
    }

    #endregion
}
