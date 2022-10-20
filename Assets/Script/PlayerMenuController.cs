using UnityEngine;
using UnityEngine.UI;

public class PlayerMenuController : MonoBehaviour
{
    public GameObject throwButton;

    [HideInInspector]
    public GameObject selectedPirate = null;

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
            throwButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            throwButton.GetComponent<Button>().interactable = true;
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
    //public void SelectedPirateBomb(string name)
    //{
    //    IEnumerable<GameObject> b = from item in Items
    //                                where item.GetComponent<Weapon>().weaponName == name
    //                                select item;
    public void SelectedPirateBomb(GameObject weaponPrefab)
    {
        if (Singleton.Instance.turnManager.turnNum != selectedPirate.GetComponent<PirateController>().teamNum
         || Singleton.Instance.turnManager.state >= TurnState.BombSummoned)
            return;
        var w = weaponPrefab.GetComponent<Weapon>();
        var p = selectedPirate.GetComponent<PirateController>();
        Debug.Log($"Selected pirate {selectedPirate.name} inv {p.inventory} use weapon {w.name}");
        if (!Singleton.Instance.weaponController.BeginWeaponUse(p, w, w.weaponCount))
        {
            Debug.LogWarning("WARN: bad weapon use");
            return;
        }
        Singleton.Instance.turnManager.selectedBoy = selectedPirate;
    }

    public void EndCurrentTurn()
    {
        if (!Singleton.Instance.turnManager.UpdateState(TurnState.End))
            throw new System.Exception("EndCurrentTurn, turnmanager");
    }
}
