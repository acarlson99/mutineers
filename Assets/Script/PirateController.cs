using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// negative number means infinity
public class Inventory : Dictionary<EWeaponType, int>
{
    public Inventory(IEnumerable<STRINT> ls)
    {
        //Debug.Log($"Inventory init with {ls}");
        foreach (var item in ls)
        {
            //Debug.Log($"ADD ITEM {item.i} {item.s}");
            if (item.i > 0) Add(item.s, item.i);
            else if (item.i < 0) Add(item.s, -1);
        }
    }

    // mark item as used, return false if unable
    public bool UseItem(EWeaponType name)
    {
        if (CanUseItem(name))
        {
            if (this[name] > 0) this[name] -= 1;
            if (this[name] == 0)
            {
                Remove(name);
            }
            return true;
        }
        return false;
    }

    public void Add(EWeaponType name)
    {
        if (!ContainsKey(name)) Add(name, 1);
        else Add(name, this[name] + 1);
    }

    public bool CanUseItem(EWeaponType name)
    {
        return ContainsKey(name) && this[name] != 0;
    }
}

[Serializable]
public struct STRINT
{
    public EWeaponType s;
    public int i;
}

public class PirateController : MonoBehaviour, IExplodable
{
    public GameObject menu;
    //public GameObject bombObject;
    public int teamNum;

    [SerializeField]
    public List<STRINT> Ilist; // TODO: find way of making this work
    public Inventory inventory;

    [HideInInspector]
    public bool throwMode = false;
    [HideInInspector]
    public bool bombThrowMode = false;
    [HideInInspector]
    public Weapon lastThrown = null;

    private LaunchController launchController;
    private Rigidbody2D rb;
    public HealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        this.inventory = new Inventory(Ilist);
        Debug.Log($"Created inventory {this.inventory}");
        launchController = GetComponent<LaunchController>();
        launchController.acceptingInput = false;
        rb = GetComponent<Rigidbody2D>();
        PriateSpriteInit();

        Singleton.Instance.turnManager.PlayerRegister(teamNum);
    }

    [ContextMenu("Pirate Sprite Init")]
    void PriateSpriteInit()
    {
        if (teamNum != 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 1, 1);
        }

    }

    // Update is called once per frame
    void Update()
    {
        // NOTE: this destroys the object sometimes
        // FINDME: find this, important
        // destroy if out of bounds
        launchController.DestroyIfOOB();
        if (throwMode)
        {
            launchController.acceptingInput = true;
            bombThrowMode = false;
        }
        if (bombThrowMode)
        {
            launchController.acceptingInput = false;
            throwMode = false;
        }
        if (IsActiveTurn())
        {
            if (IsSelectedPirate()) transform.position = new Vector3(transform.position.x, transform.position.y, -1);
            else transform.position = new Vector3(transform.position.x, transform.position.y, -0.5f);
        }
        else transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }

    public bool IsActiveTurn()
    {
        // returns if it is this pirate's team's turn
        return Singleton.Instance.turnManager.turnNum == teamNum;
    }

    public bool IsSelectedPirate()
    {
        // returns whether turnmanager has selected this pirate
        return Singleton.Instance.turnManager.selectedBoy == gameObject;
    }

    private void OnDestroy()
    {
        Singleton.Instance.turnManager.PlayerUnregister(teamNum);
        if (IsSelectedPirate())
        {
            Singleton.Instance.turnManager.UpdateState(TurnState.End);
            Singleton.Instance.CamFollow(null);
        }
    }

    void OnMouseUpAsButton()
    {
        if (!IsActiveTurn()) return;
        Debug.Log("Click pirate " + gameObject.name);
        if (throwMode || bombThrowMode) return;

        menu.GetComponent<PlayerMenuController>().OpenMenuForPirate(gameObject);
    }

    public void BeginPlayerThrow()
    {
        throwMode = true;

        Singleton.Instance.CamFollow(transform);
    }

    public void NotifyOfLaunch(Vector2 velocity)
    {
        EndPlayerThrow();
    }

    public void EndPlayerThrow()
    {
        if (!Singleton.Instance.turnManager.UpdateState(TurnState.Thrown))
            Debug.LogWarning("BeginPlayerThrow, turnmanager");

        throwMode = false;
        launchController.acceptingInput = false;

        Singleton.Instance.CamFollow(transform);
    }

    public void BeginBombThrow(GameObject bombObject)
    {
        if (!Singleton.Instance.turnManager.UpdateState(TurnState.BombSummoned))
            Debug.LogWarning("EndWeaponUse, turnmanager");

        bombThrowMode = true;
    }

    public void EndWeaponUse()
    {
        Debug.Log("End weapon use");
        if (!Singleton.Instance.turnManager.UpdateState(TurnState.BombThrown))
            Debug.LogWarning("EndWeaponUse, turnmanager");

        bombThrowMode = false;
    }

    public void DealExplosionDamage(Vector2 f, float damageMultiplier)
    {
        // called with force vector representing launch angle
        Debug.Log("Explosion Damage " + gameObject.name + ' ' + f.magnitude);

        healthBar.currentHealth -= f.magnitude * damageMultiplier;
        Debug.Log($"Dealt {f.magnitude} damage");

        if (healthBar.currentHealth <= 0) Destroy(gameObject);

    }
}
