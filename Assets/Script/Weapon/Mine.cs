using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: make mine heavier, increase mass, this requires changes to LaunchController plotTrajectory
public class Mine : Exploder
{
    public override string weaponName { get; set; } = "mine";

    private GameObject triggerArea;
    private SpriteRenderer spriteRenderer;
    private bool areaTriggerActive = false;
    private bool aboutToExplode = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        triggerArea = new GameObject("mineTriggerArea");
        spriteRenderer = triggerArea.AddComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(1f, 0.2f, 0.2f, 0.1f);
        spriteRenderer.sprite = explosionSprite;
        triggerArea.SetActive(false);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // if mine zone active
        triggerArea.transform.position = transform.position + new Vector3(0, 0, 0.1f);
        triggerArea.transform.localScale = new Vector3(explosionRadius * 2, explosionRadius * 2, 1f);

        if (!thrown) return;

        if (rb.velocity.magnitude <= 0.1)
        {
            if (!areaTriggerActive) NotifyThrowerEndWeaponUse(); // first time only
            areaTriggerActive = true;
        }

        if (!areaTriggerActive) return;
        else triggerArea.SetActive(true);

        if (aboutToExplode)
        {
            //spriteRenderer.color = new Color(1f, 0, 0, 0.4f); // TODO: blink
            return;
        }
        // TODO: likely making the triggerArea a trigger zone would be more efficient
        Collider2D[] objs = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D o in objs)
        {
            if (o.gameObject == gameObject)
            {
                continue;
            }
            var orb = o.GetComponent<Rigidbody2D>();
            if (orb && orb != rb && orb.velocity.magnitude > 1)
            {
                ExplodeWithDelay(gameObject, 2f);
                aboutToExplode = true;
                StartCoroutine(LerpColor(spriteRenderer.color, new Color(1f, 0, 0, 0.75f), 2f / 5, spriteRenderer, true));
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Destroy(triggerArea);
    }

    // https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/
    // TODO: this should not live here
    IEnumerator LerpColor(Color start, Color end, float duration, SpriteRenderer sprite, bool repeat = false)
    {
        float time = 0;
        while (time < duration)
        {
            if (!sprite?.gameObject) break;
            sprite.color = Color.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        if (sprite?.gameObject && repeat) yield return LerpColor(end, start, duration, sprite);
    }

    public override void NotifyOfLaunch(Vector2 velocity)
    {
        base.NotifyOfLaunch(velocity);
        explosionEnabled = false; // SMELL: resetting var set in previous call
    }
}
