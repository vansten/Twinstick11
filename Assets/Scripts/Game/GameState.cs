﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState
{
    #region Variables

    public uint FirstWaveEnemiesCount;
    public uint EnemiesCountChangeRate;

    #endregion

    #region Properties

    protected uint _currentWave;
    public delegate void CurrentWaveChangedDelgate(uint currentWave);
    public event CurrentWaveChangedDelgate OnCurrentWaveChanged;
    public uint CurrentWave
    {
        get
        {
            return _currentWave;
        }
        set
        {
            if (_currentWave != value)
            {
                _currentWave = value;
                if(OnCurrentWaveChanged != null)
                {
                    OnCurrentWaveChanged(_currentWave);
                }

                EnemiesLeft = FirstWaveEnemiesCount + (_currentWave - 1) * EnemiesCountChangeRate;
            }
        }
    }

    protected uint _enemiesLeft;
    public delegate void EnemiesLeftChangedDelgate(uint enemiesLeft);
    public event EnemiesLeftChangedDelgate OnEnemiesLeftChanged;
    public uint EnemiesLeft
    {
        get
        {
            return _enemiesLeft;
        }
        set
        {
            if (_enemiesLeft != value)
            {
                _enemiesLeft = value;
                if(OnEnemiesLeftChanged != null)
                {
                    OnEnemiesLeftChanged(_enemiesLeft);
                }

                if(_enemiesLeft == 0)
                {
                    CurrentWave += 1;
                }
            }
        }
    }

    #endregion

    #region Methods

    public void Init()
    {
        GameController.Instance.OnGamePhaseChanged += OnGamePhaseChangedCallback;
    }

    protected void OnGamePhaseChangedCallback(GamePhase gamePhase)
    {
        if(gamePhase == GamePhase.Game)
        {
            CurrentWave = 1;
            EnemiesLeft = FirstWaveEnemiesCount;
        }
    }

    #endregion
}
