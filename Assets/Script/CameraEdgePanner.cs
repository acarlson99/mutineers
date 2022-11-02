using Cinemachine;
using UnityEngine;

public class CameraEdgePanner : MonoBehaviour
{
    public Vector3 offset;
    public float currentZoom = 1.0f;
    public float zoomSensitivity = 0.25f;
    public float minCamSize = 2;
    public float maxCamSize = 10;
    //public float maxCamSize = 8;
    public float panSpeed = 20f;
    public float panMargin = 30f;
    public float cameraFreemoveResetTime = 2f;
    public GameObject winCanvas;

    private CinemachineVirtualCamera vcam;
    private float size;
    private Vector2 pos;
    private float timer;
    private bool freemoveMode = false;

    private void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        size = vcam.m_Lens.OrthographicSize;
        vcam.GetComponent<CinemachineConfiner2D>().m_MaxWindowSize = maxCamSize;

        if (minCamSize > maxCamSize)
        {
            Debug.LogError($"min cam size {minCamSize} greater than max cam size {maxCamSize}");
        }
    }

    private float ClampZoom(float z, float min, float max)
    {
        // clamp zoom
        if (size * z > max)
        {
            z -= ((size * z) - max) / size;
        }
        if (size * z < min)
        {
            z += (min - (size * z)) / size;
        }
        return z;
    }

    private void Update()
    {
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            var deltaZoom = scroll * zoomSensitivity * Mathf.Lerp(minCamSize, maxCamSize, 4);
            //Debug.Log($"{scroll} * {zoomSensitivity} * {Mathf.Lerp(minCamSize, maxCamSize, 4)} = {deltaZoom}");
            currentZoom -= deltaZoom;
        }
        var z = ClampZoom(currentZoom, minCamSize, maxCamSize);
        //if (z != currentZoom) Debug.Log($"{currentZoom} clamped to {z}");
        currentZoom = z;
        static bool inRange(float a, float b, float c) => a <= b && b <= c;
        pos = (Vector2)Camera.main.transform.position; // set pos to current cam pos (not vcam) to avoid pos infinitely scrolling
        if (inRange(0, Input.mousePosition.x, Screen.width) && inRange(0, Input.mousePosition.y, Screen.height))
        {
            float v = Time.deltaTime * panSpeed * currentZoom;
            if (inRange(Screen.height - panMargin, Input.mousePosition.y, Screen.height)) pos.y += v;
            if (inRange(Screen.width - panMargin, Input.mousePosition.x, Screen.width)) pos.x += v;
            if (inRange(0, Input.mousePosition.y, panMargin)) pos.y -= v;
            if (inRange(0, Input.mousePosition.x, panMargin)) pos.x -= v;
        }

        if (pos == (Vector2)Camera.main.transform.position)
        {
            timer += Time.deltaTime;
            if (freemoveMode && timer >= cameraFreemoveResetTime)
            {
                Singleton.Instance.CamQuietRefollow();
                freemoveMode = false;
            }
        }
        else if (pos != (Vector2)Camera.main.transform.position)
        {
            timer = 0;
            if (vcam.Follow)
            {
                Singleton.Instance.CamQuietUnfollow(); // TODO: move CamQuietUnfollow to this class
                freemoveMode = true; // TODO: set this anytime CamQuietUnfollow is called
            }
        }
    }

    private void LateUpdate()
    {
        vcam.m_Lens.OrthographicSize = Mathf.Clamp(size * currentZoom, minCamSize, maxCamSize);
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);

        // TODO: likely this should live in TurnManager
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
