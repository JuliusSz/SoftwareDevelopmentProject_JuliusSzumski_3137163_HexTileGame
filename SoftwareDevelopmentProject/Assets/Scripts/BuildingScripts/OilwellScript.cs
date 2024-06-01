using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilwellScript : MonoBehaviour
{
    public GameObject Tile;
    public int maxOilStorage = 10000;
    public int currentStoredOil = 0;
    public int oilProduction = 250;
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
        if(currentStoredOil + oilProduction < maxOilStorage)
        {
            currentStoredOil += oilProduction;
        }
        else
        {
            currentStoredOil = maxOilStorage;
        }
    }
    
}
