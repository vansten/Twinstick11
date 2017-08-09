using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables

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
            if(_currentHP != value)
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

        if(Input.GetKeyDown(KeyCode.K))
        {
            CurrentHP -= 10.0f;
        }
    }

    #endregion

    #region Methods

    protected void ProcessMovement()
    {
        transform.position += InputManager.GetMovementDirection() * Speed * Time.deltaTime;
        transform.forward = (_crosshair.position - transform.position).normalized;
    }

    protected void OnGamePhaseChangedCallback(GamePhase gamePhase)
    {
        enabled = gamePhase == GamePhase.Game;
        if(enabled)
        {
            CurrentHP = BaseHP;
        }
    }

    #endregion
}
