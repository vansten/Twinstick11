using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Variables

    public float Angle;
    public float Offset;
    [Range(0.0f, 1.0f)]
    public float CameraSpeed;

    [SerializeField]
    protected Transform _target;

    #endregion

    #region Unity Methods

    protected void OnValidate()
    {
        transform.forward = (Quaternion.Euler(-Angle, 0.0f, 0.0f) * Vector3.down).normalized;
        if (_target != null)
        {
            transform.position = _target.position + Offset * -transform.forward;
        }
    }

    protected void Start()
    {
        OnValidate();
    }

    protected void LateUpdate()
    {
        Vector3 targetPosition = _target.position + Offset * -transform.forward;
        transform.position = Vector3.Slerp(transform.position, targetPosition, CameraSpeed);
    }

    #endregion
}
