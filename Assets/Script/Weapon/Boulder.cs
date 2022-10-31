using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : Weapon
{
    public override EWeaponType WeaponType { get; } = EWeaponType.Boulder;

    private TimerMux<int> timerMux;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        timerMux = new TimerMux<int>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        timerMux.Update(Time.deltaTime);

        if (!thrown) return;

        if (rb.IsMovingSlowly(0.04f))
        {
            if (thrower)
            {
                NotifyThrowerEndWeaponUse();
                Destroy(gameObject, 2);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Player") return;

        var id = collision.gameObject.GetInstanceID();
        if (timerMux.ContainsKey(id))
        {
            if (timerMux[id] < 0.5f) return;
            timerMux[id] -= 0.5f;
        }
        else timerMux.Add(id);
        collision.gameObject.GetComponent<PirateController>().DealRawDamage(100);
    }
}
