using Sirenix.OdinInspector.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(TileTerrain))]
public class TileTerrainEditor : OdinEditor
{
    [MenuItem("GameObject/TileTerrain")]
    private static void CreateTileTerrainObject()
    {
        GameObject terrain = new GameObject("TileTerrain");
        terrain.AddComponent<TileTerrain>();
        Selection.activeGameObject = terrain;
    }

    public VisualTreeAsset editorUIAsset;
    private VisualElement root;
    private TemplateContainer editorUIAssetInstance;
    private IMGUIContainer baseGUI;
    private TileTerrain terrain { get => (TileTerrain)target; }
    public GameObject prefab;

    private void init()
    {
        if (terrain.tileConfig == null || terrain.terrainData == null)
        {
            return;
        }
        if (editorUIAssetInstance == null)
        {
            editorUIAssetInstance = editorUIAsset.Instantiate();
            initPanels();
        }
        initMenu();
        initTileTerrainPanel();
        initItemPanel();
        initSettingPanel();
        switchPanel(PanelType.TERRAIN);
    }

    private TileTerrainTileConfig lastConfig;
    private void DrawEditor()
    {
        init();
        if (terrain.tileConfig == null || terrain.terrainData == null)
        {
            if (root.Contains(editorUIAssetInstance))
            {
                root.Remove(editorUIAssetInstance);
            }
            return;
        }
        else
        {
            if (!root.Contains(editorUIAssetInstance))
            {
                root.Add(editorUIAssetInstance);
            }
        }

        if (lastConfig != null)
        {
            lastConfig.ClearAction();
        }
        lastConfig = terrain.tileConfig;
        if (terrain.tileConfig != null && !terrain.tileConfig.isSetConfigChangeAction())
        {
            // 监听，如果用户修改了配置
            terrain.tileConfig.SetConfigChangedAction(OnTileConfigChanged);
        }
    }

    // 当设置地块配置
    private void OnTileConfigOrDataSet()
    {
        DrawEditor();
    }

    // 当地块配置修改时要触发的行为
    private void OnTileConfigChanged()
    {
        //重新初始化地形面板
        initTileTerrainPanel();
        //ClearPrefabPreviews();
        if (terrain.terrainData.enablePreview)
        {
            //重新创建格子
            terrain.CreateAllCell();
            terrain.CreateAllCellGameObjectForEditor();
        }
    }


    #region 公共方法 
    private Camera SceneCamera
    {
        get
        {
            return SceneView.currentDrawingSceneView.camera;
        }
    }

    private Vector3 getMousePosition()
    {
        Vector2 eventMousePos = Event.current.mousePosition;
        Vector3 mousePosition = eventMousePos;
        mousePosition.y = SceneCamera.pixelHeight - eventMousePos.y;
        return mousePosition;

    }

    private bool leftShiftKeyDown = false;

    private void updateLeftShiftKeyDown()
    {
        if (Event.current.type == EventType.KeyDown)
        {
            leftShiftKeyDown = Event.current.keyCode == KeyCode.LeftShift;
        }
        else if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.LeftShift)
        {
            leftShiftKeyDown = false;
        }
        Debug.Log(leftShiftKeyDown);
    }
    #endregion

    #region 菜单
    enum PanelType
    {
        TERRAIN,
        ITEM,
        SETTING,
    }


    private List<Button> menuButtons;
    private static Color noramlMenuItemColor = new Color(0.35f, 0.35f, 0.35f);
    private static Color selectMenuItemColor = new Color(0.24f, 0.57f, 0.7f);
    private Toggle previewToggle;
    private void initMenu()
    {
        menuButtons = new List<Button>();
        Toolbar toolbar = editorUIAssetInstance.Q<Toolbar>("menuToolbar");
        previewToggle = editorUIAssetInstance.Q<Toggle>("previewToggle");
        previewToggle.RegisterValueChangedCallback(previewToggleValueChange);
        previewToggle.value = terrain.terrainData.enablePreview;
        for (int i = 0; i < toolbar.childCount; i++)
        {
            Button button = toolbar[i] as Button;
            PanelType panelType = (PanelType)i;
            button.clicked += () =>
            {
                switchPanel(panelType);
            };
            menuButtons.Add(button);
        }
    }

    private void previewToggleValueChange(ChangeEvent<bool> evt)
    {
        terrain.terrainData.enablePreview = evt.newValue;
        if (evt.newValue)
        {
            //绘制地图
            terrain.CreateAllCell();
            terrain.CreateAllCellGameObjectForEditor();
        }
        else
        {
            //取消绘制
            terrain.CleanCellsForEditor();
        }
    }


    #endregion

    #region 面板控制
    private VisualElement panelParent;
    private List<VisualElement> panels;
    private static PanelType curPanelType;
    private void initPanels()
    {
        panels = new List<VisualElement>();
        panelParent = editorUIAssetInstance.Q<VisualElement>("PanelParent");
        for (int i = 0; i < panelParent.childCount; i++)
        {
            VisualElement element = panelParent[i];
            panels.Add(element);
        }
    }
    private void switchPanel(PanelType panelType)
    {
        curPanelType = panelType;
        for (int i = 0; i < menuButtons.Count; i++)
        {
            Button btn = menuButtons[i];
            if (i == (int)panelType)
            {
                btn.style.backgroundColor = selectMenuItemColor;
            }
            else
            {
                btn.style.backgroundColor = noramlMenuItemColor;
            }
        }

        panelParent.Clear();
        panelParent.Add(panels[(int)panelType]);
    }
    #endregion

    #region 地形面板
    private static List<TilePrefabPreView> tilePrefabPreViews = new List<TilePrefabPreView>();
    private static TilePrefabPreView curSelectPrefabView;
    private VisualElement scrollContent;
    private DropdownField operationTypeDp;

    private void onSelectPreViewPrefab(TilePrefabPreView prefabPreView)
    {
        curSelectPrefabView = prefabPreView;
        for (int i = 0; i < tilePrefabPreViews.Count; i++)
        {
            tilePrefabPreViews[i].UnSelect();
        }
        curSelectPrefabView?.Select();
    }
    private void ClearPrefabPreviews()
    {
        curSelectPrefabView = null;

        for (int i = 0; i < tilePrefabPreViews.Count; i++)
        {
            tilePrefabPreViews[i].Destory();
        }
        tilePrefabPreViews.Clear();
    }

    private void initTileTerrainPanel()
    {
        ClearPrefabPreviews();
        onSelectPreViewPrefab(curSelectPrefabView);
        if (scrollContent == null)
        {
            scrollContent = panels[(int)PanelType.TERRAIN].Q<ScrollView>("TileTerrainPrefabView").contentContainer;
        }
        operationTypeDp = panels[(int)PanelType.TERRAIN].Q<DropdownField>("operationTypeDp");
        operationTypeDp.RegisterValueChangedCallback(onDpValueChange);
        for (int i = 0; i < terrain.tileConfig.tileConfigList.Count; i++)
        {
            GameObject prefab = terrain.tileConfig.tileConfigList[i].topPrefab;
            TilePrefabPreView preView = new TilePrefabPreView();
            preView.Init(scrollContent, i, prefab, onSelectPreViewPrefab);
            tilePrefabPreViews.Add(preView);
            EditorUtility.SetDirty(terrain);
        }

    }

    /// <summary>
    /// 1:增加 2.移除 3.替换
    /// </summary>
    private static int curOperation = -1;
    private void onDpValueChange(ChangeEvent<string> evt)
    {
        operationTypeDp.value = evt.newValue;
        curOperation = operationTypeDp.index;
    }


    private void onTileTerrainPanelSceneUI()
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        Vector3Int coord = Vector3Int.down;
        Vector3 testGoPos = Vector3.zero;
        if (curOperation != 0 && curSelectPrefabView != null)
        {
            // 禁止用户选择游戏物体

            MeshFilter[] meshFilters = terrain.tileParent.GetComponentsInChildren<MeshFilter>();
            Vector3 mousePosition = getMousePosition();
            Ray ray = SceneCamera.ScreenPointToRay(mousePosition);
            float minDis = -1;
            for (int i = 0; i < meshFilters.Length; i++)
            {
                MeshFilter meshFilter = meshFilters[i];
                if (meshFilter == null)
                {
                    continue;
                }
                RaycastHit hitInfo;
                bool isHit = InterscetRayMeshTool.IntersectRayMesh(ray, meshFilter, out hitInfo);
                if (isHit && (minDis < 0 || minDis > hitInfo.distance))
                {
                    testGoPos = hitInfo.point;
                    minDis = hitInfo.distance;
                    coord = terrain.getCoordByWorldPosition(hitInfo.point);
                }
            }
            if (coord.y < 0 || minDis < 0)
            {
                return;
            }
            terrain.testGo.transform.position = testGoPos;
            if (curOperation == 1)
            {
                TileCell topCell = terrain.GetTopCell(coord);
                if (topCell == null)
                {
                    terrain.SetWireCubePosAndOperation(coord, curOperation);
                }
            }
            else
            {
                terrain.SetWireCubePosAndOperation(coord, curOperation);
            }
            if (Event.current.type == EventType.MouseDown)
            {
                if (curOperation == 1)
                {
                    terrain.AddCell(curSelectPrefabView.TileConfigIndex, new Vector3Int(coord.x, coord.y + 1, coord.z));
                }
                else if (curOperation == 2)
                {
                    terrain.RemoveCell(coord);
                }
                else if (curOperation == 3)
                {
                    terrain.ReplaceCell(coord, curSelectPrefabView.TileConfigIndex);
                }
            }
        }
        else
        {
            terrain.SetWireCubePosAndOperation(Vector3Int.one * -1, -1);
        }
    }
    #endregion

    #region 物品面板

    private static List<ItemPrefabPreView> ItemPrefabPreViews = new List<ItemPrefabPreView>();
    private static ItemPrefabPreView curSelectItemPrefabView;
    private VisualElement itemScrollContent;
    private static GameObject curSelectItemObj;
    private void initItemPanel()
    {
        ClearItemPrefabPreView();
        onSelectItemPreView(curSelectItemPrefabView);
        if (itemScrollContent == null)
        {
            itemScrollContent = panels[(int)PanelType.ITEM].Q<ScrollView>("ItemPrefabPreView").contentContainer;
        }

        for (int i = 0; i < terrain.tileConfig.itemConfigList.Count; i++)
        {
            GameObject prefab = terrain.tileConfig.itemConfigList[i].prefab;
            ItemPrefabPreView preView = new ItemPrefabPreView();
            preView.Init(itemScrollContent, prefab, i, onSelectItemPreView);
            ItemPrefabPreViews.Add(preView);
            EditorUtility.SetDirty(terrain);
        }
    }

    private void ClearItemPrefabPreView()
    {
        curSelectItemPrefabView = null;
        for (int i = 0; i < ItemPrefabPreViews.Count; i++)
        {
            ItemPrefabPreViews[i].Destory();
        }
        ItemPrefabPreViews.Clear();
    }

    private void onSelectItemPreView(ItemPrefabPreView curSelectPrefabView)
    {
        if (curSelectPrefabView == null)
        {
            return;
        }
        for (int i = 0; i < ItemPrefabPreViews.Count; i++)
        {
            ItemPrefabPreViews[i].UnSelect();
        }
        curSelectPrefabView.Select();
        curSelectItemPrefabView = curSelectPrefabView;
        if (curSelectItemObj != null)
        {
            GameObject.DestroyImmediate(curSelectItemObj);
            curSelectItemObj = null;
        }
        if (curSelectItemObj == null)
        {
            curSelectItemObj = GameObject.Instantiate(curSelectPrefabView.itemPrefab, terrain.itemParent);
            curSelectItemObj.name = curSelectPrefabView.itemPrefab.name;
        }
    }

    private void onItemPanelSceneUI()
    {
        if (curSelectItemPrefabView != null)
        {
            // 禁止用户选择游戏物体
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            MeshFilter[] meshFilters = terrain.tileParent.GetComponentsInChildren<MeshFilter>();
            Vector3 mousePosition = getMousePosition();
            Ray ray = SceneCamera.ScreenPointToRay(mousePosition);
            float minDis = -1;
            Vector3Int coord = -Vector3Int.one;
            for (int i = 0; i < meshFilters.Length; i++)
            {
                MeshFilter meshFilter = meshFilters[i];
                if (meshFilter == null)
                {
                    continue;
                }
                RaycastHit hitInfo;
                bool isHit = InterscetRayMeshTool.IntersectRayMesh(ray, meshFilter, out hitInfo);
                if (isHit && (minDis < 0 || minDis > hitInfo.distance))
                {
                    minDis = hitInfo.distance;
                    coord = terrain.getCoordByWorldPosition(hitInfo.point);
                }
            }
            if (minDis != -1)
            {
                terrain.SetWireCubePosAndOperation(coord, leftShiftKeyDown ? 2 : 1);
                curSelectItemObj.SetActive(!leftShiftKeyDown);
                if (curSelectItemObj != null)
                {
                    TileCell cell = terrain.GetCell(coord);
                    if (cell != null)
                    {
                        Vector3 pos = cell.GetCellPosition();
                        curSelectItemObj.transform.position = new Vector3(pos.x, pos.y + terrain.CellSize, pos.z);
                    }
                }
                if (Event.current.type == EventType.MouseDown)
                {
                    TileCell tileCell = terrain.GetCell(coord);
                    if (tileCell == null) return;
                    if (leftShiftKeyDown)
                    {
                        tileCell.RemoveItem();
                    }
                    else
                    {
                        tileCell.SetItem(curSelectItemPrefabView.ItemConfigIndex);
                    }
                }
            }
        }
        else
        {
            terrain.SetWireCubePosAndOperation(Vector3Int.one * -1, -1);
        }
    }
    #endregion

    #region 设置面板
    private FloatField cellSize;
    private Vector3IntField mapSize;

    private void initSettingPanel()
    {
        VisualElement panel = panels[(int)PanelType.SETTING];
        cellSize = panel.Q<FloatField>("CellSize");
        mapSize = panel.Q<Vector3IntField>("MapSize");

        cellSize.value = terrain.terrainData.cellSize;
        mapSize.value = terrain.terrainData.mapSize;

        cellSize.RegisterCallback<FocusInEvent>(CellSizeForcusIn);
        cellSize.RegisterCallback<FocusOutEvent>(CellSizeForcusOut);

        mapSize.RegisterCallback<FocusInEvent>(MapSizeForcursIn);
        mapSize.RegisterCallback<FocusOutEvent>(MapSizeForcusOut);
    }


    private float oldCellSize;
    private void CellSizeForcusIn(FocusInEvent evt)
    {
        oldCellSize = cellSize.value;
    }

    private void CellSizeForcusOut(FocusOutEvent evt)
    {
        if (oldCellSize != cellSize.value)
        {
            terrain.terrainData.SetCellSize(cellSize.value);
        }
    }

    private Vector3Int oldMapSize;
    private void MapSizeForcursIn(FocusInEvent evt)
    {

    }

    private void MapSizeForcusOut(FocusOutEvent evt)
    {
        if (oldMapSize != mapSize.value)
        {
            terrain.terrainData.SetMapSize(mapSize.value);
            terrain.CreateAllCell();
            if (terrain.terrainData.enablePreview)
            {
                terrain.CreateAllCellGameObjectForEditor();
            }
        }
    }
    #endregion

    #region 系统方法
    public override VisualElement CreateInspectorGUI()
    {
        root = new VisualElement();
        baseGUI = new IMGUIContainer(() =>
        {
            base.DrawDefaultInspector();
        });
        root.Add(baseGUI);
        terrain.SetOnTileConfigChangeAction(OnTileConfigOrDataSet);
        if (terrain.terrainData == null || terrain.tileConfig == null)
        {
            // 监听，如果用户又设置了配置
            return root;
        }

        DrawEditor();
        return root;
    }

    public void OnSceneGUI()
    {
        if (terrain == null)
        {
            return;
        }
        if (terrain.terrainData == null || terrain.tileConfig == null || terrain.terrainData.enablePreview == false)
        {
            return;
        }

        updateLeftShiftKeyDown();
        switch (curPanelType)
        {
            case PanelType.TERRAIN:
                onTileTerrainPanelSceneUI();
                break;
            case PanelType.ITEM:
                onItemPanelSceneUI();
                break;
            case PanelType.SETTING:
                break;
        }

    }

    protected override void OnDisable()
    {
        if (terrain.terrainData != null)
        {
            terrain.terrainData.Save();
        }
        curSelectPrefabView = null;
        ClearPrefabPreviews();
        ClearItemPrefabPreView();
        if (curSelectItemObj != null)
        {
            GameObject.DestroyImmediate(curSelectItemObj);
            curSelectItemObj = null;
        }
        base.OnDisable();
    }
    #endregion
}
