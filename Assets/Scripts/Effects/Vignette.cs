using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Vignette : MonoBehaviour
{
    #region Consts and statics

    protected static string _powerName = "_Power";
    protected static string _colorName = "_Color";

    #endregion

    #region Variables

    [SerializeField]
    protected Color _vignetteColor;
    [SerializeField]
    [Range(0.0f, 10.0f)]
    protected float _vignettePower = 5.0f;
    [SerializeField]
    protected Shader _shader;

    protected Material _currentMaterial;

    #endregion

    #region Properties

    private Material material
    {
        get
        {
            if (_currentMaterial == null)
            {
                _currentMaterial = new Material(_shader);
                _currentMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return _currentMaterial;
        }
    }

    #endregion

    #region Unity Methods

    protected void Awake()
    {
        GameController.Instance.Player.OnHPChanged += OnHPChangedCallback;
    }

    protected void Start()
    {
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }
    }

    protected void OnDisable()
    {
        if (_currentMaterial)
        {
            DestroyImmediate(_currentMaterial);
        }
    }

    protected void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_shader == null)
        {
            Graphics.Blit(source, destination);
        }
        else
        {
            material.SetFloat(_powerName, _vignettePower);
            material.SetColor(_colorName, _vignetteColor);
            Graphics.Blit(source, destination, material);
        }
    }

    #endregion

    #region Methods
    
    private void OnHPChangedCallback(float hp, float percent)
    {
        _vignettePower = Mathf.Clamp((1.0f - percent) * 12.0f, 0, 10);
    }

    #endregion
}
