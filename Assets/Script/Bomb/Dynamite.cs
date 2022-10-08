using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dynamite : Exploder
{
    public override string explosiveName { get; set; } = "dynamite";

    public float slowMagnitude = 4;
    public float timeToExplode = 3;

    private float slowTime = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        // Tick if slow enough or already ticking
        if (slowTime > 0 || rb.velocity.magnitude <= slowMagnitude)
        {
            slowTime += Time.deltaTime;
        }
        else
        {
            slowTime = 0;
        }
        base.Update();
    }

    // Explode after duration if slow enough
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (slowTime >= timeToExplode)
        {
            base.OnCollisionEnter2D(collision);
        }
        else
        {
            Debug.Log($"{rb.velocity.magnitude} {slowTime} insufficient");
        }
    }

    public override void Launch()
    {
        slowTime = 0;
        base.Launch();
    }
}
