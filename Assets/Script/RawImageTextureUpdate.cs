using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RawImageTextureUpdate : MonoBehaviour
{
    public Camera cam;
    public RawImage rawImage;

    // Start is called before the first frame update
    void Start()
    {
        rawImage.texture = cam.targetTexture;
        var w = (float)rawImage.texture.width;
        var h = (float)rawImage.texture.height;
        Debug.Log(w);
        Debug.Log(h);
        //rawImage.rectTransform;
        var h_ = 300 / (w / h);
        var pos = rawImage.rectTransform.anchoredPosition;
        rawImage.rectTransform.anchoredPosition = new Vector3(pos.x, -h_ / 2);
        rawImage.rectTransform.sizeDelta = new Vector2(300, h_);
    }
}
