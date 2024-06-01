using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefineryScript : MonoBehaviour
{
    public GameObject Tile;
    public int maxOilStorage = 10000;
    public int currentStoredOil = 0;
    public int maxFuelStorage = 10000;
    public int currentStoredFuel = 0;
    public int fuelCost = 500;
    public int fuelProduction = 1000;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnTurnStart()
    {
        if (currentStoredFuel + fuelProduction < maxFuelStorage && fuelCost <= currentStoredOil)
        {
            currentStoredFuel += fuelProduction;
        }
        else if (currentStoredFuel + fuelProduction > maxFuelStorage && fuelCost <= currentStoredOil)
        {
            currentStoredFuel = maxFuelStorage;
        }
    }
}
