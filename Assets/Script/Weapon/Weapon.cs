using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public abstract string weaponName { get; set; }

    [HideInInspector]
    public PirateController thrower;
    [HideInInspector]
    protected Rigidbody2D rb;
    [HideInInspector]
    protected LaunchController lc;
    [HideInInspector]
    protected bool thrown = false;

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
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        // wake up to enable OnCollisionEnter OnCollisionStay
        if (rb.IsSleeping()) rb.WakeUp();
        GetComponent<LaunchController>()?.DestroyIfOOB();
    }

    protected virtual void OnDestroy()
    {
        NotifyThrowerEndWeaponUse();
    }

    protected void NotifyThrowerEndWeaponUse()
    {
        Debug.Log($"Notifying thrower {thrower}");
        if (thrower) thrower.SendMessage(nameof(thrower.EndWeaponUse));
        thrower = null;
    }

    public virtual void NotifyOfLaunch(Vector2 velocity)
    {
        thrown = true;
    }
}
