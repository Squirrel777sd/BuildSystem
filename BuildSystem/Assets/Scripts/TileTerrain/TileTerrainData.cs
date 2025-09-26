using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif


[CreateAssetMenu(fileName = "TerrainData", menuName = "TileTerrain/TerrainData")]
public class TileTerrainData : ConfigBase
{
    public float cellSize;
    public Vector3Int mapSize = new Vector3Int(10, 5, 10);
    public TileTerrainCellData[,,] cellDatas;

#if UNITY_EDITOR
    public bool enablePreview;
    public void Save()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssetIfDirty(this);
    }

    public void SetCellSize(float cellSize)
    {
        this.cellSize = cellSize;
        this.CreateDefultData();
        Save();
    }

    public void SetMapSize(Vector3Int mapSize)
    {
        this.mapSize = mapSize;
        this.CreateDefultData();
        Save();
    }

    private void Reset()
    {
        CreateDefultData();
    }

    public void CreateDefultData()
    {
        cellDatas = new TileTerrainCellData[mapSize.x, mapSize.y, mapSize.z];
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    TileTerrainCellData cellData = new TileTerrainCellData();
                    cellData.Init(0, new Vector3Int(x, y, z), cellSize);
                    cellDatas[x, y, z] = cellData;
                }
            }
        }
    }
#endif
}


/// <summary>
/// 单个地形格子的数据
/// </summary>
[Serializable]
public class TileTerrainCellData
{
    public int index;
    public int Index
    {
        get
        {
            return index;
        }
        set
        {
            index = value;
        }
    }
    public Vector3 postion;
    public Vector3Int coord;


    public void Init(int index, Vector3Int coord, float cellSize)
    {
        this.index = index;
        this.coord = coord;
        this.postion = new Vector3(coord.x * cellSize + cellSize / 2, coord.y * cellSize, coord.z * cellSize + cellSize / 2);
    }
}
