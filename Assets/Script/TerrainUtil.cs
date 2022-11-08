using System;
using System.Reflection;
using UnityEngine;

public class TerrainUtil : MonoBehaviour
{
    [ContextMenu("Change Grid Size")]
    void ChangeGridSize()
    {
        Assembly assembly = Assembly.Load("UnityEditor.dll");
        Type gridSettings = assembly.GetType("UnityEditor.GridSettings");
        PropertyInfo gridSize = gridSettings.GetProperty("size");
        gridSize.SetValue("size", new Vector3(1, 1, 1));
    }

    void SnapPos()
    {
        Vector3 pos = transform.position;
        var fx = Mathf.Floor(pos.x);
        var fy = Mathf.Floor(pos.y);
        if (Mathf.Abs(fx - pos.x) >= 0.25f) fx += 0.5f;
        if (Mathf.Abs(fy - pos.y) >= 0.25f) fy += 0.5f;
        transform.position = new Vector3(fx, fy, pos.z);
    }

    void SnapScale()
    {
        Vector3 scale = transform.localScale;
        var fx = Mathf.Floor(scale.x);
        var fy = Mathf.Floor(scale.y);
        if (Mathf.Abs(fx - scale.x) >= 0.25f) fx += 0.5f;
        if (Mathf.Abs(fy - scale.y) >= 0.25f) fy += 0.5f;
        transform.localScale = new Vector3(fx, fy, scale.z);
    }

    [ContextMenu("Snap")]
    void Snap()
    {
        SnapScale();
        SnapPos();
    }
}
