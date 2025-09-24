using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JKFrame;
using System;


[CreateAssetMenu(fileName = "TerrainData",menuName = "TileTerrain/TerrainData")]
public class TileTerrainData : ConfigBase
{
    public float cellSize;
    public Vector3Int terrainSize;
    public TileTerrainCellData[,,] cellDatas;
}


/// <summary>
/// �������θ��ӵ�����
/// </summary>
[Serializable]
public class TileTerrainCellData
{
    private int index;


    public Vector3 postion;
    public Vector3Int coord;
}
