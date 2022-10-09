using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public abstract string weaponName { get; set; }

    [HideInInspector]
    public PirateController thrower;
    [HideInInspector]
    protected Rigidbody2D rb;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        if (thrower) thrower.SendMessage(nameof(thrower.EndWeaponUse));
    }

    abstract public void NotifyOfLaunch(Vector2 velocity);
}
