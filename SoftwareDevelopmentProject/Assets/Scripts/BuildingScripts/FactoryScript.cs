using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryScript : MonoBehaviour
{
    public GameObject Tile;
    public int maxOreStorage = 10000;
    public int currentStoredOre = 0;
    public int maxSupplyStorage = 10000;
    public int currentStoredSupplies = 0;
    public int oreCost = 500;
    public int supplyProduction = 1000;
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
        if(currentStoredSupplies + supplyProduction < maxSupplyStorage && oreCost <= currentStoredOre )
        {
            currentStoredSupplies += supplyProduction;
        }else if (currentStoredSupplies + supplyProduction > maxSupplyStorage && oreCost <= currentStoredOre)
        {
            currentStoredSupplies = maxSupplyStorage;
        }
    }
}
