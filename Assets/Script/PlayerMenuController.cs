using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerMenuController : MonoBehaviour
{
    [HideInInspector]
    public GameObject selectedPirate = null;

    public GameObject throwButton;

    public GameObject[] Items = { };

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            selectedPirate = null;
            gameObject.SetActive(false);
        }
        if (Singleton.Instance.turnManager.state >= TurnState.Thrown)
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
        if (Singleton.Instance.turnManager.selectedBoy != null && Singleton.Instance.turnManager.selectedBoy != pirate)
        {
            Debug.Log("Already selected " + Singleton.Instance.turnManager.selectedBoy.name + "; ignoring");
            return false;
        }
        selectedPirate = pirate;
        gameObject.SetActive(true);
        return true;
    }

    // TODO: don't allow clicking through the UI when it is active

    public void SelectedPirateThrowMode()
    {
        if (Singleton.Instance.turnManager.turnNum != selectedPirate.GetComponent<PirateController>().teamNum
         || Singleton.Instance.turnManager.state >= TurnState.Thrown)
            return;
        selectedPirate.GetComponent<PirateController>().BeginPlayerThrow();
        Singleton.Instance.turnManager.selectedBoy = selectedPirate;
    }

    //public void SelectedPirateBomb()
    public void SelectedPirateBomb(string name)
    {
        IEnumerable<GameObject> b = from item in Items
                                    where item.GetComponent<Exploder>().explosiveName == name
                                    select item;
        Assert.AreEqual<int>(b.Count(), 1);
        if (Singleton.Instance.turnManager.turnNum != selectedPirate.GetComponent<PirateController>().teamNum
         || Singleton.Instance.turnManager.state >= TurnState.BombSummoned)
            return;
        selectedPirate.GetComponent<PirateController>().BeginBombThrow(b.First());
        Singleton.Instance.turnManager.selectedBoy = selectedPirate;
    }

    public void EndCurrentTurn()
    {
        if (!Singleton.Instance.turnManager.UpdateState(TurnState.End))
            throw new System.Exception("EndCurrentTurn, turnmanager");
    }
}
