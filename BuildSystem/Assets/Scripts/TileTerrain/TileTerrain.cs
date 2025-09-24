using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileTerrain : MonoBehaviour
{
    [LabelText("µÿøÈ≈‰÷√")]
    [OnValueChanged("OnConfigOrDataChanged")]
    public TileTerrainTileConfig tileConfig;

    [LabelText("µÿ–Œ≈‰÷√")]
    [OnValueChanged("OnConfigOrDataChanged")]
    public TileTerrainData terrainData;

#if UNITY_EDITOR
    private Action onConfiggOrDataChangeAction;
    public void SetOnTileConfigChangeAction(Action action)
    {
        onConfiggOrDataChangeAction = action;
    }
    public void OnConfigOrDataChanged()
    {
        onConfiggOrDataChangeAction?.Invoke();
    }
#endif
}
