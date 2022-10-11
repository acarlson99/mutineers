using UnityEngine;

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

        if (rb.IsMovingSlowly(0.05f))
        {
            if (!areaTriggerActive)
            {
                NotifyThrowerEndWeaponUse(); // first time only
                // TODO: refactor sprite rendering layers
                transform.position = transform.position + new Vector3(0, 0, 1f);
            }
            areaTriggerActive = true;
        }

        if (!areaTriggerActive) return;
        else triggerArea.SetActive(true);

        if (aboutToExplode) return;

        // TODO: likely making the triggerArea a trigger zone would be more efficient
        Collider2D[] objs = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D o in objs)
        {
            if (o.gameObject == gameObject) continue;

            var orb = o.GetComponent<Rigidbody2D>();
            if (orb && orb != rb && orb.velocity.magnitude > 1)
            {
                ExplodeWithDelay(gameObject, 2f);
                aboutToExplode = true;
                StartCoroutine(spriteRenderer.LerpColor(new Color(1f, 0, 0, 0.75f), 2f / 5, true));
                break;
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Destroy(triggerArea);
    }

    public override void NotifyOfLaunch(Vector2 velocity)
    {
        base.NotifyOfLaunch(velocity);
        explosionEnabled = false; // SMELL: resetting var set in previous call
    }
}
