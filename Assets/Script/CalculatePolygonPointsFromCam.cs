using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CalculatePolygonPointsFromCam : MonoBehaviour
{
    public Camera cam;
    public PolygonCollider2D poly;

    [ContextMenu("CalcPoints")]
    public void CalculatePoints()
    {
        //poly.points
        var h = cam.orthographicSize * 2;
        Debug.Log(h);
        var w = h * ((float)cam.pixelWidth / (float)cam.pixelHeight);
        Debug.Log($"{cam.pixelWidth} {cam.pixelHeight}");
        Debug.Log(w);

        var xs = (new Vector2(-w, w) / 2) + (Vector2.one * cam.transform.position.x);
        Debug.Log(xs);

        var ys = (new Vector2(-h, h) / 2) + (Vector2.one * cam.transform.position.y);
        Debug.Log(ys);

        //int i = 0;
        //foreach (var x in xs)
        //{
        //    foreach (var y in ys)
        //    {
        //        Debug.Log(poly.points[i]);
        //        poly.points[i] = new Vector2(x, y);
        //        Debug.Log($"TO: {poly.points[i]}");
        //        i++;
        //    }
        //}
        poly.points = new Vector2[] { new Vector2(xs[0], ys[0]),
                                      new Vector2(xs[0], ys[1]),
                                      new Vector2(xs[1], ys[1]),
                                      new Vector2(xs[1], ys[0]) };
    }
}
