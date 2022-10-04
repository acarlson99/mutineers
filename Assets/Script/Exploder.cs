using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Exploder : MonoBehaviour
{
    public Sprite explosionSprite;
    public float explosionRadius = 3f;
    public float explosionPower = 1f;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // TODO: if thrown against object it bounces off it and then explodes
        if (!explosionEnabled) return;

        explosionEnabled = false; // FIXED: prevent double explosion

        Vector2 collisionPos = collision.collider.ClosestPoint(transform.position);

        GameObject explosionCircle = new GameObject("explosion");
        explosionCircle.transform.position = collisionPos;
        explosionCircle.transform.localScale = new Vector3(explosionRadius * 2, explosionRadius * 2, 1f);
        var spriteRenderer = explosionCircle.AddComponent<SpriteRenderer>();
        spriteRenderer.color = Color.red;
        spriteRenderer.sprite = explosionSprite;

        var colliders = Physics2D.OverlapCircleAll(collisionPos, explosionRadius);
        foreach (var c in colliders)
        {
            //if (!c.CompareTag("Player")) continue;
            var rb = c.GetComponent<Rigidbody2D>();
            if (!rb) continue;
            rb.AddExpExplosionForce(rb.ClosestPoint(collisionPos), 250, 50, 1.2f);
        }
        Destroy(gameObject);
        Destroy(explosionCircle, 1.0f);
    }
}
