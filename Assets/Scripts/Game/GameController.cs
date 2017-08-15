using System;
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

public enum HitType
{
    Flesh,
    Metal
}

[Serializable]
public struct HitParticleInfo
{
    public HitType Type;
    public ParticleSystem ParticlesPrefab;
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
    protected List<HitParticleInfo> _hitParticles;
    [SerializeField]
    protected List<BaseWeapon> _weapons;

    protected Dictionary<WeaponType, ObjectPool<BaseWeapon>> _weaponsPools;
    protected Dictionary<HitType, ObjectPool<ParticleSystem>> _hitParticlesPools;

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

        _hitParticlesPools = new Dictionary<HitType, ObjectPool<ParticleSystem>>();
        int particlesCount = _hitParticles.Count;
        for (int i = 0; i < particlesCount; ++i)
        {
            _hitParticlesPools.Add(_hitParticles[i].Type, new ObjectPool<ParticleSystem>());
            _hitParticlesPools[_hitParticles[i].Type].Init(_hitParticles[i].ParticlesPrefab, transform);
        }

        InputManager.Init();
    }

    protected void Start()
    {
        GameState.Init();
        CurrentPhase = GamePhase.StartScreen;
        OnGamePhaseChanged(GamePhase.StartScreen);
    }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        foreach (WeaponType weaponType in _weaponsPools.Keys)
        {
            _weaponsPools[weaponType].CollectInactiveObjects();
        }

        foreach (HitType hitType in _hitParticlesPools.Keys)
        {
            _hitParticlesPools[hitType].CollectInactiveObjects((particleObject) => { return particleObject != null && !particleObject.isPlaying; });
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

    public void SpawnHitParticlesAtPosition(HitType hitType, Vector3 position)
    {
        ParticleSystem particles = _hitParticlesPools[hitType].GetObject(null);
        particles.transform.position = position;
        particles.Play();
    }

#endregion
}
