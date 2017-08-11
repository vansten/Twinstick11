using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Variables

    [SerializeField]
    protected float _angle;
    [SerializeField]
    protected float _offset;
    [Range(0.0f, 1.0f)]
    [SerializeField]
    protected float _cameraSpeed;

    [SerializeField]
    protected Transform _target;

    #endregion

    #region Unity Methods

    protected void OnValidate()
    {
        transform.forward = (Quaternion.Euler(-_angle, 0.0f, 0.0f) * Vector3.down).normalized;
        if (_target != null)
        {
            transform.position = _target.position + _offset * -transform.forward;
        }
    }

    protected void Start()
    {
        OnValidate();
    }

    protected void LateUpdate()
    {
        Vector3 targetPosition = _target.position + _offset * -transform.forward;
        transform.position = Vector3.Slerp(transform.position, targetPosition, _cameraSpeed);
    }

    #endregion
}
