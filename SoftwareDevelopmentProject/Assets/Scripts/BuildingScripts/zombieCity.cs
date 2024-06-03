using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class zombieCity : MonoBehaviour
{
    public int crntHealth;
    public int maxHealth;

    public GameObject Grid;
    public GameObject Tile;

    public GameObject zombieManager;

    public GameObject SquadPrefab;
    // Start is called before the first frame update
    void Start()
    {
        zombieManager = GameObject.FindWithTag("zombieManager");
        Grid = GameObject.FindWithTag("Grid");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTurnStart()
    {
        List<GameObject> surroundigTiles = GetSurroundingTiles();
        if(zombieManager.GetComponent<PlayerManager>().squadList.Count < 50)
        {
            foreach (GameObject tile in surroundigTiles)
            {
                if (tile.GetComponent<Hex_Data>().whatsOnThisTile == null && tile.GetComponent<Hex_Data>().moveCostLand > 0)
                {
                    GameObject temp = Instantiate(SquadPrefab, new Vector3(tile.transform.position.x, 3, tile.transform.position.z), new Quaternion(), zombieManager.transform);
                    temp.GetComponent<SquadBehaviour>().currentTile = tile;
                    temp.GetComponent<SquadData>().squadTemplate = zombieManager.GetComponent<PlayerManager>().UnitTemplateList[0];
                    temp.tag = "enemySquad";
                    zombieManager.GetComponent<PlayerManager>().squadList.Add(temp);
                    break;
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
            if (tilePos.x + 1 < Grid.GetComponent<Grid>().size)
            {
                SurroundingTiles.Add(map[(int)tilePos.x + 1, (int)tilePos.y]);
            }

            if (tilePos.y + 1 < Grid.GetComponent<Grid>().size)
            {
                SurroundingTiles.Add(map[(int)tilePos.x, (int)tilePos.y + 1]);
            }

            if (tilePos.x - 1 > 0 && tilePos.y + 1 < Grid.GetComponent<Grid>().size)
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
            if (tilePos.x + 1 < Grid.GetComponent<Grid>().size)
            {
                SurroundingTiles.Add(map[(int)tilePos.x + 1, (int)tilePos.y]);
            }
            if (tilePos.x + 1 < Grid.GetComponent<Grid>().size && tilePos.y + 1 < Grid.GetComponent<Grid>().size)
            {
                SurroundingTiles.Add(map[(int)tilePos.x + 1, (int)tilePos.y + 1]);
            }
            if (tilePos.y + 1 < Grid.GetComponent<Grid>().size)
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
            if (tilePos.x + 1 < Grid.GetComponent<Grid>().size && tilePos.y - 1 >= 0)
            {
                SurroundingTiles.Add(map[(int)tilePos.x + 1, (int)tilePos.y - 1]);
            }
        }
        return SurroundingTiles;
    }
    private void OnDestroy()
    {
        zombieManager.GetComponent<PlayerManager>().cityList.Remove(this.gameObject);
    }
}
