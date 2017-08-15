using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardSetter : MonoBehaviour
{
    #region Variables

    [SerializeField]
    protected Transform _crosshair;

    #endregion

    #region Unity Methods

    protected void LateUpdate()
    {
        Vector3 forward = _crosshair.position - transform.position;
        forward.y = 0.0f;
        if (forward.magnitude > 0.001f)
        {
            transform.forward = forward.normalized;
        }
    }

    #endregion  
}
