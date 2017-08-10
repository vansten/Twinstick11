using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables

    public Transform WeaponParent;
    public float Speed;
    public float BaseHP;

    [SerializeField]
    protected Transform _crosshair;

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

                _currentHP = Mathf.Clamp(_currentHP, 0.0f, BaseHP);
                if (OnHPChanged != null)
                {
                    OnHPChanged(_currentHP, _currentHP / BaseHP);
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
        if(BaseHP <= 0.0f)
        {
            BaseHP = 0.01f;
        }
    }

    protected void Awake()
    {
        GameController.Instance.OnGamePhaseChanged += OnGamePhaseChangedCallback;
    }

    protected void Start()
    {
        CurrentHP = BaseHP;
    }

    protected void Update()
    {
        ProcessMovement();

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
            Debug.LogWarning("Change this!");
            Destroy(CurrentEquippedWeapon.gameObject);
        }
        weapon.transform.parent = WeaponParent;
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        CurrentEquippedWeapon = weapon;
    }

    protected void ProcessMovement()
    {
        transform.position += InputManager.GetMovementDirection() * Speed * Time.deltaTime;
        transform.forward = (_crosshair.position - transform.position).normalized;
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
            CurrentHP = BaseHP;
            if (CurrentEquippedWeapon == null || CurrentEquippedWeapon.Type != WeaponType.Pistol)
            {
                if (CurrentEquippedWeapon != null)
                {
                    Debug.LogWarning("Change this!");
                    Destroy(CurrentEquippedWeapon.gameObject);
                }
                CurrentEquippedWeapon = GameController.Instance.CreateWeapon(WeaponType.Pistol, WeaponParent);
            }
        }
    }

    #endregion
}
