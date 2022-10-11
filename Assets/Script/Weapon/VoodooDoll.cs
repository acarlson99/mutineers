using Unity.VisualScripting;
using UnityEngine;

public class VoodooDoll : Weapon
{
    public override string weaponName { get; set; } = "voodoo doll";

    public float slowMagnitude = 4;
    public float timeToDestroy = 3;

    private float slowTime = 0;

    private PirateController targetPirate;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        // FIXME: cleanup, this is janky
        if (targetPirate == null && Input.GetMouseButtonDown((int)MouseButton.Left))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                Debug.Log($"{hit} {hit.collider} {hit.rigidbody} {hit.collider.gameObject.name}");
                PirateController pc = hit.collider?.gameObject?.GetComponent<PirateController>();
                if (pc) targetPirate = pc;
            }
        }
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (targetPirate == null)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.5f);
            Singleton.Instance.Vcam.Follow = null;
            lc.acceptingInput = false; // turn off launch input until target selected
        }
        else if (!thrown)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
            Singleton.Instance.Vcam.Follow = transform;
            lc.acceptingInput = true;
        }
        if (thrown && rb.IsMovingSlowly(slowMagnitude))
        {
            slowTime += Time.deltaTime;
        }
        if (slowTime >= timeToDestroy) Destroy(gameObject);
    }

    public override void NotifyOfLaunch(Vector2 velocity)
    {
        base.NotifyOfLaunch(velocity);
        slowTime = 0;
        Singleton.Instance.Vcam.Follow = targetPirate.transform;
        targetPirate.GetComponent<LaunchController>().AddLaunchForce(velocity);
    }
}
