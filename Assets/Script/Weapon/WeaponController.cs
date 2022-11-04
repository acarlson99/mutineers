using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public WeaponController() { }

    PirateController p;
    GameObject pgo;
    Weapon w;
    int n;
    int nSum;  // num summoned
    int nDest; // num destroyed
    GameObject lastThrown;
    public bool destroyIfPirateDead = true;

    public void Update()
    {
        if (n > 0 && pgo.IsDestroyed())
        {
            // TODO: move EndWeaponUse and OnDestroy UpdateState calls from PirateController to this class
            p = null;
            pgo = null;
            if (destroyIfPirateDead)
            {
                Singleton.Instance.CamFollow(null);
                Destroy(lastThrown);
            }
            w = null;
            lastThrown = null;
            n = 0;
            nSum = 0;
            nDest = 0;
            return;
        }
        if (lastThrown != null)
        {
            if (!lastThrown.GetComponent<Weapon>().Thrown)
            {
                lastThrown.transform.position = p.transform.position - new Vector3(0, -0.25f, 1);
            }
        }
    }

    // return false if unable to use weapon
    //public bool BeginWeaponUse(GameObject pirate, Weapon weapon, int count)
    public bool BeginWeaponUse(PirateController pirate, Weapon weapon, int count)
    {
        if (!pirate.inventory.UseItem(weapon.WeaponType)) return false;

        p = pirate;
        p.BeginBombThrow(weapon.gameObject);
        pgo = p.gameObject;
        Debug.Log("Begin weapon use");
        w = weapon;
        nSum = 0;
        nDest = 0;
        this.n = count;
        destroyIfPirateDead = true;
        Debug.Log($"{p} {w} {nSum} {nDest} {count}");
        summonWeapon();
        return true;
    }

    private void summonWeapon()
    {
        Debug.Log($"Summon weapon {p} {w} {p.gameObject} {p.gameObject.IsDestroyed()}");
        nSum++;
        if (nSum > n)
        {
            Debug.LogWarning($"WARN: {MethodBase.GetCurrentMethod().Name} n:{n} num summoned:{nSum} num destroyed:{nDest}");
            return;
        }
        var fab = Instantiate(w.gameObject);
        if (w.GetComponent<LaunchController>())
            Physics2D.IgnoreCollision(p.GetComponent<Collider2D>(), fab.GetComponent<Collider2D>());
        fab.transform.position = p.transform.position - new Vector3(0, 0, 1); // move into foreground
        var lc = fab.GetComponent<LaunchController>();
        if (lc) lc.acceptingInput = true;
        fab.GetComponent<Weapon>().thrower = this;
        //lastThrown = fab.GetComponent<Weapon>();
        lastThrown = fab;

        Debug.Log("Begin bomb throw follow");
        Singleton.Instance.CamFollow(fab.transform);
    }

    public void EndWeaponUse()
    {
        Debug.Log("End weapon use weaponcontroller");
        nDest++;
        if (nDest > nSum || nDest > n)
        {
            nDest--;
            Debug.LogWarning($"WARN: {MethodBase.GetCurrentMethod().Name} n:{n} num summoned:{nSum} num destroyed:{nDest}");
        }
        if (nDest == n)
        {
            if (p) p.EndWeaponUse();
            Singleton.Instance.CamFollow(null);
        }
        else
        {
            summonWeapon();
        }
    }
}
