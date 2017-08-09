using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GamePhaseUIObjectInfo
{
    public GamePhase ActivePhase;
    public GameObject UIObject;
}

public class UIController : MonoBehaviour
{
    #region Variables

    [SerializeField]
    protected GamePhaseUIObjectInfo[] _uiObjectsInfos;

    #endregion

    #region Unity Methods
    
    protected void Awake()
    {
        GameController.Instance.OnGamePhaseChanged += OnGamePhaseChangedCallback;
    }

    #endregion

    #region Methods
    
    protected void OnGamePhaseChangedCallback(GamePhase gamePhase)
    {
        int length = _uiObjectsInfos.Length;
        for(int i = 0; i < length; ++i)
        {
            if(_uiObjectsInfos[i].UIObject != null)
            {
                _uiObjectsInfos[i].UIObject.SetActive(_uiObjectsInfos[i].ActivePhase == gamePhase);
            }
        }
    }

    #endregion
}
