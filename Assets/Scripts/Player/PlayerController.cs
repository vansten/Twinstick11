using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamagable
{
    #region Consts and statics

    protected static string SpeedName = "Speed";
    protected static string DirectionName = "Direction";

    protected static float onePer360 = 1.0f / 360.0f;

    #endregion

    #region Variables

    [SerializeField]
    protected Transform _weaponParent;
    [SerializeField]
    protected Transform _crosshair;
    [SerializeField]
    protected float _speed;
    [SerializeField]
    protected float _baseHP;
    [SerializeField]
    protected WeaponType _startWeapon;

    protected Animator _animator;
    protected Rigidbody _rigidbody;
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
                    _animator.SetFloat(SpeedName, 0.0f);
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
        _rigidbody = GetComponent<Rigidbody>();
    }

    protected void Start()
    {
        CurrentHP = _baseHP;
    }

    protected void FixedUpdate()
    {
        _rigidbody.velocity = Vector3.zero;
    }

    protected void Update()
    {
        if (_currentEquippedWeapon != null)
        {
            ProcessShoot();
        }

        ProcessTransform();
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
        weapon.transform.forward = _weaponParent.forward;
        CurrentEquippedWeapon = weapon;
    }

    public void EquipBaseWeapon(float delay = 0.5f)
    {
        BaseWeapon weapon = GameController.Instance.CreateWeapon(_startWeapon, _weaponParent);
        StartCoroutine(WaitToEquipBaseWeapon(weapon, delay));
    }

    public void TakeDamage(float damage, Vector3 hitPosition)
    {
        GameController.Instance.SpawnHitParticlesAtPosition(HitType.Flesh, hitPosition);
        CurrentHP -= damage;
    }

    protected void ProcessTransform()
    {
        Vector3 translation = InputManager.GetMovementDirection() * _speed * Time.fixedDeltaTime;
        transform.position += translation;

        Vector3 forward = _crosshair.position - transform.position;
        forward.y = 0.0f;
        if(forward.magnitude > 0.001f)
        {
            transform.forward = forward.normalized;
        }

        float effectiveSpeed = translation.magnitude;
        _animator.SetFloat(SpeedName, effectiveSpeed);
        if (effectiveSpeed > 0.0f)
        {
            //Calculate angle between translation and forward
            float directionAngle = Vector3.Angle(translation, transform.forward);
            //Give it a sign (from [0, 180] to [-180, 180]) based on cross product y (y is always up)
            directionAngle *= Mathf.Sign(Vector3.Cross(translation, transform.forward).y);
            //Translate from [-180, 180] to [0, 1]
            directionAngle = (directionAngle + 180.0f) * onePer360;
            _animator.SetFloat(DirectionName, directionAngle);
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
            EquipBaseWeapon(0.0f);
        }
    }

    protected IEnumerator WaitToEquipBaseWeapon(BaseWeapon weapon, float delay = 0.5f)
    {
        yield return new WaitForSecondsRealtime(delay);
        EquipWeapon(weapon);
    }

    #endregion
}
