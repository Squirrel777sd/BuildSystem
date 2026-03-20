using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class InterscetRayMeshTool
{
    public static Type handleUniltiType;
    public static MethodInfo rayMeshMethodInfo;

    static InterscetRayMeshTool()
    {
        Type[] types = typeof(Editor).Assembly.GetTypes();
        Type type = null;
        foreach (Type item in types)
        {
            if (item.Name == "HandleUtility")
            {
                type = item;
                break;
            }
        }

        rayMeshMethodInfo = type?.GetMethod("IntersectRayMesh", (BindingFlags.Static | BindingFlags.NonPublic));
    }

    public static bool IntersectRayMesh(Ray ray, MeshFilter meshFilter, out RaycastHit hit)
    {
        object[] pars = new object[] { ray, meshFilter.sharedMesh, meshFilter.transform.localToWorldMatrix, null };
        bool result = (bool)rayMeshMethodInfo.Invoke(null,pars);
        hit = (RaycastHit)pars[3];
        return result;
    }
}
