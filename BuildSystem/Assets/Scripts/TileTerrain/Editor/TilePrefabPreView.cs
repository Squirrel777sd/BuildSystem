using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class TilePrefabPreView
{
    private GameObject tilePrefab;
    private VisualElement rootElement;
    private Mesh tileMesh;
    private Material tileMaterial;
    private PreviewRenderUtility renderUtility;
    private IMGUIContainer IMGUIContainer;
    private Action<TilePrefabPreView> mouseSelectCallBack;
    private string prefabName;
    public void Init(VisualElement rootElement,GameObject prefab,Action<TilePrefabPreView> mouseSelectCallBack)
    {
        this.rootElement = rootElement;
        this.tilePrefab = prefab;
        tileMaterial = prefab.GetComponent<MeshRenderer>().sharedMaterial;
        tileMesh = prefab.GetComponent<MeshFilter>().sharedMesh;
        this.prefabName = prefab.name;

        IMGUIContainer = new IMGUIContainer();
        IMGUIContainer.style.height = 35;
        IMGUIContainer.style.width = 35;
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
        renderUtility.camera.transform.position = new Vector3(0, 0, -3f);
        renderUtility.lights[0].color = Color.white;
        renderUtility.lights[0].transform.rotation = Quaternion.Euler(new Vector3(55f, 0, 0));
        renderUtility.lights[0].intensity = 1;
        UnSelect();
        IMGUIContainer.onGUIHandler = DrawTilePreview;
        rootElement.Add(IMGUIContainer);
    }

    private void onMouseClicked (MouseDownEvent evt)
    {
        mouseSelectCallBack?.Invoke(this);
    }

    private void DrawTilePreview()
    {
        renderUtility.BeginPreview(IMGUIContainer.contentRect, GUIStyle.none);
        renderUtility.DrawMesh(tileMesh, Vector3.zero, Quaternion.Euler(new Vector3(0, 180f, 0)), tileMaterial, 0);
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
        //IMGUIContainer.onGUIHandler = null;
        rootElement.Remove(IMGUIContainer);
    }
}
