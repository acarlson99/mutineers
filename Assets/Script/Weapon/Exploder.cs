using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Exploder : Weapon
{
    // send explosion into background
    public Vector3 explosionSpriteOffset = new Vector3(0, 0, 1);
    public Sprite explosionSprite;
    public float explosionRadius = 3f;
    public float explosionPower = 250f;
    public float upwardEffect = 1; // send upwards for effect (0 to disable)
    public float falloff = 1.2f; // explosion power weakens at distance

    [HideInInspector]
    public bool explosionEnabled = false;

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionEnter2D(collision);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!explosionEnabled) return;

        explosionEnabled = false; // FIXED: prevent double explosion
        Explode(collision.collider.ClosestPoint(transform.position));
    }

    public virtual void Explode(Vector2 explosionPos)
    {
        GameObject explosionCircle = new GameObject("explosion");
        explosionCircle.transform.position = (Vector3)explosionPos + explosionSpriteOffset;
        explosionCircle.transform.localScale = new Vector3(explosionRadius * 2, explosionRadius * 2, 1f);
        var spriteRenderer = explosionCircle.AddComponent<SpriteRenderer>();
        spriteRenderer.color = Color.red;
        spriteRenderer.sprite = explosionSprite;

        var colliders = Physics2D.OverlapCircleAll(explosionPos, explosionRadius);
        foreach (var c in colliders)
        {
            var rb = c.GetComponent<Rigidbody2D>();
            if (!rb) continue;
            var v = rb.AddExpExplosionForce(explosionPos, explosionPower, upwardEffect, falloff);

            var p = c.GetComponent<PirateController>();
            if (!p) continue;
            p.DealExplosionDamage(v);
        }
        Destroy(gameObject);
        Destroy(explosionCircle, 1.0f);
    }

    public override void NotifyOfLaunch(Vector2 velocity)
    {
        // TODO: merge this var with VoodooDoll.thrown
        explosionEnabled = true;
    }
}
