using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PirateController : MonoBehaviour
{
    // TODO: add health
    public GameObject menu;
    public GameObject bombObject;
    public int teamNum;

    [HideInInspector]
    public bool throwMode = false;
    [HideInInspector]
    public bool bombThrowMode = false;
    [HideInInspector]
    public Exploder lastThrown = null;

    private LaunchController launchController;

    // Start is called before the first frame update
    void Start()
    {
        launchController = GetComponent<LaunchController>();
        launchController.enabled = false;
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

    public void EndPlayerThrow() { EndLaunch(); }
    public void EndLaunch()
    {
        if (!Singleton.Instance.turnManager.UpdateState(TurnState.Thrown))
            throw new System.Exception("BeginPlayerThrow, turnmanager");

        throwMode = false;
        launchController.enabled = false;

        Singleton.Instance.vcam.Follow = transform;
    }

    public void BeginBombThrow()
    {
        if (!Singleton.Instance.turnManager.UpdateState(TurnState.BombSummoned))
            throw new System.Exception("BeginBombThrow, turnmanager");

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
            throw new System.Exception("EndBombThrow, turnmanager");

        bombThrowMode = false;

        Singleton.Instance.vcam.Follow = null;
    }
}
