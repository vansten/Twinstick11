using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    #region Variables

    [SerializeField]
    protected float _offset;

    #endregion

    #region Unity Methods

    protected void OnValidate()
    {
        LateUpdate();
    }

    protected void Awake()
    {
        Cursor.visible = false;
    }

    protected void LateUpdate()
    {
        transform.up = Vector3.up;
        transform.position = transform.parent.position + transform.parent.forward * _offset;
    }

    #endregion  
}
