using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RumFire : Exploder
{
    public Vector2 direction;
    public int moveSpeed = 4;
    public float triggerStayExplodeTime = 0.25f;

    public override string weaponName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    private float t;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        transform.position += (Vector3)direction * Time.deltaTime * moveSpeed;
        t += Time.deltaTime;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log($"COLLISION ENTER {collision.gameObject.name}");
        //ExplodeAt(collision.collider.ClosestPoint(transform.position));
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        //OnCollisionEnter2D(collision);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log($"TRIGGER ENTER {other.gameObject.name}");
        Vector2 p = other.ClosestPoint(transform.position);
        ExplodeAt(p);
        t = 0;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (t >= triggerStayExplodeTime)
            {
                t -= triggerStayExplodeTime;
                OnTriggerEnter2D(collision);
            }
        }
        else
        {
            //Destroy(gameObject);
        }
    }

    // TODO: make flames disappear
}
