using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileTerrain : MonoBehaviour
{
    [LabelText("地块配置")]
    [OnValueChanged("OnConfigOrDataChanged")]
    public TileTerrainTileConfig tileConfig;

    [LabelText("地形配置")]
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

    public float CellSize
    {
        get
        {
            return terrainData.cellSize;
        }
    }

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
        for (int x = 0; x < terrainData.mapSize.x; x++)
        {
            for (int z = 0; z < terrainData.mapSize.z; z++)
            {
                TileCell cell = cells[x, 0, z];
                if (cell != null)
                {
                    cell.CheckAllFace(true, false);
                }
                cells[x, 0, z] = cell;
            }
        }
    }
#endif


    // 创建具体的格子类 填充数据 但是并不会创建游戏物体
    // 不能直接绘制，因为运行时，不回绘制全部格子
    public void CreateAllCell()
    {
        if (tileConfig != null && terrainData != null && cellData != null)
        {
            cells = new TileCell[terrainData.mapSize.x, terrainData.mapSize.y, terrainData.mapSize.z];
            for (int x = 0; x < terrainData.mapSize.x; x++)
            {
                for (int z = 0; z < terrainData.mapSize.z; z++)
                {
                    TileTerrainCellData data = this.terrainData.cellDatas[x, 0, z];
                    TileCell cell = new TileCell();
                    cell.init(transform, this, data, tileConfig.tileConfigList[data.Index]);
                    cells[x, 0, z] = cell;
                }
            }
        }
    }


    public TileCell GetCell(int x, int y, int z)
    {
        if (x < 0 || x >= terrainData.mapSize.x || y < 0 || y >= terrainData.mapSize.y || z < 0 || z >= terrainData.mapSize.z)
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
