using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleToCamSize : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float cameraHeight = Camera.main.orthographicSize * 2;
        float cameraWidth = cameraHeight * Screen.width / Screen.height;

        gameObject.transform.localScale = new Vector3(cameraWidth, cameraHeight, 0);
    }
}
