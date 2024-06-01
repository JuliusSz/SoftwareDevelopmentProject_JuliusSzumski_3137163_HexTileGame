using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityData : MonoBehaviour
{
    public int equipmentProduction = 250;
    public int fuelProduction = 250;
    public bool harbourcity= false;
    public GameObject SquadPrefab;
    public GameObject Grid;

    public GameObject PlayerManager;
    public GameObject GameManager;

    public GameObject Tile;
    public int maxEquipmentStorage = 10000;
    public int storedEquipment;
    public int maxOreStorage = 10000;
    public int storedOre;
    public int maxOilStorage = 10000;
    public int storedOil;
    public int maxFuelStorage = 10000;
    public int storedFuel;
    public Queue<UnitTemplate> BuildQueue = new Queue<UnitTemplate>();
    public List<GameObject> surroundigTiles;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.FindWithTag("GameController");
        Grid = GameObject.FindWithTag("Grid");
        PlayerManager = GameObject.FindWithTag("playerManager");
        surroundigTiles = GetSurroundingTiles();
        foreach(GameObject tile in surroundigTiles)
        {
            if (tile.GetComponent<Hex_Data>().moveCostWater > 0)
            {
                harbourcity = true;
            }
        }

        storedEquipment = 2000;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTurnStart()
    {
        if (storedEquipment + equipmentProduction < maxEquipmentStorage)
        {
            storedEquipment += equipmentProduction;
        }
        else
        {
            storedEquipment = maxEquipmentStorage;
        }
        if (storedFuel + fuelProduction < maxFuelStorage)
        {
            storedFuel += fuelProduction;
        }
        else
        {
            storedFuel = maxFuelStorage;
        }
        if(BuildQueue.Count> 0)
        {
            BuildQueue.Peek().productionTime--;
            if (BuildQueue.Peek().productionTime == 0)
            {
                Debug.Log("UnitComplete");
                foreach (GameObject tile in surroundigTiles)
                {
                    if (tile.GetComponent<Hex_Data>().whatsOnThisTile == null && tile.GetComponent<Hex_Data>().moveCostLand > 0)
                    {
                        GameObject temp = Instantiate(SquadPrefab, new Vector3(tile.transform.position.x, 3, tile.transform.position.z), new Quaternion());
                        temp.GetComponent<SquadData>().squadTemplate = BuildQueue.Dequeue();
                        temp.GetComponent<SquadBehaviour>().currentTile = tile;
                        PlayerManager.GetComponent<PlayerManager>().squadList.Add(temp);
                        break;
                    }
                }
            }
        }   
    }

    public List<GameObject> GetSurroundingTiles()
    {
        List<GameObject> SurroundingTiles = new List<GameObject>();
        GameObject[,] map = Grid.GetComponent<Grid>().objMap;
        Vector2 tilePos = Tile.GetComponent<Hex_Data>().TilePosition;
        if (tilePos.y % 2 == 0)
        {
            if (tilePos.x + 1 < 100)
            {
                SurroundingTiles.Add(map[(int)tilePos.x + 1, (int)tilePos.y]);
            }

            if (tilePos.y + 1 < 100)
            {
                SurroundingTiles.Add(map[(int)tilePos.x, (int)tilePos.y + 1]);
            }

            if (tilePos.x - 1 > 0 && tilePos.y + 1 < 100)
            {
                SurroundingTiles.Add(map[(int)tilePos.x - 1, (int)tilePos.y + 1]);
            }

            if (tilePos.x - 1 >= 0)
            {
                SurroundingTiles.Add(map[(int)tilePos.x - 1, (int)tilePos.y]);
            }

            if (tilePos.x - 1 >= 0 && tilePos.y - 1 >= 0)
            {
                 SurroundingTiles.Add(map[(int)tilePos.x - 1, (int)tilePos.y - 1]);
            }

            if (tilePos.y - 1 >= 0)
            {
                SurroundingTiles.Add(map[(int)tilePos.x, (int)tilePos.y - 1]);
            }
        }
        else
        {
            if (tilePos.x + 1 < 100)
            {
                SurroundingTiles.Add(map[(int)tilePos.x + 1, (int)tilePos.y]);
            }
            if (tilePos.x + 1 < 100 && tilePos.y + 1 < 100)
            {
                SurroundingTiles.Add(map[(int)tilePos.x + 1, (int)tilePos.y + 1]);
            }
            if (tilePos.y + 1 < 100)
            {
                SurroundingTiles.Add(map[(int)tilePos.x, (int)tilePos.y + 1]);
            }
            if (tilePos.x - 1 >= 0)
            {
                SurroundingTiles.Add(map[(int)tilePos.x - 1, (int)tilePos.y]);
            }
            if (tilePos.y - 1 >= 0)
            {
                SurroundingTiles.Add(map[(int)tilePos.x, (int)tilePos.y - 1]);
            }
            if (tilePos.x + 1 < 100 && tilePos.y - 1 >= 0)
            {
                SurroundingTiles.Add(map[(int)tilePos.x + 1, (int)tilePos.y - 1]);
            }
        }
    return SurroundingTiles;
    }

}
