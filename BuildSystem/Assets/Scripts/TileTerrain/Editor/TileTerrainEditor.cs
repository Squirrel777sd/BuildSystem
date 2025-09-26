using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

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

        init();
        root.Add(editorUIAssetInstance);
        return root;
    }

    private void init()
    {
        if (editorUIAssetInstance == null)
        {
            editorUIAssetInstance = editorUIAsset.Instantiate();
            initPanels();
        }
        initMenu();
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

    }

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
    private PanelType curPanelType;
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
        }
    }
    #endregion

    protected override void OnDisable()
    {
        base.OnDisable();
        terrain.terrainData.Save();
    }
}
