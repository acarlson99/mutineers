using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Banana : Exploder
{

    public override string weaponName { get; set; } = "banana";
    public float slowMagnitude = 4;
    public float timeToExplode = 3;
    private float slowTime = 0;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (Input.GetMouseButtonDown((int)MouseButton.Left) && explosionEnabled)
        {
            ExplodeAndDestroy(rb.position);
        }

        if (!thrown) return;

        if (slowTime == 0 && rb.IsMovingSlowly(slowMagnitude))
        {
            StartCoroutine(spriteRenderer.LerpColor(Color.red, 2f / 5, true));
        }
        if (slowTime > 0 || rb.IsMovingSlowly(slowMagnitude))
        {
            slowTime += Time.deltaTime;
        }
        else
        {
            slowTime = 0;
        }
        if (slowTime >= timeToExplode)
        {
            ExplodeAndDestroy(rb.position);
        }
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
    }
}
