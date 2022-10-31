using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PirateController : MonoBehaviour, IExplodable
{
    public GameObject menu;
    //public GameObject bombObject;
    public int teamNum;

    [SerializeField]
    public List<STRINT> Ilist;
    public bool isCaptain = false;
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
        inventory = new Inventory(Ilist);
        if (inventory.Count() == 0)
        {
            Debug.LogWarning("Inventory empty");
            foreach (EWeaponType wt in Enum.GetValues(typeof(EWeaponType)))
            {
                inventory.Add(wt, -1);
            }
        }
        launchController = GetComponent<LaunchController>();
        launchController.acceptingInput = false;
        rb = GetComponent<Rigidbody2D>();
        PriateSpriteInit();

        Singleton.Instance.turnManager.PlayerRegister(teamNum);
    }

    [ContextMenu("Pirate Sprite Init")]
    void PriateSpriteInit()
    {
        if (isCaptain)
        {
            GetComponent<SpriteRenderer>().sprite = Singleton.Instance.captainSprite;
        }
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

    private void Die()
    {
        Destroy(gameObject);
    }

    public void DealExplosionDamage(Vector2 f, float damageMultiplier)
    {
        // called with force vector representing launch angle
        Debug.Log("Explosion Damage " + gameObject.name + ' ' + f.magnitude);

        healthBar.currentHealth -= f.magnitude * damageMultiplier;
        Debug.Log($"Dealt {f.magnitude} damage");

        if (healthBar.currentHealth <= 0) Die();
    }

    public void DealRawDamage(float damage)
    {
        Debug.Log($"Raw damage {name} {damage}");
        healthBar.currentHealth -= damage;
        if (healthBar.currentHealth <= 0) Die();
    }
}
