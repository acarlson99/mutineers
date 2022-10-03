using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum TurnState { Start, Thrown, BombSummoned, BombThrown, End };

public class Singleton : MonoBehaviour
{
    // Start is called before the first frame update
    public static Singleton Instance { get; private set; }

    public float killzoneY;
    public LineRenderer lineRenderer;
    public GameObject selectedBoy = null;

    [HideInInspector]
    public int turnNum = 0;

    public TurnState state = TurnState.Start;

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
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (state == TurnState.BombThrown || state == TurnState.End)
        {
            turnNum++;
            turnNum %= 2;
            state = TurnState.Start;
            selectedBoy = null;
        }
    }

    public int[] unitCounts = { 0, 0 };

    public void PlayerManager(int team, bool register)
    {
        // register gameobj or unregister if false
        if (register) unitCounts[team]++;
        else unitCounts[team]--;
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
