using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheatingControl : MonoBehaviour
{
    #region Variables

    [SerializeField]
    protected MeshRenderer[] _meshes;
    [SerializeField]
    protected Color _overheatedColor;

    protected BaseWeapon _weapon;
    protected Color _initColor;

    #endregion

    #region Unity Methods

    protected void Awake()
    {
        if(_meshes.Length > 0)
        {
            _initColor = _meshes[0].material.color;
        }

        _weapon = GetComponent<BaseWeapon>();
    }

    protected void Update()
    {
        SetColor(1.0f - _weapon.GetReadyPercent());
    }

    #endregion

    #region Methods

    protected void SetColor(float t)
    {
        Color c = Color.Lerp(_initColor, _overheatedColor, t);
        foreach(MeshRenderer mr in _meshes)
        {
            mr.material.color = c;
        }
    }

    [ContextMenu("Set renderers")]
    protected void SetRenderers()
    {
        _meshes = GetComponentsInChildren<MeshRenderer>();
    }

    #endregion
}
