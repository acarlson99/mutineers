using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dynamite : Exploder
{
    private Vector3 lastPosition;
    public override string explosiveName { get; set; } = "cherry bomb";

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        lastPosition = transform.position;
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        if (transform.position == lastPosition)
        {
            if (!explosionEnabled) return;

            explosionEnabled = false; // FIXED: prevent double explosion
            Explode(collision.collider.ClosestPoint(transform.position));
        }
    }
}
