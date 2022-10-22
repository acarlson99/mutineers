using UnityEngine;

public class CannonFiringPin : MonoBehaviour
{
    public GameObject cannon;

    float maxX;
    float minX = -1.5f;

    private bool dragging = false;

    // Start is called before the first frame update
    void Start()
    {
        maxX = transform.localPosition.x;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public bool FiringPinMaxPull()
    {
        return Mathf.Abs(minX - transform.localPosition.x) <= 0.01f;
    }

    private void OnMouseDown()
    {
    }

    private void OnMouseDrag()
    {
        if (cannon.GetComponent<Cannon>().hasFired) return;

        dragging = true;
        var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        point.z = cannon.transform.position.z;
        var dir = cannon.transform.position - point;
        var t = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        cannon.transform.rotation = Quaternion.AngleAxis(t, Vector3.forward);

        var lpos = transform.localPosition;
        lpos.x = Mathf.Clamp(-1 * (cannon.transform.position - point).magnitude, minX, maxX);
        transform.localPosition = lpos;
    }

    private void OnMouseUp()
    {
        if (!dragging || cannon.GetComponent<Cannon>().hasFired) return;

        if (FiringPinMaxPull())
        {
            Debug.Log($"FIRE: {maxX} {transform.localPosition.x}");
            cannon.GetComponent<Cannon>().Fire();
        }
        var lpos = transform.localPosition;
        lpos.x = maxX;
        transform.localPosition = lpos;
    }
}
