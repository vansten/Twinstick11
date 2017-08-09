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

    public GameState GameState;

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
    }

    protected void Start()
    {
        GameState.Init();
        CurrentPhase = GamePhase.StartScreen;
        OnGamePhaseChanged(GamePhase.StartScreen);
    }

    protected void Update()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            GameState.EnemiesLeft -= 1;
        }
    }

    #endregion

    #region Methods

    #endregion
}
