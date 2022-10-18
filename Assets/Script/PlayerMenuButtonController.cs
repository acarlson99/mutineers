using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMenuButtonController : MonoBehaviour
{
    public GameObject weaponPrefab;
    public GameObject playerMenu;
    public GameObject numText;
    public GameObject nameText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var pmc = playerMenu.GetComponent<PlayerMenuController>();
        if (pmc && pmc.selectedPirate != null)
        {
            var pc = pmc.selectedPirate.GetComponent<PirateController>();
            if (!pc) Debug.LogWarning("NO pirate controller????");
            SetCount(pc.inventory.Get(weaponPrefab.GetComponent<Weapon>().weaponType));
        }
    }

    public void SetCount(int n)
    {
        if (n == 0)
        {
            GetComponent<Button>().interactable = false;
            numText.GetComponent<TMP_Text>().text = "";
        }
        else
        {
            GetComponent<Button>().interactable = true;
            var s = $"{n}";
            if (n < 0) s = "\u221E";
            numText.GetComponent<TMP_Text>().text = s;
        }
    }

    public void SetCanvasInactive()
    {
        playerMenu.SetActive(false);
    }

    public void UseItem()
    {
        playerMenu.GetComponent<PlayerMenuController>().SelectedPirateBomb(weaponPrefab);
    }
}
