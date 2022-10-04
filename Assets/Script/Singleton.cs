using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        }
    }

    public bool UpdateState(TurnState s)
    {
        if (s <= state) return false;
        state = s;
        if (state == TurnState.End) selectedBoy = null;
        return true;
    }

    public void PlayerRegister(int team)
    {
        unitCounts[team]++;
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
        }
    }

    // Update is called once per frame
    void Update()
    {
        turnManager.Update();
    }
}
