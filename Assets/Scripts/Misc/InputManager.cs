using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputManager
{
    #region Consts and statics

    public static Vector3 LastMousePosition;

    protected static float _deadzoneX = 0.2f;
    protected static float _deadzoneZ = 0.2f;
    protected static float _oneMinusDeadzoneX = 1.0f - _deadzoneX;
    protected static float _oneMinusDeadzoneZ = 1.0f - _deadzoneZ;

    #endregion

    #region Methods

    public static void Init()
    {
        LastMousePosition = Input.mousePosition;
    }

    public static bool IsShooting()
    {
        return Input.GetButton("Fire");// || Input.GetAxis("FireAxis") > 0.0f;
    }

    public static Vector3 GetMovementDirection()
    {
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        //Applying deadzones
        direction.x = Mathf.Sign(direction.x) * Mathf.Clamp01((Mathf.Abs(direction.x) - _deadzoneX) / _oneMinusDeadzoneX);
        direction.z = Mathf.Sign(direction.z) * Mathf.Clamp01((Mathf.Abs(direction.z) - _deadzoneZ) / _oneMinusDeadzoneZ);
        return Vector3.ClampMagnitude(direction, 1.0f);
    }

    public static Vector3 GetCrosshairMovement()
    {
        Vector3 direction = new Vector3(Input.GetAxis("RotationX"), 0.0f, Input.GetAxis("RotationY"));
        //Applying deadzones
        direction.x = Mathf.Sign(direction.x) * Mathf.Clamp01((Mathf.Abs(direction.x) - _deadzoneX) / _oneMinusDeadzoneX);
        direction.z = Mathf.Sign(direction.z) * Mathf.Clamp01((Mathf.Abs(direction.z) - _deadzoneZ) / _oneMinusDeadzoneZ);
        return Vector3.ClampMagnitude(direction, 1.0f);
    }

    public static Vector3 GetMouseCrosshairMovement()
    {
        Vector3 currentMousePosition = Input.mousePosition;
        Vector3 difference = currentMousePosition - LastMousePosition;
        LastMousePosition = Input.mousePosition;
        return difference;
    }

    #endregion
}
