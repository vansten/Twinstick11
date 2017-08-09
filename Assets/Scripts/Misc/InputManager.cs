using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    public static bool IsShooting()
    {
        return Input.GetButton("Fire") || Input.GetAxis("FireAxis") > 0.0f;
    }

    public static Vector3 GetMovementDirection()
    {
        return new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical")).normalized;
    }

    public static Vector3 GetCrosshairMovement()
    {
        return new Vector3(Input.GetAxis("RotationX"), 0.0f, Input.GetAxis("RotationY")).normalized;
    }
}
