using UnityEngine;

public class PirateController : MonoBehaviour
{
    public GameObject menu;
    //public GameObject bombObject;
    public int teamNum;

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
        launchController = GetComponent<LaunchController>();
        launchController.acceptingInput = false;
        rb = GetComponent<Rigidbody2D>();
        //launchController.deselectCB = () =>
        //{
        //    launchController.enabled = false;
        //    throwMode = false;
        //};

        Singleton.Instance.turnManager.PlayerRegister(teamNum);
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
            Singleton.Instance.vcam.Follow = null;
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

        Singleton.Instance.vcam.Follow = transform;
    }

    public void NotifyOfLaunch(Vector2 velocity)
    {
        //var players = GameObject.FindGameObjectsWithTag(gameObject.tag);
        //foreach (var p in players)
        //{
        //    if (!(p && p != gameObject)) continue;
        //    Debug.Log($"Sending message to {p}");
        //    p.GetComponent<LaunchController>().SendMessage(nameof(p.AddLaunchForce), velocity);
        //}
        EndPlayerThrow();
    }

    public void EndPlayerThrow()
    {
        if (!Singleton.Instance.turnManager.UpdateState(TurnState.Thrown))
            Debug.LogWarning("BeginPlayerThrow, turnmanager");

        throwMode = false;
        launchController.acceptingInput = false;

        Singleton.Instance.vcam.Follow = transform;
    }

    public void BeginBombThrow(GameObject bombObject)
    {
        if (!Singleton.Instance.turnManager.UpdateState(TurnState.BombSummoned))
            Debug.LogWarning("EndWeaponUse, turnmanager");

        bombThrowMode = true;

        var fab = Instantiate(bombObject);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), fab.GetComponent<Collider2D>());
        var pos = transform.position;
        pos.z--;
        fab.transform.position = pos;
        fab.GetComponent<LaunchController>().acceptingInput = true;
        fab.GetComponent<Weapon>().thrower = this;
        lastThrown = fab.GetComponent<Weapon>();

        Singleton.Instance.vcam.Follow = fab.transform;
    }

    public void EndWeaponUse()
    {
        if (!Singleton.Instance.turnManager.UpdateState(TurnState.BombThrown))
            Debug.LogWarning("EndWeaponUse, turnmanager");

        bombThrowMode = false;

        Singleton.Instance.vcam.Follow = null;
    }

    public void DealExplosionDamage(Vector2 f)
    {
        // called with force vector representing launch angle
        Debug.Log("Explosion Damage " + gameObject.name + ' ' + f.magnitude);

        healthBar.currentHealth -= f.magnitude;
        Debug.Log($"Dealt {f.magnitude} damage");

        if (healthBar.currentHealth <= 0) Destroy(gameObject);
    }
}
