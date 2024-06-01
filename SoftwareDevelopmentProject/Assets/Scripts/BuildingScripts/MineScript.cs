using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineScript : MonoBehaviour
{
    public GameObject Tile;
    public int maxOreStorage = 10000;
    public int currentStoredOre = 0;
    public int oreProduction = 250;
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
        if (currentStoredOre + oreProduction < maxOreStorage)
        {
            currentStoredOre += oreProduction;
        }
        else
        {
            currentStoredOre = maxOreStorage;
        }
    }
}
