using System.Collections;
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
    public float damageMultiplier = 1;

    public bool explosionEnabled = false;
    private bool hasExploded = false;

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionEnter2D(collision);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!explosionEnabled) return;
        ExplodeAndDestroy(collision.collider.ClosestPoint(transform.position));
    }

    public virtual void Explode(Vector2 explosionPos)
    {
        if (hasExploded) return;
        hasExploded = true; // FIXED: prevent double explosion
        ExplodeAt(explosionPos);
    }

    public virtual void ExplodeAt(Vector2 explosionPos)
    {
        var colliders = Physics2D.OverlapCircleAll(explosionPos, explosionRadius);
        foreach (var c in colliders)
        {
            if (c.gameObject == gameObject) continue; // no explode self
            var rb = c.GetComponent<Rigidbody2D>();
            if (!rb) continue;
            var v = rb.AddExpExplosionForce(explosionPos, explosionPower, upwardEffect, falloff);

            var p = c.GetComponent<IExplodable>();
            if (p == null) continue;
            p.DealExplosionDamage(v, damageMultiplier);
        }

        if (explosionSprite != null)
        {
            GameObject explosionCircle = new GameObject("explosion");
            explosionCircle.transform.position = (Vector3)explosionPos + explosionSpriteOffset;
            explosionCircle.transform.localScale = new Vector3(explosionRadius * 2, explosionRadius * 2, 1f);
            var spriteRenderer = explosionCircle.AddComponent<SpriteRenderer>();
            spriteRenderer.color = Color.red;
            spriteRenderer.sprite = explosionSprite;
            Destroy(explosionCircle, 1.0f);
        }
    }

    public virtual void ExplodeAndDestroy(Vector2 explosionPos)
    {
        Explode(explosionPos);
        Destroy(gameObject);
        NotifyThrowerEndWeaponUse();
    }

    public virtual void ExplodeWithDelay(Vector2 explosionPos, float t)
    {
        this.ScheduleFuncall((e) => { ExplodeAndDestroy(e); return 1; }, explosionPos, t);
    }

    public virtual void ExplodeWithDelay(GameObject o, float t)
    {
        this.ScheduleFuncall((ob) => { ExplodeAndDestroy(ob.transform.position); return 1; }, o, t);
    }

    public override void NotifyOfLaunch(Vector2 velocity)
    {
        base.NotifyOfLaunch(velocity);
        explosionEnabled = true;
    }
}
