using JKFrame;
using System;
using UnityEngine;

public class TileCell
{
    #region 静态配置
    private static Vector3 forwardPos = new Vector3(0f, 0.5f, 0.5f);
    private static Vector3 backPos = new Vector3(0f, 0.5f, -0.5f);
    private static Vector3 leftPos = new Vector3(-0.5f, 0.5f, 0);
    private static Vector3 rightPos = new Vector3(0.5f, 0.5f, 0f);
    private static Vector3 topPos = new Vector3(0f, 1f, 0f);

    private static Vector3 forwardRotation = new Vector3(0f, 0f, 0f);
    private static Vector3 backRotation = new Vector3(0f, 180f, 0f);
    private static Vector3 leftRotation = new Vector3(0f, -90f, 0f);
    private static Vector3 rightRotation = new Vector3(0f, 90f, 0f);
    private static Vector3 topRotation = new Vector3(-90f, 0f, 0f);
    #endregion

    private TileTerrain _terrain;
    private TileTerrainCellData _cellData;
    private TileTerrainConfigItem _cellConfig;
    private Transform _parent;

    public GameObject cellRoot;

    private GameObject fowardGo;
    private GameObject backGo;
    private GameObject leftGo;
    private GameObject rightGo;
    private GameObject topGo;

    private bool showForward;
    private bool showBack;
    private bool showLeft;
    private bool showRight;
    private bool showTop;

    public void init(Transform parentTrans, TileTerrain terrainParent, TileTerrainCellData cellData, TileTerrainConfigItem configItem)
    {
        this._parent = parentTrans;
        this._cellData = cellData;
        this._cellConfig = configItem;
        this._terrain = terrainParent;
    }

    private void creatCellRoot(bool usePool)
    {
        if (usePool)
        {
            cellRoot = PoolSystem.GetGameObject("tileRoot");
        }
        if (cellRoot == null)
        {
            cellRoot = new GameObject();
            cellRoot.name = "tileRoot";
        }
        cellRoot.transform.SetParent(_parent);
        cellRoot.transform.position = _cellData.Postion;
        cellRoot.name = $"{_cellData.Coord.x}_{_cellData.Coord.y}_{_cellData.Coord.z}";
    }

    #region 检查面
    public void CheckAllFace(bool createObj, bool usePool)
    {
        if (createObj)
        {
            creatCellRoot(usePool);
        }
        CheckForward(createObj, usePool);
        CheckBack(createObj, usePool);
        CheckLeft(createObj, usePool);
        CheckRight(createObj, usePool);
        CheckTop(createObj, usePool);
    }

    public void CheckForward(bool createObj, bool usePool)
    {
        showForward = _terrain.GetForwordCell(_cellData.Coord) == null;
        if (showForward)
        {
            if (createObj && fowardGo == null)
            {
                if (usePool)
                {
                    fowardGo = PoolSystem.GetGameObject("forward", cellRoot.transform);
                }
                if (fowardGo == null)
                {
                    fowardGo = GameObject.Instantiate(_cellConfig.forwardPrefab, cellRoot.transform);
                    fowardGo.name = "forward";
                    Transform trans = fowardGo.transform;
                    trans.localScale = Vector3.one * _terrain.CellSize;
                    trans.localPosition = forwardPos * _terrain.CellSize;
                    trans.rotation = Quaternion.Euler(forwardRotation);
                }
            }
        }
        else
        {
            if (usePool)
            {
                PoolSystem.PushGameObject(fowardGo);
            }
            else
            {
                GameObject.DestroyImmediate(fowardGo);
            }
            fowardGo = null;
        }
    }

    public void CheckBack(bool createObj, bool usePool)
    {
        showBack = _terrain.GetBackCell(_cellData.Coord) == null;
        if (showBack)
        {
            if (createObj && backGo == null)
            {
                if (usePool)
                {
                    backGo = PoolSystem.GetGameObject("back", cellRoot.transform);
                }
                if (backGo == null)
                {
                    backGo = GameObject.Instantiate(_cellConfig.backPrefab, cellRoot.transform);
                    backGo.name = "back";
                    Transform trans = backGo.transform;
                    trans.localScale = Vector3.one * _terrain.CellSize;
                    trans.localPosition = backPos * _terrain.CellSize;
                    trans.rotation = Quaternion.Euler(backRotation);
                }
            }
        }
        else
        {
            if (usePool)
            {
                PoolSystem.PushGameObject(backGo);
            }
            else
            {
                GameObject.DestroyImmediate(backGo);
            }
            backGo = null;
        }
    }

    public void CheckLeft(bool createObj, bool usePool)
    {
        showLeft = _terrain.GetLeftCell(_cellData.Coord) == null;
        if (showLeft)
        {
            if (createObj && leftGo == null)
            {
                if (usePool)
                {
                    leftGo = PoolSystem.GetGameObject("left", cellRoot.transform);
                }
                if (leftGo == null)
                {
                    leftGo = GameObject.Instantiate(_cellConfig.leftPrefab, cellRoot.transform);
                    leftGo.name = "left";
                    Transform trans = leftGo.transform;
                    trans.localScale = Vector3.one * _terrain.CellSize;
                    trans.localPosition = leftPos * _terrain.CellSize;
                    trans.rotation = Quaternion.Euler(leftRotation);
                }
            }
        }
        else
        {
            if (usePool)
            {
                PoolSystem.PushGameObject(leftGo);
            }
            else
            {
                GameObject.DestroyImmediate(leftGo);
            }
            leftGo = null;
        }
    }

    public void CheckRight(bool createObj, bool usePool)
    {
        showRight = _terrain.GetRightCell(_cellData.Coord) == null;
        if (showRight)
        {
            if (createObj && rightGo == null)
            {
                if (usePool)
                {
                    rightGo = PoolSystem.GetGameObject("right", cellRoot.transform);
                }
                if (rightGo == null)
                {
                    rightGo = GameObject.Instantiate(_cellConfig.rightPrefab, cellRoot.transform);
                    rightGo.name = "right";
                    Transform trans = rightGo.transform;
                    trans.localScale = Vector3.one * _terrain.CellSize;
                    trans.localPosition = rightPos * _terrain.CellSize;
                    trans.rotation = Quaternion.Euler(rightRotation);
                }
            }
        }
        else
        {
            if (usePool)
            {
                PoolSystem.PushGameObject(rightGo);
            }
            else
            {
                GameObject.DestroyImmediate(rightGo);
            }
            rightGo = null;
        }
    }

    public void CheckTop(bool createObj, bool usePool)
    {
        showTop = _terrain.GetTopCell(_cellData.Coord) == null;
        if (showTop)
        {
            if (createObj && topGo == null)
            {
                if (usePool)
                {
                    topGo = PoolSystem.GetGameObject("top", cellRoot.transform);
                }
                if (topGo == null)
                {
                    topGo = GameObject.Instantiate(_cellConfig.topPrefab, cellRoot.transform);
                    topGo.name = "top";
                    Transform trans = topGo.transform;
                    trans.localScale = Vector3.one * _terrain.CellSize;
                    trans.localPosition = topPos * _terrain.CellSize;
                    trans.rotation = Quaternion.Euler(topRotation);
                }
            }
        }
        else
        {
            if (usePool)
            {
                PoolSystem.PushGameObject(topGo);
            }
            else
            {
                GameObject.DestroyImmediate(topGo);
            }
            topGo = null;
        }
    }


    public void ReplaceAll(TileTerrainConfigItem configItem)
    {
        this._cellConfig = configItem;
        if (showForward)
        {
            fowardGo.GetComponent<MeshFilter>().sharedMesh = configItem.forwardPrefab.GetComponent<MeshFilter>().sharedMesh;
            fowardGo.GetComponent<MeshRenderer>().sharedMaterial = configItem.forwardPrefab.GetComponent<MeshRenderer>().sharedMaterial;
        }
        if (showBack)
        {
            backGo.GetComponent<MeshFilter>().sharedMesh = configItem.backPrefab.GetComponent<MeshFilter>().sharedMesh;
            backGo.GetComponent<MeshRenderer>().sharedMaterial = configItem.backPrefab.GetComponent<MeshRenderer>().sharedMaterial;
        }
        if (showLeft)
        {
            leftGo.GetComponent<MeshFilter>().sharedMesh = configItem.leftPrefab.GetComponent<MeshFilter>().sharedMesh;
            leftGo.GetComponent<MeshRenderer>().sharedMaterial = configItem.leftPrefab.GetComponent<MeshRenderer>().sharedMaterial;
        }

        if (showRight)
        {
            rightGo.GetComponent<MeshFilter>().sharedMesh = configItem.rightPrefab.GetComponent<MeshFilter>().sharedMesh;
            rightGo.GetComponent<MeshRenderer>().sharedMaterial = configItem.rightPrefab.GetComponent<MeshRenderer>().sharedMaterial;
        }

        if (showTop)
        {
            topGo.GetComponent<MeshFilter>().sharedMesh = configItem.topPrefab.GetComponent<MeshFilter>().sharedMesh;
            topGo.GetComponent<MeshRenderer>().sharedMaterial = configItem.topPrefab.GetComponent<MeshRenderer>().sharedMaterial;
        }
    }

    public Vector3 GetCellPosition()
    {
        return _cellData.Postion;
    }

    #endregion

    public void DestoryGameObject(bool usePool)
    {
        if (usePool)
        {
            PoolSystem.PushGameObject(cellRoot);
        }
        else
        {
#if UNITY_EDITOR
            GameObject.DestroyImmediate(cellRoot);
            return;
#endif
            GameObject.Destroy(cellRoot);
        }
    }
}
