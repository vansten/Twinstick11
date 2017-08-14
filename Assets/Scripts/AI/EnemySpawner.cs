using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnInfo
{
    public BaseEnemy EnemyPrefab;
    [Range(0.0f, 1.0f)]
    public float SpawnChance;
    [HideInInspector]
    public ObjectPool<BaseEnemy> EnemyPool;

    public void Init(Transform parent)
    {
        EnemyPool = new ObjectPool<BaseEnemy>();
        EnemyPool.Init(EnemyPrefab, parent);
    }

    public BaseEnemy GetEnemy(Transform transform)
    {
        return EnemyPool.GetObject(transform);
    }
}

public class EnemySpawner : MonoBehaviour
{
    #region Variables

    [SerializeField]
    protected List<EnemySpawnInfo> _enemiesPrefabs;
    [SerializeField]
    protected Transform _minSpawnTransform;
    [SerializeField]
    protected Transform _maxSpawnTransform;
    [SerializeField]
    protected float _spawnDelay;

    protected float _spawnTimer;
    protected uint _enemiesToSpawn;
    
    #endregion

    #region Unity Methods

    protected void OnValidate()
    {
        int enemiesCount = _enemiesPrefabs.Count;
        float sum = 0.0f;
        for(int i = 0; i < enemiesCount; ++i)
        {
            float chance = _enemiesPrefabs[i].SpawnChance;
            if (sum + chance > 1.0f)
            {
                _enemiesPrefabs[i].SpawnChance = 1.0f - sum;
            }
            sum += _enemiesPrefabs[i].SpawnChance;
        }
    }

    protected void Awake()
    {
        foreach(EnemySpawnInfo enemySpawnInfo in _enemiesPrefabs)
        {
            enemySpawnInfo.Init(transform);
        }

        GameController.Instance.OnGamePhaseChanged += OnGamePhaseChangedCallback;
        GameController.Instance.GameState.OnCurrentWaveChanged += OnWaveChangedCallback;
    }

    protected void Update()
    {
        ManagePools();
        ProcessSpawn();
    }

    #endregion

    #region Methods

    public void EnemyKilled(BaseEnemy enemy)
    {
        enemy.gameObject.SetActive(false);
        GameController.Instance.GameState.EnemiesLeft -= 1;
    }

    protected void SpawnEnemy()
    {
        float random = Random.Range(0.0f, 1.0f);
        float sum = 0.0f;
        foreach(EnemySpawnInfo enemySpawnInfo in _enemiesPrefabs)
        {
            if(random <= enemySpawnInfo.SpawnChance + sum)
            {
                BaseEnemy newEnemy = enemySpawnInfo.GetEnemy(transform);
                newEnemy.transform.position = GetRandomPosition();
                --_enemiesToSpawn;
                return;
            }
            sum += enemySpawnInfo.SpawnChance;
        }
    }

    protected Vector3 GetRandomPosition()
    {
        bool constX = Random.Range(0, 1000000) % 2 == 0;
        bool useMin = Random.Range(0, 1000000) % 2 == 0;

        Vector3 finalPosition = new Vector3();
        if(constX)
        {
            finalPosition.x = useMin ? _minSpawnTransform.position.x : _maxSpawnTransform.position.x;
            finalPosition.z = Random.Range(_minSpawnTransform.position.z, _maxSpawnTransform.position.z);
        }
        else
        {
            finalPosition.x = Random.Range(_minSpawnTransform.position.x, _maxSpawnTransform.position.x);
            finalPosition.z = useMin ? _minSpawnTransform.position.z : _maxSpawnTransform.position.z;
        }

        return finalPosition;
    }

    protected void ManagePools()
    {
        foreach (EnemySpawnInfo enemySpawnInfo in _enemiesPrefabs)
        {
            enemySpawnInfo.EnemyPool.CollectInactiveObjects();
        }
    }

    protected void ProcessSpawn()
    {
        if(_enemiesToSpawn == 0)
        {
            return;
        }
        _spawnTimer += Time.deltaTime;
        if(_spawnTimer >= _spawnDelay)
        {
            _spawnTimer = 0.0f;
            SpawnEnemy();
        }
    }

    protected void OnGamePhaseChangedCallback(GamePhase gamePhase)
    {
        enabled = gamePhase == GamePhase.Game;
        if(!enabled)
        {
            foreach (EnemySpawnInfo enemySpawnInfo in _enemiesPrefabs)
            {
                enemySpawnInfo.EnemyPool.DisableAllObjects();
            }
        }
    }

    protected void OnWaveChangedCallback(uint currentWave)
    {
        if(!enabled)
        {
            return;
        }

        _enemiesToSpawn = GameController.Instance.GameState.EnemiesLeft;
    }

    #endregion
}
