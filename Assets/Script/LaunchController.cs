using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class LaunchController : MonoBehaviour
{
    public float launchPower = 5f;
    public Action deselectCB = () => { };

    [HideInInspector]
    public float gravityScale = 0f;

    private bool dragging = false;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (gravityScale == 0f) gravityScale = rb.gravityScale;
    }

    void Update()
    {
        DestroyIfOOB();
        // deselect unit on rightclick
        if (Input.GetMouseButton((int)MouseButton.Right))
        {
            Deselect();
        }
    }

    private void OnMouseDown()
    {
        if (!enabled) return;
        dragging = true;
    }

    private void OnMouseDrag()
    {
        if (!enabled || !dragging) return;
        dragging = true;
        DrawTrajectory();
    }

    private void OnMouseUp()
    {
        if (!enabled || !dragging) return;
        rb.AddForce(LaunchVelocity(), ForceMode2D.Impulse);
        rb.gravityScale = gravityScale;
        Deselect();
        enabled = false;

        // if attatched to a bomb make it explodable
        // TODO: this is messy, refactor
        Exploder e = GetComponent<Exploder>();
        if (e) e.explosionEnabled = true;
    }

    private void Deselect()
    {
        dragging = false;
        deselectCB();
#if UNITY_EDITOR
#else
        Singleton.Instance.lineRenderer.enabled = false;
#endif
    }

    public void DrawTrajectory()
    {
        Singleton.Instance.lineRenderer.enabled = true;
        Vector2 _velocity = LaunchVelocity();
        int steps = (int)(Math.Sqrt(_velocity.SqrMagnitude()) * 100);
        List<Vector2> points = PlotTrajectory(gravityScale, rb.drag, (Vector2)transform.position, _velocity, steps);
        points.Insert(0, ((Vector2)transform.position));

        Singleton.Instance.lineRenderer.positionCount = points.Count;

        Vector3[] positions = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            positions[i] = points[i];
        }
        Singleton.Instance.lineRenderer.SetPositions(positions);
    }

    public List<Vector2> PlotTrajectory(float gravityScale, float rbdrag, Vector2 pos, Vector2 velocity, int steps)
    {
        List<Vector2> results = new List<Vector2>();

        float timestep = Time.fixedDeltaTime / Physics2D.velocityIterations;
        Vector2 gravityAccel = gravityScale * timestep * timestep * Physics2D.gravity;

        float drag = 1f - timestep * rbdrag;
        Vector2 moveStep = velocity * timestep;

        for (int i = 0; i < steps; i++)
        {
            moveStep += gravityAccel;
            moveStep *= drag;
            pos += moveStep;
            results.Add(pos);
        }

        return results;
    }

    private Vector2 LaunchVelocity()
    {
        if (!rb) return Vector2.zero;
        Vector2 pos = rb.position;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 v = (Vector2)mouseWorldPos - pos;
        // TODO: parameterize
        v = Vector2.ClampMagnitude(v, 8);
        return -v * launchPower;
    }

    // FINDME: find this, important
    public bool DestroyIfOOB()
    {
        if (transform.position.y < Singleton.Instance.killzoneY)
        {
            Deselect();
            Destroy(gameObject);
            return true;
        }
        return false;
    }
}
