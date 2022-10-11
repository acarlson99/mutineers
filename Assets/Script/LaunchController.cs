using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LaunchController : MonoBehaviour
{
    public float launchPower = 5f;
    public float launchMagnitudeClamp = 2.5f;

    [HideInInspector]
    public float gravityScale = 0f;
    [HideInInspector]
    public bool acceptingInput = false;

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
            StopDragging();
        }
    }

    private void OnMouseDown()
    {
        if (!acceptingInput) return;
        dragging = true;
    }

    public Vector2 MousePosToRelativePos(Vector3 mousePos)
    {
        return (Vector2)Camera.main.ScreenToWorldPoint(mousePos) - rb.position;
    }

    private void OnMouseDrag()
    {
        if (!acceptingInput || !dragging) return;
        DrawTrajectory(MousePosToRelativePos(Input.mousePosition));
    }

    private void OnMouseUp()
    {
        if (!acceptingInput || !dragging) return;
        dragging = false;
        LaunchWithVelocity(LaunchVelocity(MousePosToRelativePos(Input.mousePosition)));
    }

    // launch with velocity 
    public void LaunchWithVelocity(Vector2 velocity)
    {
        rb.gravityScale = gravityScale;
        AddLaunchForce(velocity);
        // disable input after launched successfully
        acceptingInput = false;
        StopDragging();

        // notify of launch
        Weapon e = GetComponent<Weapon>();
        if (e) e.SendMessage(nameof(e.NotifyOfLaunch), velocity);
        PirateController p = GetComponent<PirateController>();
        if (p) p.SendMessage(nameof(p.NotifyOfLaunch), velocity);
    }

    public void AddLaunchForce(Vector2 velocity)
    {
        //Debug.Log("AYAYA");
        rb.AddForce(velocity, ForceMode2D.Impulse);
    }

    private void StopDragging()
    {
        dragging = false;
#if UNITY_EDITOR
#else
        Singleton.Instance.lineRenderer.enabled = false;
#endif
    }

    // TODO: this should take mass into account
    public void DrawTrajectory(Vector3 relativePos)
    {
        Singleton.Instance.lineRenderer.enabled = true;
        Vector2 _velocity = LaunchVelocity(relativePos);
        int steps = (int)(Math.Sqrt(_velocity.SqrMagnitude()) * 100);
        List<Vector2> points = PlotTrajectory(rb.mass, gravityScale, rb.drag, (Vector2)transform.position, _velocity, steps);
        points.Insert(0, (Vector2)transform.position);
        points.Insert(0, (Vector2)transform.position + Vector2.ClampMagnitude((Vector2)relativePos, launchMagnitudeClamp));

        Singleton.Instance.lineRenderer.positionCount = points.Count;

        Vector3[] positions = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            positions[i] = points[i];
        }
        Singleton.Instance.lineRenderer.SetPositions(positions);
    }

    public List<Vector2> PlotTrajectory(float mass, float gravityScale, float rbdrag, Vector2 pos, Vector2 velocity, int steps)
    {
        List<Vector2> results = new List<Vector2>();

        float timestep = Time.fixedDeltaTime / Physics2D.velocityIterations;
        Vector2 gravityAccel = gravityScale * timestep * timestep * Physics2D.gravity;

        float drag = 1f - timestep * rbdrag;
        Vector2 moveStep = velocity / mass * timestep;

        for (int i = 0; i < steps; i++)
        {
            moveStep += gravityAccel;
            moveStep *= drag;
            pos += moveStep;
            results.Add(pos);
        }

        return results;
    }

    // get launch velocity from a relative position
    // launching from relativePos=(0,-1) with launchPower=2.0f
    // returns (0,2)
    // often called LaunchVelocity(MousePosToRelativePos(Input.mousePosition))
    public Vector2 LaunchVelocity(Vector2 relativePos)
    {
        relativePos = Vector2.ClampMagnitude(relativePos, launchMagnitudeClamp);
        return -relativePos * launchPower;
    }

    // FINDME: find this, important
    public bool DestroyIfOOB()
    {
        if (transform.position.y < Singleton.Instance.killzoneY)
        {
            StopDragging();
            Destroy(gameObject);
            return true;
        }
        return false;
    }
}
