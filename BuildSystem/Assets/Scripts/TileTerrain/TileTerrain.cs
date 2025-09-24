using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileTerrain : MonoBehaviour
{
    [LabelText("�ؿ�����")]
    [OnValueChanged("OnConfigOrDataChanged")]
    public TileTerrainTileConfig tileConfig;

    [LabelText("��������")]
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
