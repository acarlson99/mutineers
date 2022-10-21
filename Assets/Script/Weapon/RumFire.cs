using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RumFire : Exploder
{
    public Vector2 direction;
    public int moveSpeed = 4;
    public float triggerStayExplodeTime = 0.25f;

    public override EWeaponType WeaponType { get => throw new System.NotImplementedException(); }

    private TimerMux<int> timerMux = new TimerMux<int>();

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        explosionSpriteOffset = new Vector3(0, 0, -1);
        timerMux.Add(0);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        transform.position += (Vector3)direction * Time.deltaTime * moveSpeed;
        timerMux.Update(Time.deltaTime);

        var l = new List<ContactPoint2D>();
        if (rb.GetContacts(l) == 0 && timerMux[0] >= 0.1f)
        {
            Destroy(gameObject);
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision) { }

    protected override void OnCollisionStay2D(Collision2D collision) { }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log($"TRIGGER ENTER {other.gameObject.name}");
        if (other.tag != "Player") return;
        Vector2 p = other.ClosestPoint(transform.position);
        ExplodeAt(p);
        timerMux.Add(other.GetInstanceID());
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            var t = timerMux[collision.GetInstanceID()];
            if (t >= triggerStayExplodeTime)
            {
                timerMux[collision.GetInstanceID()] -= triggerStayExplodeTime;
                OnTriggerEnter2D(collision);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        timerMux.Remove(collision.GetInstanceID());
    }
}
