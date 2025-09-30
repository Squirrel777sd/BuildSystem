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


    public GameObject testGo;
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

    #region Editor

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
            for (int y = 0; y < terrainData.mapSize.y; y++)
            {
                for (int z = 0; z < terrainData.mapSize.z; z++)
                {
                    TileCell cell = cells[x, y, z];
                    if (cell != null)
                    {
                        cell.CheckAllFace(true, false);
                    }
                    cells[x, y, z] = cell;
                }
            }
        }
    }

    private Vector3Int wireCubePos;
    private int operationType = -1;
    public void SetWireCubePosAndOperation(Vector3Int pos,int operationType)
    {
        wireCubePos = pos;
        this.operationType = operationType;
    }

    public void OnDrawGizmos()
    {
        if (terrainData.enablePreview && wireCubePos != null && wireCubePos != Vector3.one * -1)
        {
            if (wireCubePos.x > terrainData.mapSize.x ||
                wireCubePos.y > terrainData.mapSize.y || 
                wireCubePos.z > terrainData.mapSize.z || 
                operationType == -1)
            {
                return;
            }
            TileCell cell = GetCell(wireCubePos);
            if (cell == null || operationType == -1)
            {
                return;
            }
            Vector3 cellPos = cell.GetCellPosition();
            if (operationType == 1)
            {
                Gizmos.DrawWireCube(new Vector3(cellPos.x, cellPos.y + CellSize + CellSize / 2, cellPos.z), Vector3.one * CellSize);
                return;
            }

            Gizmos.DrawWireCube(new Vector3(cellPos.x, cellPos.y + CellSize / 2, cellPos.z), Vector3.one * CellSize);
        }
    }
#endif
    #endregion


    #region 格子

    // 创建具体的格子类 填充数据 但是并不会创建游戏物体
    // 不能直接绘制，因为运行时，不回绘制全部格子
    public void CreateAllCell()
    {
        if (tileConfig != null && terrainData != null && cellData != null)
        {
            cells = new TileCell[terrainData.mapSize.x, terrainData.mapSize.y, terrainData.mapSize.z];
            for (int x = 0; x < terrainData.mapSize.x; x++)
            {
                for (int y = 0; y < terrainData.mapSize.y; y++)
                {
                    for (int z = 0; z < terrainData.mapSize.z; z++)
                    {
                        TileTerrainCellData data = this.terrainData.cellDatas[x, y, z];
                        data.InitPostion(CellSize);
                        TileCell cell = new TileCell();
                        cell.init(transform, this, data, tileConfig.tileConfigList[y >= 1 ? 1 : y]);
                        cells[x, y, z] = cell;
                    }
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
        return GetCell(coord.x + 1, coord.y, coord.z);
    }

    public TileCell GetTopCell(Vector3Int coord)
    {
        return GetCell(coord.x, coord.y + 1, coord.z);
    }

    public TileCell GetBottomCell(Vector3Int coord)
    {
        return GetCell(coord.x, coord.y - 1, coord.z);
    }

    public TileCell GetCellByWorldPostion(Vector3 pos)
    {
        return GetCell(getCoordByWorldPosition(pos));
    }

    public Vector3Int getCoordByWorldPosition(Vector3 worldPostion)
    {
        float offset = CellSize / 2;
        int x = Mathf.RoundToInt(Mathf.Clamp(worldPostion.x / CellSize - offset, 0, terrainData.mapSize.x - 1));
        int y = Mathf.RoundToInt(Mathf.Clamp(worldPostion.y / CellSize - offset, 0, terrainData.mapSize.y - 1));
        int z = Mathf.RoundToInt(Mathf.Clamp(worldPostion.z / CellSize - offset, 0, terrainData.mapSize.z - 1));

        return new Vector3Int(x, y, z);
    }
    #endregion
}
