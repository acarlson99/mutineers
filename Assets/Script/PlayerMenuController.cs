using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenuController : MonoBehaviour
{
    [HideInInspector]
    public GameObject selectedPirate = null;

    public GameObject throwButton;

    // Start is called before the first frame update
    void Start()
    {
        //gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            selectedPirate = null;
            gameObject.SetActive(false);
        }
        if (Singleton.Instance.state >= TurnState.Thrown)
        {
            throwButton.SetActive(false);
        }
        else
        {
            throwButton.SetActive(true);
        }
    }

    public bool OpenMenuForPirate(GameObject pirate)
    {
        if (Singleton.Instance.selectedBoy != null && Singleton.Instance.selectedBoy != pirate)
        {
            Debug.Log("Already selected " + Singleton.Instance.selectedBoy.name + "; ignoring");
            return false;
        }
        selectedPirate = pirate;
        gameObject.SetActive(true);
        return true;
    }

    // TODO: don't allow clicking through the UI when it is active

    public void SelectedPirateThrowMode()
    {
        if (Singleton.Instance.turnNum != selectedPirate.GetComponent<PirateController>().teamNum || Singleton.Instance.state >= TurnState.Thrown) return;
        selectedPirate.GetComponent<PirateController>().BeginPlayerThrow();
        Singleton.Instance.selectedBoy = selectedPirate;
    }

    public void SelectedPirateBomb()
    {
        if (Singleton.Instance.turnNum != selectedPirate.GetComponent<PirateController>().teamNum || Singleton.Instance.state >= TurnState.BombSummoned) return;
        selectedPirate.GetComponent<PirateController>().BeginBombThrow();
        Singleton.Instance.selectedBoy = selectedPirate;
    }

    public void EndCurrentTurn()
    {
        Singleton.Instance.state = TurnState.End;
    }
}
