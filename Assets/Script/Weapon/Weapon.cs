using UnityEngine;

// TODO: factory prolly
public enum EWeaponType
{
    CherryBomb,
    Dynamite,
    Banana,
    Crate,
    Gunpowder,
    Mine,
    Parachute,
    Po8,
    Rum,
    Seagull,
    Voodoo,
    TidalWave,
}

public abstract class Weapon : MonoBehaviour
{
    public string WeaponName { get { return WeaponType.ToString(); } }
    public abstract EWeaponType WeaponType { get; }
    public int weaponCount = 1;

    [HideInInspector]
    public WeaponController thrower; // TODO: rename to `controller`
    //public PirateController thrower;
    [HideInInspector]
    protected Rigidbody2D rb;
    [HideInInspector]
    protected LaunchController lc;
    [HideInInspector]
    protected bool thrown = false;
    public bool Thrown { get { return thrown; } }

    protected bool isTrigger = false;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        lc = GetComponent<LaunchController>();
        if (lc)
        {
            lc.gravityScale = rb.gravityScale;
            rb.gravityScale = 0f;
        }

        var collider = GetComponent<Collider2D>();
        if (collider) isTrigger = collider.isTrigger;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        var collider = GetComponent<Collider2D>();
        if (collider && !isTrigger)
        {
            if (!thrown) collider.isTrigger = true;
            else collider.isTrigger = false;
        }
        // wake up to enable OnCollisionEnter OnCollisionStay
        if (rb && rb.IsSleeping()) rb.WakeUp();
        GetComponent<LaunchController>()?.DestroyIfOOB();
    }

    public void NotifyThrowerEndWeaponUse()
    {
        Debug.Log($"Notifying thrower {thrower}");
        thrower?.EndWeaponUse();
        //if (thrower != null) thrower.SendMessage(nameof(thrower.EndWeaponUse));
        thrower = null;
    }

    public virtual void NotifyOfLaunch(Vector2 velocity)
    {
        thrown = true;
    }
}
