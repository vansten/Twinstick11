using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    #region Variables
    
    [SerializeField]
    protected float _maxOffset;
    [SerializeField]
    protected float _mouseSpeed;
    [SerializeField]
    protected float _gamepadSpeed;

    protected Vector3 _offset;

    #endregion

    #region Unity Methods

    protected void Awake()
    {
        Cursor.visible = false;
        GameController.Instance.OnGamePhaseChanged += OnGamePhaseChangedCallback;
    }

    protected void LateUpdate()
    {
        _offset += (InputManager.GetCrosshairMovement() * _gamepadSpeed + InputManager.GetMouseCrosshairMovement() * _mouseSpeed) * Time.deltaTime;
        _offset = Vector3.ClampMagnitude(_offset, _maxOffset);
        transform.position = GameController.Instance.Player.transform.position + _offset;
    }

    #endregion

    #region Methods

    protected void OnGamePhaseChangedCallback(GamePhase gamePhase)
    {
        enabled = gamePhase == GamePhase.Game;
        if(enabled)
        {
            _offset = Vector3.forward;
        }
    }

    #endregion
}
