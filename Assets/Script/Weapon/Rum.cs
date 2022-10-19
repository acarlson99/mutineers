using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rum : Exploder
{
    public GameObject fire;
    public override string weaponName { get; set; } = "rum";
    public override EWeaponType weaponType { get; set; } = EWeaponType.Rum;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!explosionEnabled) return;
        explosionEnabled = false;
        ExplodeAndDestroy(collision, collision.collider.ClosestPoint(transform.position));
    }

    public void ExplodeAndDestroy(Collision2D collision, Vector2 explosionPos)
    {
        base.ExplodeAndDestroy(explosionPos);
        foreach (ContactPoint2D contact in collision.contacts)
        {
            var norm = contact.normal;
            if (norm == new Vector2(0, 1)) // pointing up
            {
                // spawn fire
                Vector2[] dirs = { Vector2.left, Vector2.right };
                foreach (Vector2 dir in dirs)
                {
                    //var fab = Instantiate(fire, transform.position - new Vector3(0, 0, 0.5f) + (Vector3)dir*2, Quaternion.identity);
                    var fab = Instantiate(fire, transform.position - new Vector3(0, 0, 0.5f) + (Vector3)dir * explosionRadius / 2, Quaternion.identity);
                    fab.GetComponent<RumFire>().direction = dir;
                }
                break;
            }
        }
    }

    public override void ExplodeAndDestroy(Vector2 explosionPos)
    {
        base.ExplodeAndDestroy(explosionPos);
    }
}
