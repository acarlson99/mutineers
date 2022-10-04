using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 offset;
    public float currentZoom = 1.0f;
    public float zoomSpeed = 100f;
    public float minZoom = 0.5f;
    public float maxZoom = 2;
    public float panSpeed = 20f;
    public float panMargin = 30f;
    public GameObject winCanvas;
    public BoxCollider2D boundingBox;

    //public Vector2 minBound = new Vector2(-10f, -10f);
    //public Vector2 maxBound = new Vector2(10f, 10f);
    private Camera cam;
    private float size;
    private Vector2 pos;

    private void Start()
    {
        cam = GetComponent<Camera>();
        size = cam.orthographicSize;
    }

    private void Update()
    {
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        // TODO: linearly interpolate (somehow idk)
        if (scroll != 0)
        {
            currentZoom -= Time.deltaTime * scroll * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        }
        static bool inRange(float a, float b, float c) => a <= b && b <= c;
        pos = (Vector2)transform.position;
        if (inRange(0, Input.mousePosition.x, Screen.width) && inRange(0, Input.mousePosition.y, Screen.height))
        {
            if (inRange(0, Input.mousePosition.x, panMargin)) pos.x -= Time.deltaTime * panSpeed;
            if (inRange(0, Input.mousePosition.y, panMargin)) pos.y -= Time.deltaTime * panSpeed;
            if (inRange(Screen.width - panMargin, Input.mousePosition.x, Screen.width)) pos.x += Time.deltaTime * panSpeed;
            if (inRange(Screen.height - panMargin, Input.mousePosition.y, Screen.height)) pos.y += Time.deltaTime * panSpeed;
        }
    }

    private void LateUpdate()
    {
        cam.orthographicSize = size * currentZoom;
        // TODO: make sure camera is contained entirely in bounding box
        if (!boundingBox.bounds.Contains(pos))
        {
            pos = boundingBox.bounds.ClosestPoint(pos);
        }
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);

        if (Singleton.Instance.turnManager.GetLosingTeam() > -1)
        {
            Invoke(nameof(MainMenu), 2.5f);
            winCanvas.SetActive(true);
        }
    }

    public void MainMenu()
    {
        // TODO: use scene handler
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scene/MainMenu");
    }
}
