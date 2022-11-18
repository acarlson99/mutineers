using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapResizer : MonoBehaviour
{
    public Camera cam;
    public RawImage rawImage;

    void Update()
    {
        var margin = new Vector2(1, -1) * 15;

        rawImage.texture = cam.targetTexture;
        var minimapHeight = Screen.height / 4f;
        var imgW = (float)rawImage.texture.width;
        var imgH = (float)rawImage.texture.height;
        var w = minimapHeight * (imgW / imgH);
        rawImage.rectTransform.anchoredPosition = new Vector3(w / 2f, -minimapHeight / 2f) + (Vector3)margin;
        rawImage.rectTransform.sizeDelta = new Vector2(w, minimapHeight);
    }
}
