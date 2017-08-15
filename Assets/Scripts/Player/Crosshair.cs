using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    #region Variables

    [SerializeField]
    protected Transform _target;
    [SerializeField]
    protected float _gamepadOffset;

    protected Vector3 _offset;
    protected Vector3 _targetCursorOffset;

    #endregion

    #region Unity Methods

    protected void Awake()
    {
        Cursor.visible = false;
        GameController.Instance.OnGamePhaseChanged += OnGamePhaseChangedCallback;
    }

    protected void LateUpdate()
    {
        Vector3 gamepadInput = InputManager.GetCrosshairMovement();
        if(gamepadInput.magnitude > 0.1f)
        {
            _targetCursorOffset = gamepadInput.normalized * _gamepadOffset;

            _offset = Vector3.Lerp(_offset, _targetCursorOffset, 0.3f);
        }
        else
        {
            if (InputManager.GetMouseCrosshairMovement().magnitude > 0)
            {
                Vector3 currentWorldPosition = GetGroundPosition(Input.mousePosition);
                _targetCursorOffset = currentWorldPosition - _target.position;

                _offset = _targetCursorOffset;
            }
        }

        transform.position = _target.position + _offset;
    }

    #endregion

    #region Methods

    protected void OnGamePhaseChangedCallback(GamePhase gamePhase)
    {
        enabled = gamePhase == GamePhase.Game;
        if(enabled)
        {
            _offset = Vector3.forward * _gamepadOffset;
            _targetCursorOffset = _offset;
        }
    }

    protected Vector3 GetGroundPosition(Vector3 screenPosition)
    {
        screenPosition.z = 1.0f;
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, float.MaxValue, 1 << LayerManager.Ground, QueryTriggerInteraction.Ignore))
        {
            return hit.point;
        }

        return Vector3.zero;
    }

    #endregion
}
