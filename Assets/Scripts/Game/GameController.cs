﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GamePhase
{
    StartScreen,
    Game,
    DeathScreen,

    Count
}

public class GameController : MonoBehaviour
{
    #region Singleton

    private static GameController _instance;

    public static GameController Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<GameController>();
                if(_instance == null)
                {
                    GameObject go = new GameObject("GameController_DummyInstance");
                    _instance = go.AddComponent<GameController>();
                }
            }

            return _instance;
        }
    }

    #endregion

    #region Variables

    public PlayerController Player;
    public EnemySpawner EnemySpawner;
    public GameState GameState;

    [SerializeField]
    protected List<BaseWeapon> _weapons;

    protected Dictionary<WeaponType, ObjectPool<BaseWeapon>> _weaponsPools;

    #endregion

    #region Properties

    protected GamePhase _currentPhase;
    public delegate void GamePhaseChangedDelegate(GamePhase gamePhase);
    public event GamePhaseChangedDelegate OnGamePhaseChanged;
    public GamePhase CurrentPhase
    {
        get
        {
            return _currentPhase;
        }
        set
        {
            if(_currentPhase != value)
            {
                _currentPhase = value;
                if(OnGamePhaseChanged != null)
                {
                    OnGamePhaseChanged(_currentPhase);
                }
            }
        }
    }

    #endregion

    #region Unity methods

    protected void Awake()
    {
        _weaponsPools = new Dictionary<WeaponType, ObjectPool<BaseWeapon>>();
        int weaponsCount = _weapons.Count;
        for(int i = 0; i < weaponsCount; ++i)
        {
            _weaponsPools.Add(_weapons[i].Type, new ObjectPool<BaseWeapon>());
            _weaponsPools[_weapons[i].Type].Init(_weapons[i], transform, GrowthStrategy.DoubleSize, 8);
        }
    }

    protected void Start()
    {
        GameState.Init();
        CurrentPhase = GamePhase.StartScreen;
        OnGamePhaseChanged(GamePhase.StartScreen);
    }

    protected void Update()
    {
        foreach(WeaponType weaponType in _weaponsPools.Keys)
        {
            _weaponsPools[weaponType].CollectInactiveObjects();
        }
    }

    #endregion

    #region Methods

    public BaseWeapon CreateWeapon(Transform transform)
    {
        WeaponType weaponType = (WeaponType)UnityEngine.Random.Range(0, (int)WeaponType.Count);
        while(weaponType == WeaponType.None || weaponType == WeaponType.Count)
        {
            weaponType = (WeaponType)UnityEngine.Random.Range(0, (int)WeaponType.Count);
        }
        return CreateWeapon(weaponType, transform);
    }

    public BaseWeapon CreateWeapon(WeaponType weaponType, Transform transform)
    {
        if(weaponType == WeaponType.None || weaponType == WeaponType.Count)
        {
            return null;
        }

        foreach(BaseWeapon weapon in _weapons)
        {
            if(weapon.Type == weaponType)
            {
                return _weaponsPools[weaponType].GetObject(transform);
            }
        }

        return null;
    }

    #endregion
}
