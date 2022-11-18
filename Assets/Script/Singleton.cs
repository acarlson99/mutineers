using Cinemachine;
using UnityEngine;

public enum TurnState { Start, Thrown, BombSummoned, BombThrown, End };

public class TurnManager
{
    public TurnState state { get; private set; }
    public GameObject selectedBoy = null;
    public int turnNum = 0;
    public int[] unitCounts = { 0, 0 };

    public void Update()
    {
        if (state == TurnState.BombThrown || state == TurnState.End)
        {
            turnNum++;
            turnNum %= 2;
            state = TurnState.Start;
            selectedBoy = null;
            Singleton.Instance.CamFollow(null);
            var chestObj = TreasureChest.DropRandomly();
            if (chestObj != null) Singleton.Instance.CamFollow(chestObj.transform);
        }
    }

    public bool UpdateState(TurnState s)
    {
        Debug.Log($"Update state {state} to {s}");
        if (s <= state) return false;
        state = s;
        if (state == TurnState.End) selectedBoy = null;
        return true;
    }

    public void PlayerRegister(int team, PirateController p)
    {
        unitCounts[team]++;
        var bars = GameObject.FindGameObjectsWithTag("TeamHealthBar");
        foreach (var bar in bars)
        {
            if (bar.GetComponent<TeamHealthBar>().teamNum == p.teamNum) bar.GetComponent<TeamHealthBar>().AddToPool(p.healthBar);
        }
    }

    public void PlayerUnregister(int team)
    {
        unitCounts[team]--;
    }

    public int GetLosingTeam()
    {
        for (int i = 0; i < unitCounts.Length; i++)
        {
            if (unitCounts[i] == 0) return i;
        }
        return -1;
    }
}

public class Singleton : MonoBehaviour
{
    public static Singleton Instance { get; private set; }

    public float killzoneY;
    public LineRenderer lineRenderer;

    public TurnManager turnManager = null;
    public WeaponController weaponController;

    [SerializeField]
    private CinemachineVirtualCamera vcam = null;
    public CinemachineVirtualCamera Vcam { get { return vcam; } set { vcam = value; } }

    public EWeaponType[] chestWeapons; // TODO: parameterize further

    public bool camFollowMode = false;
    public PolygonCollider2D cameraBounds;
    public GameObject playerMenu;
    public GameObject chestFab;
    public Sprite captainSprite;
    public Sprite mutineerSprite;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            turnManager = new TurnManager();
            weaponController = gameObject.AddComponent<WeaponController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        turnManager.Update();
        if (turnManager.selectedBoy == null)
        {
            vcam.Follow = null;
        }
    }

    public void CamFollow(Transform t)
    {
        Debug.Log($"FOLLOW {t}");
        vcam.Follow = t;
        if (t == null) camFollowMode = false;
        else camFollowMode = true;
        _f = null;
    }

    [HideInInspector]
    public Transform _f;

    public void CamQuietUnfollow()
    {
        if (!camFollowMode || vcam.Follow == null) return;
        Debug.Log("Quietly unfollow");
        _f = vcam.Follow;
        vcam.Follow = null;
    }

    public void CamQuietRefollow()
    {
        if (!camFollowMode || _f == null) return;
        Debug.Log("Quiet REFOLLOW");
        vcam.Follow = _f;
        _f = null;
    }

    // TODO: create VCAM priority queue for vcam.follow
    //       enum FollowPriority {
    //           Player,
    //           Event, // new chest, text, etc.
    //           Weapon,
    //           WeaponThrown,
    //       }
}
