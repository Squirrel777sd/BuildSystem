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

    private TileTerrainCellData[,,] cellData
    {
        get 
        {
            return terrainData.cellDatas;
        }
    }
    private TileCell[,,] cells;

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

    public void CleanCellsForEditor()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    public void CreateAllCellGameObjectForEditor()
    {
        CleanCellsForEditor();
    }
#endif


    public void CreateAllCell()
    {
        if (tileConfig != null && terrainData != null && cellData != null)
        {
            cells = new TileCell[terrainData.mapSize.x, terrainData.mapSize.y, terrainData.mapSize.z];
            //TODO ��������ĸ����� ������� ���ǲ����ᴴ����Ϸ����
            // ����ֱ�ӻ��ƣ���Ϊ����ʱ�����ػ���ȫ������
        }
    }


    public TileCell GetCell(int x,int y,int z)
    {
        if (x < 0 || x > terrainData.mapSize.x || y < 0 || y > terrainData.mapSize.y || z < 0 || z > terrainData.mapSize.z)
        {
            return null;
        }
        return cells[x, y, z];
    }

    public TileCell GetCell(Vector3Int coord)
    {
        return GetCell(coord.x, coord.y, coord.z);
    }

    public TileCell GetForwordCell(Vector3Int coord)
    {
        return GetCell(coord.x, coord.y, coord.z + 1);
    }

    public TileCell GetBackCell(Vector3Int coord)
    {
        return GetCell(coord.x, coord.y, coord.z - 1);
    }


    public TileCell GetLeftCell(Vector3Int coord)
    {
        return GetCell(coord.x - 1, coord.y, coord.z);
    }

    public TileCell GetRightCell(Vector3Int coord)
    {
        return GetCell(coord.x + 1, coord.y, coord.z + 1);
    }

    public TileCell GetTopCell(Vector3Int coord)
    {
        return GetCell(coord.x, coord.y + 1, coord.z + 1);
    }

    public TileCell GetBottomCell(Vector3Int coord)
    {
        return GetCell(coord.x, coord.y - 1, coord.z + 1);
    }
}
