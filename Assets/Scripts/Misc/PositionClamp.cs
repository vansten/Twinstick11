using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionClamp : MonoBehaviour
{
    #region Variables

    [SerializeField]
    protected Transform _min;
    [SerializeField]
    protected Transform _max;

    #endregion

    #region Unity Methods

    protected void LateUpdate()
    {
        Vector3 position = transform.position;
        position = new Vector3(Mathf.Clamp(position.x, _min.position.x, _max.position.x),
                                Mathf.Clamp(position.y, _min.position.y, _max.position.y),
                                Mathf.Clamp(position.z, _min.position.z, _max.position.z));
        transform.position = position;
    }

    #endregion
}
