using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using System;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "TileConfig",menuName = "TileTerrain/TileConfig")]
public class TileTerrainTileConfig : ConfigBase
{
    [ListDrawerSettings(ShowIndexLabels = true, ShowPaging = false)]
    [OnValueChanged("OnTileConfigChanged")]
    public List<TileTerrainConfigItem> tileConfigList = new List<TileTerrainConfigItem>();

#if UNITY_EDITOR
    private Action onTileConfigChangedAction;
    public void SetConfigChangedAction(Action onTileConfigChangedAction)
    {
        this.onTileConfigChangedAction = onTileConfigChangedAction;
    }

    public bool isSetConfigChangeAction()
    { 
        return onTileConfigChangedAction == null;
    }

    public void ClearAction()
    {
        onTileConfigChangedAction = null;
    }
    private void OnTileConfigChanged()
    {
        onTileConfigChangedAction?.Invoke();
    }
#endif
}

[Serializable]
public class TileTerrainConfigItem
{
#if UNITY_EDITOR
    public string name;
#endif
    public GameObject forwardPrefab;
    public GameObject backPrefab;
    public GameObject leftPrefab;
    public GameObject rightPrefab;
    public GameObject topPrefab;
}
