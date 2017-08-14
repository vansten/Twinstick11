using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    #region Consts and statics

    protected static Vector3 _lastMousePosition;

    #endregion

    #region Methods

    public static bool IsShooting()
    {
        return Input.GetButton("Fire") || Input.GetAxis("FireAxis") > 0.0f;
    }

    public static Vector3 GetMovementDirection()
    {
        return new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical")).normalized;
    }

    public static Vector3 GetForwardDirection()
    {
        return new Vector3(Input.GetAxis("RotationX"), 0.0f, Input.GetAxis("RotationY")).normalized;
    }

    public static Vector3 GetMouseForward(Vector3 objectWorldPosition)
    {
        Vector3 objectScreenPosition = Camera.main.WorldToScreenPoint(objectWorldPosition);
        Vector3 currentMousePosition = Input.mousePosition;
        Vector3 difference = currentMousePosition - objectScreenPosition;
        if((currentMousePosition - _lastMousePosition).magnitude <= 1)
        {
            difference = Vector3.zero;
        }
        difference.z = difference.y;
        difference.y = 0.0f;
        _lastMousePosition = Input.mousePosition;
        return difference.normalized;
    }

    #endregion
}
