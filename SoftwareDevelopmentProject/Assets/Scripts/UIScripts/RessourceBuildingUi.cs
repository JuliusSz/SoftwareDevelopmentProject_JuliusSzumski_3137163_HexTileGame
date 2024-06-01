using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RessourceBuildingUi : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject building;
    public TMP_Text title;
    public TMP_Text r1;
    public TMP_Text r2;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        switch (building.tag) 
        {
            case "mine":
                title.text = "Mine";
                r1.text = "Ore: " + building.GetComponent<MineScript>().currentStoredOre + "/" + building.GetComponent<MineScript>().maxOreStorage;
                r2.text = "";
                break;
            case "factory":
                title.text = "Factory";
                r1.text = "Ore: " + building.GetComponent<FactoryScript>().currentStoredOre + "/" + building.GetComponent<FactoryScript>().maxOreStorage;
                r2.text = "Supplies: " + building.GetComponent<FactoryScript>().currentStoredSupplies + "/" + building.GetComponent<FactoryScript>().maxSupplyStorage;
                break;
            case "oilWell":
                title.text = "Oil Well";
                r1.text = "Oil: " + building.GetComponent<OilwellScript>().currentStoredOil + "/" + building.GetComponent<OilwellScript>().maxOilStorage;
                r2.text = "";
                break;
            case "refinery":
                title.text = "Refinery";
                r1.text = "Oil: " + building.GetComponent<RefineryScript>().currentStoredOil + "/" + building.GetComponent<RefineryScript>().maxOilStorage;
                r2.text = "Fuel: " + building.GetComponent<RefineryScript>().currentStoredFuel + "/" + building.GetComponent<RefineryScript>().maxFuelStorage;
                break;
        }
    }
    private void OnDisable()
    {
        title.text = "";
        r1.text = "";
        r2.text = "";
        building = null;
    }
}
