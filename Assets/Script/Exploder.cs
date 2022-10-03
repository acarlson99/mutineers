using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Exploder : MonoBehaviour
{
    //CircleCollider2D circleCollider;
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
        var lc = GetComponent<LaunchController>();
        if (lc) GetComponent<Rigidbody2D>().gravityScale = lc.gravityScale;
        //thrower = null;
    }

    private void OnDestroy()
    {
        // FIXME: this will bug if bomb throw cancelled
        if (thrower) thrower.SendMessage("EndBombThrow");

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
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
