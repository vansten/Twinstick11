using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackPoint : MonoBehaviour
{
    #region Variables

    protected MeleeEnemy _enemy;

    #endregion

    #region Unity Methods

    protected void OnTriggerEnter(Collider other)
    {
        if(!enabled)
        {
            return;
        }

        if (other.gameObject.layer == LayerManager.Player)
        {
            if (_enemy != null)
            {
                _enemy.NotifyAttackPlayer(other.gameObject.GetComponent<PlayerController>(), other.transform.position);
            }
        }
    }

    #endregion

    #region Methods

    public void SetEnemy(MeleeEnemy enemy)
    {
        _enemy = enemy;
    }

    #endregion
}
