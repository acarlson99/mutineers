using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Exploder : MonoBehaviour
{
    public Sprite explosionSprite;
    public float explosionRadius = 3f;
    public float explosionPower = 250f;
    public float upwardEffect = 1; // send upwards for effect (0 to disable)
    public float falloff = 1.2f; // explosion power weakens at distance

    [HideInInspector]
    public bool explosionEnabled = false;
    [HideInInspector]
    public GameObject thrower;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnMouseUp()
    {
    }

    private void OnDestroy()
    {
        if (thrower) thrower.SendMessage("EndBombThrow");

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionEnter2D(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!explosionEnabled) return;

        explosionEnabled = false; // FIXED: prevent double explosion
        Explode(collision.collider.ClosestPoint(transform.position));
    }

    public Vector3 spriteOffset = new Vector3(0, 0, 1);

    public void Explode(Vector2 explosionPos)
    {
        GameObject explosionCircle = new GameObject("explosion");
        explosionCircle.transform.position = (Vector3)explosionPos + spriteOffset;
        explosionCircle.transform.localScale = new Vector3(explosionRadius * 2, explosionRadius * 2, 1f);
        var spriteRenderer = explosionCircle.AddComponent<SpriteRenderer>();
        spriteRenderer.color = Color.red;
        spriteRenderer.sprite = explosionSprite;

        var colliders = Physics2D.OverlapCircleAll(explosionPos, explosionRadius);
        foreach (var c in colliders)
        {
            //if (!c.CompareTag("Player")) continue;
            var rb = c.GetComponent<Rigidbody2D>();
            if (!rb) continue;
            var v = rb.AddExpExplosionForce(explosionPos, explosionPower, upwardEffect, falloff);

            var p = c.GetComponent<PirateController>();
            if (!p) continue;
            p.DealExplosionDamage(v);
            //p.DealExplosionDamage(rb.GetExplosionForceVector2D(explosionPos, explosionPower, falloff));
        }
        Destroy(gameObject);
        Destroy(explosionCircle, 1.0f);
    }

    public void EndLaunch()
    {
        explosionEnabled = true;
    }
}
