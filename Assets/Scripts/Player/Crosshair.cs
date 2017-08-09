using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    #region Variables

    public float MaxOffset;

    [SerializeField]
    protected Transform _playerTransform;

    protected Vector3 _direction;

    #endregion

    #region Unity Methods

    protected void Awake()
    {
        GameController.Instance.OnGamePhaseChanged += OnGamePhaseChangedCallback;
    }

    protected void Start()
    {
        _direction = Vector3.forward * MaxOffset;
    }

    protected void LateUpdate()
    {
        ProcessPosition();
    }

    #endregion

    #region Methods

    protected void ProcessPosition()
    {
        Vector3 crosshairMovement = InputManager.GetCrosshairMovement();
        if (crosshairMovement.magnitude > 0.0f)
        {
            _direction = crosshairMovement * MaxOffset;
        }
        transform.position = _playerTransform.position + _direction;
    }
    
    protected void OnGamePhaseChangedCallback(GamePhase gamePhase)
    {
        enabled = gamePhase == GamePhase.Game;
    }

    #endregion
}
