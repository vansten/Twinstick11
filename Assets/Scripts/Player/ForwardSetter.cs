using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardSetter : MonoBehaviour
{
    #region Unity Methods

    protected void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(0.0f, transform.parent.rotation.eulerAngles.y, 0.0f);
    }

    #endregion  
}
