using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootToStart : MonoBehaviour
{
    #region Unity Methods

    protected void Update()
    {
        if(InputManager.IsShooting())
        {
            GameController.Instance.CurrentPhase = GamePhase.Game;
        }
    }

    #endregion
}
