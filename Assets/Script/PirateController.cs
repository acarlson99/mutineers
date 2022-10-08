using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PirateController : MonoBehaviour
{
    // TODO: add health
    public GameObject menu;
    //public GameObject bombObject;
    public int teamNum;

    [HideInInspector]
    public bool throwMode = false;
    [HideInInspector]
    public bool bombThrowMode = false;
    [HideInInspector]
    public Exploder lastThrown = null;

    private LaunchController launchController;
    private Rigidbody2D rb;
    public HealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        launchController = GetComponent<LaunchController>();
        launchController.enabled = false;
        rb = GetComponent<Rigidbody2D>();
        //launchController.deselectCB = () =>
        //{
        //    launchController.enabled = false;
        //    throwMode = false;
        //};

        Singleton.Instance.turnManager.PlayerRegister(teamNum);
        if (teamNum != 0) GetComponent<SpriteRenderer>().flipX = true;
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
            launchController.enabled = true;
            bombThrowMode = false;
        }
        if (bombThrowMode)
        {
            launchController.enabled = false;
            throwMode = false;
        }
        if (Singleton.Instance.turnManager.turnNum == teamNum)
            transform.position = new Vector3(transform.position.x, transform.position.y, -0.5f);
        else
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }

    private void OnDestroy()
    {
        Singleton.Instance.turnManager.PlayerUnregister(teamNum);
        if (Singleton.Instance.turnManager.selectedBoy == gameObject)
        {
            Singleton.Instance.turnManager.UpdateState(TurnState.End);
            Singleton.Instance.vcam.Follow = null;
        }
    }

    void OnMouseUpAsButton()
    {
        if (Singleton.Instance.turnManager.turnNum != teamNum) return;
        Debug.Log("Click pirate " + gameObject.name);
        if (throwMode || bombThrowMode) return;

        menu.GetComponent<PlayerMenuController>().OpenMenuForPirate(gameObject);
    }

    public void BeginPlayerThrow()
    {
        throwMode = true;

        Singleton.Instance.vcam.Follow = transform;
    }

    public void Launch() { EndPlayerThrow(); }
    public void EndPlayerThrow()
    {
        if (!Singleton.Instance.turnManager.UpdateState(TurnState.Thrown))
            Debug.LogWarning("BeginPlayerThrow, turnmanager");

        throwMode = false;
        launchController.enabled = false;

        Singleton.Instance.vcam.Follow = transform;
    }

    public void BeginBombThrow(GameObject bombObject)
    {
        if (!Singleton.Instance.turnManager.UpdateState(TurnState.BombSummoned))
            Debug.LogWarning("EndBombThrow, turnmanager");

        bombThrowMode = true;

        var fab = Instantiate(bombObject);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), fab.GetComponent<Collider2D>());
        var pos = transform.position;
        pos.z--;
        fab.transform.position = pos;
        fab.GetComponent<LaunchController>().gravityScale = fab.GetComponent<Rigidbody2D>().gravityScale;
        fab.GetComponent<Rigidbody2D>().gravityScale = 0f; // TODO: this is an unsafe way to temporarily disable gravityscale
        fab.GetComponent<Exploder>().thrower = gameObject;
        fab.GetComponent<Exploder>().explosionEnabled = false;
        lastThrown = fab.GetComponent<Exploder>();

        Singleton.Instance.vcam.Follow = fab.transform;
    }

    public void EndBombThrow()
    {
        if (!Singleton.Instance.turnManager.UpdateState(TurnState.BombThrown))
            Debug.LogWarning("EndBombThrow, turnmanager");

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
