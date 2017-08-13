using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardSetter : MonoBehaviour
{
    #region Unity Methods

    protected void LateUpdate()
    {
        Vector3 forward = transform.forward;
        forward.y = 0.0f;
        transform.forward = forward.normalized;
    }

    #endregion  
}
