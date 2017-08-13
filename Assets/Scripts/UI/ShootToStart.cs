using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootToStart : MonoBehaviour
{
    #region Variables

    bool _wasShootingAtStart;

    #endregion

    #region Unity Methods

    protected void OnEnable()
    {
        _wasShootingAtStart = InputManager.IsShooting();
    }

    protected void Update()
    {
        if(_wasShootingAtStart)
        {
            _wasShootingAtStart = InputManager.IsShooting();
        }

        if(InputManager.IsShooting() && !_wasShootingAtStart)
        {
            GameController.Instance.CurrentPhase = GamePhase.Game;
        }
    }

    #endregion
}
