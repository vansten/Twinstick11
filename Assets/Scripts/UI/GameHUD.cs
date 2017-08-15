using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WeaponInfo
{
    public WeaponType Type;
    public Sprite Sprite;
}

public class GameHUD : MonoBehaviour
{
    #region Variables

    [SerializeField]
    protected UnityEngine.UI.Image _hpBar;
    [SerializeField]
    protected UnityEngine.UI.Image _currentWeaponImage;
    [SerializeField]
    protected UnityEngine.UI.Text _currentWaveText;
    [SerializeField]
    protected UnityEngine.UI.Text _enemiesLeftText;
    [SerializeField]
    protected UnityEngine.UI.Image _readyBar;

    [SerializeField]
    protected List<WeaponInfo> _weaponsInfo;

    protected bool _isAppQuiting;

    #endregion

    #region Unity Methods

    protected void OnEnable()
    {
        GameController.Instance.GameState.OnCurrentWaveChanged += OnWaveChangedCallback;
        GameController.Instance.GameState.OnEnemiesLeftChanged += OnEnemiesLeftChangedCallback;
        GameController.Instance.Player.OnHPChanged += OnHPChangedCallback;
        GameController.Instance.Player.OnWeaponChanged += OnWeaponChangedCallback;

        OnEnemiesLeftChangedCallback(GameController.Instance.GameState.EnemiesLeft);
        OnWaveChangedCallback(GameController.Instance.GameState.CurrentWave);
        _hpBar.fillAmount = 1.0f;
    }

    protected void OnDisable()
    {
        if(_isAppQuiting)
        {
            return;
        }

        GameController.Instance.GameState.OnCurrentWaveChanged -= OnWaveChangedCallback;
        GameController.Instance.GameState.OnEnemiesLeftChanged -= OnEnemiesLeftChangedCallback;
        GameController.Instance.Player.OnHPChanged -= OnHPChangedCallback;
        GameController.Instance.Player.OnWeaponChanged -= OnWeaponChangedCallback;
    }

    private void OnApplicationQuit()
    {
        _isAppQuiting = true;
    }

    protected void Update()
    {
        BaseWeapon weapon = GameController.Instance.Player.CurrentEquippedWeapon;
        if(weapon != null)
        {
            _readyBar.fillAmount = weapon.GetReadyPercent();
        }
        else
        {
            _readyBar.fillAmount = 0;
        }
    }

    #endregion

    #region Methods

    protected void OnEnemiesLeftChangedCallback(uint enemiesLeft)
    {
        _enemiesLeftText.text = enemiesLeft.ToString("00");
    }

    protected void OnWaveChangedCallback(uint currentWave)
    {
        _currentWaveText.text = currentWave.ToString();
    }

    protected void OnHPChangedCallback(float hp, float percent)
    {
        _hpBar.fillAmount = percent;
    }

    protected void OnWeaponChangedCallback(WeaponType weaponType)
    {
        foreach(WeaponInfo weaponInfo in _weaponsInfo)
        {
            if(weaponInfo.Type == weaponType)
            {
                _currentWeaponImage.sprite = weaponInfo.Sprite;
                return;
            }
        }
    }

    #endregion
}
