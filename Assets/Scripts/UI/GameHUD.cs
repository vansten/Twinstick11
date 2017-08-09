using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    #endregion

    #region Unity Methods

    protected void OnEnable()
    {
        GameController.Instance.GameState.OnCurrentWaveChanged += OnWaveChangedCallback;
        GameController.Instance.GameState.OnEnemiesLeftChanged += OnEnemiesLeftChangedCallback;
        GameController.Instance.Player.OnHPChanged += OnHPChangedCallback;

        OnEnemiesLeftChangedCallback(GameController.Instance.GameState.EnemiesLeft);
        OnWaveChangedCallback(GameController.Instance.GameState.CurrentWave);
        _hpBar.fillAmount = 1.0f;
    }

    protected void OnDisable()
    {
        GameController.Instance.GameState.OnCurrentWaveChanged -= OnWaveChangedCallback;
        GameController.Instance.GameState.OnEnemiesLeftChanged -= OnEnemiesLeftChangedCallback;
        GameController.Instance.Player.OnHPChanged -= OnHPChangedCallback;
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

    #endregion
}
