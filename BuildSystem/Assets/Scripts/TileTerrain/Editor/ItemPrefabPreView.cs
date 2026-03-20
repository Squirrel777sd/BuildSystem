using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class ItemPrefabPreView
{
    public GameObject itemPrefab;
    private VisualElement rootElement;
    private Mesh itemMesh;
    private Material[] materials;
    private PreviewRenderUtility renderUtility;
    private IMGUIContainer IMGUIContainer;
    private Action<ItemPrefabPreView> mouseSelectCallBack;
    private string prefabName;
    private int itemConfigIndex;
    public int ItemConfigIndex
    {
        get
        {
            return itemConfigIndex;
        }
    }

    private Vector3 localPos;
    private Quaternion localRotation;

    public void Init(VisualElement rootElement, GameObject prefab,int itemConfigIndex ,Action<ItemPrefabPreView> mouseSelectCallBack)
    {
        this.rootElement = rootElement;
        this.itemPrefab = prefab;
        this.itemConfigIndex = itemConfigIndex;
        MeshRenderer renderer = prefab.GetComponentInChildren<MeshRenderer>();
        materials = renderer.sharedMaterials;
        MeshFilter meshFilter = prefab.GetComponentInChildren<MeshFilter>();
        itemMesh = meshFilter.sharedMesh;
        this.prefabName = prefab.name;

        localPos = meshFilter.transform.localPosition;
        localRotation = meshFilter.transform.localRotation;

        IMGUIContainer = new IMGUIContainer();
        IMGUIContainer.style.height = 65;
        IMGUIContainer.style.width = 65;
        IMGUIContainer.style.marginRight = 2;

        IMGUIContainer.style.borderLeftWidth = 2f;
        IMGUIContainer.style.borderRightWidth = 2f;
        IMGUIContainer.style.borderTopWidth = 2f;
        IMGUIContainer.style.borderBottomWidth = 2f;

        IMGUIContainer.RegisterCallback<MouseDownEvent>(onMouseClicked);
        this.mouseSelectCallBack = mouseSelectCallBack;

        renderUtility = new PreviewRenderUtility();
        renderUtility.camera.farClipPlane = 30;
        renderUtility.camera.nearClipPlane = 0.3f;
        renderUtility.camera.clearFlags = CameraClearFlags.Color;
        renderUtility.camera.transform.position = new Vector3(0, 1.2f, -10f);
        renderUtility.lights[0].color = Color.white;
        renderUtility.lights[0].transform.rotation = Quaternion.Euler(new Vector3(55f, 0, 0));
        renderUtility.lights[0].intensity = 1;
        UnSelect();
        IMGUIContainer.onGUIHandler = DrawItemPreview;
        rootElement.Add(IMGUIContainer);
    }

    private void onMouseClicked(MouseDownEvent evt)
    {
        mouseSelectCallBack?.Invoke(this);
    }

    private void DrawItemPreview()
    {
        renderUtility.BeginPreview(IMGUIContainer.contentRect, GUIStyle.none);
        for (int i = 0; i < itemMesh.subMeshCount; i++)
        {
            renderUtility.DrawMesh(itemMesh, localPos, localRotation, materials[i], 0);
        }
        renderUtility.camera.Render();
        renderUtility.EndAndDrawPreview(IMGUIContainer.contentRect);
    }

    public void Select()
    {
        IMGUIContainer.style.borderLeftColor = Color.blue;
        IMGUIContainer.style.borderRightColor = Color.blue;
        IMGUIContainer.style.borderTopColor = Color.blue;
        IMGUIContainer.style.borderBottomColor = Color.blue;
    }

    public void UnSelect()
    {
        IMGUIContainer.style.borderLeftColor = Color.white;
        IMGUIContainer.style.borderRightColor = Color.white;
        IMGUIContainer.style.borderTopColor = Color.white;
        IMGUIContainer.style.borderBottomColor = Color.white;
    }


    public void Destory()
    {
        if (renderUtility != null)
        {
            renderUtility.Cleanup();
        }
        renderUtility = null;
        rootElement.Remove(IMGUIContainer);
    }
}
