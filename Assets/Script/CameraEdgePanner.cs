using Cinemachine;
using UnityEngine;

public class CameraEdgePanner : MonoBehaviour
{
    public Vector3 offset;
    public float currentZoom = 1.0f;
    public float zoomSpeed = 100f;
    public float minZoom = 0.5f;
    public float maxZoom = 2;
    public float panSpeed = 20f;
    public float panMargin = 30f;
    public float cameraFreemoveResetTime = 2f;
    public GameObject winCanvas;
    //public BoxCollider2D boundingBox;

    //public Vector2 minBound = new Vector2(-10f, -10f);
    //public Vector2 maxBound = new Vector2(10f, 10f);
    private CinemachineVirtualCamera vcam;
    private float size;
    private Vector2 pos;
    private Transform _f;
    private float timer;
    private Vector2 stillPos;

    private void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        size = vcam.m_Lens.OrthographicSize;
    }

    private void Update()
    {
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            currentZoom -= Time.deltaTime * scroll * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        }
        static bool inRange(float a, float b, float c) => a <= b && b <= c;
        pos = (Vector2)Camera.main.transform.position; // set pos to current cam pos (not vcam) to avoid pos infinitely scrolling
        if (inRange(0, Input.mousePosition.x, Screen.width) && inRange(0, Input.mousePosition.y, Screen.height))
        {
            if (inRange(Screen.height - panMargin, Input.mousePosition.y, Screen.height)) pos.y += Time.deltaTime * panSpeed;
            if (inRange(Screen.width - panMargin, Input.mousePosition.x, Screen.width)) pos.x += Time.deltaTime * panSpeed;
            if (inRange(0, Input.mousePosition.y, panMargin)) pos.y -= Time.deltaTime * panSpeed;
            if (inRange(0, Input.mousePosition.x, panMargin)) pos.x -= Time.deltaTime * panSpeed;
        }

        if (pos != (Vector2)Camera.main.transform.position)
        {
            if (vcam.Follow != null)
            {
                _f = vcam.Follow;
                vcam.Follow = null;
                timer = 0;
            }
        }
        else if (_f && vcam.Follow == null && stillPos == pos)
        {
            // TODO: fix strange behavior when turn ends while camera is freemoving
            if (timer >= cameraFreemoveResetTime)
            {
                vcam.Follow = _f;
                _f = null;
            }
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0f;
            stillPos = pos;
        }
    }

    private void LateUpdate()
    {
        vcam.m_Lens.OrthographicSize = size * currentZoom;
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
