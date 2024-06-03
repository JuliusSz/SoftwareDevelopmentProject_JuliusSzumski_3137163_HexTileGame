using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{

    public GameObject HextileGround;
    public GameObject HextileWater;
    public GameObject HextileForest;
    public GameObject HextileOre;
    public GameObject HextileWaterOil;
    public GameObject HextileGroundOil;
    public int size = 100;

    float scale = .1f;

    public Hex[,] map;
    public GameObject[,] objMap;

    private void Awake()
    {
        objMap = new GameObject[size,size];
        float[,] noisemap = new float[size, size];
        float xOffset = Random.Range(-10000f, 10000f);
        float yOffset = Random.Range(-10000f, 10000f);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x*scale + xOffset, y*scale + yOffset);
                noisemap[y, x] = noiseValue;
            }
        }
                map = new Hex[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Hex hex = new Hex();
                if(noisemap[y, x] < 0.5)
                {
                    float randnr = Random.Range(0, 100);
                    if(randnr < 2)
                    {
                        hex.terain = Hex.terraintype.oilWater;
                    }
                    else
                    {
                        hex.terain = Hex.terraintype.water;
                    }
                }
                else if(noisemap[y, x] > 0.5 && noisemap[y, x]<0.8f)
                {
                    float randnr = Random.Range(0f, 100f);
                    if (randnr < 6 && randnr > 1)
                    {
                        hex.terain = Hex.terraintype.ore;
                    }else if (randnr <= 0.5f)
                    {
                        hex.terain = Hex.terraintype.oilLand;
                    }
                    else
                    {
                        hex.terain = Hex.terraintype.plains;
                    }
                    
                }
                else
                {
                    hex.terain = Hex.terraintype.forest;
                }
                
                map[x, y] = hex;
            }
        }
        Vector3 lastPos = new Vector3(0,0,0);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                GameObject current;
                switch (map[x, y].terain)
                {
                    case Hex.terraintype.plains:
                        current = Instantiate(HextileGround, lastPos, new Quaternion(),this.gameObject.transform);
                        current.GetComponent<Hex_Data>().moveCostLand = 1;
                        current.GetComponent<Hex_Data>().moveCostWater = -1;
                        current.GetComponent<Hex_Data>().TilePosition = new Vector2(x, y);
                        current.GetComponent<Hex_Data>().terrainData = map[x, y];
                        current.name = "[x:"+x+"|y:"+y+"]";
                        objMap[x, y] = current;
                        break;
                    case Hex.terraintype.forest:
                        current = Instantiate(HextileForest, lastPos, new Quaternion(), this.gameObject.transform);
                        current.GetComponent<Hex_Data>().moveCostLand = 2;
                        current.GetComponent<Hex_Data>().moveCostWater = -1;
                        current.GetComponent<Hex_Data>().TilePosition = new Vector2(x, y);
                        current.GetComponent<Hex_Data>().terrainData = map[x, y];
                        current.name = "[x:" + x + "|y:" + y + "]";
                        objMap[x, y] = current;
                        break;
                    case Hex.terraintype.water:
                        current = Instantiate(HextileWater, lastPos, new Quaternion(), this.gameObject.transform);
                        current.GetComponent<Hex_Data>().moveCostLand = -1;
                        current.GetComponent<Hex_Data>().moveCostWater = 1;
                        current.GetComponent<Hex_Data>().TilePosition = new Vector2(x, y);
                        current.GetComponent<Hex_Data>().terrainData = map[x, y];
                        current.name = "[x:" + x + "|y:" + y + "]";
                        objMap[x, y] = current;
                        break;
                    case Hex.terraintype.oilLand:
                        current = Instantiate(HextileGroundOil, lastPos, new Quaternion(), this.gameObject.transform);
                        current.GetComponent<Hex_Data>().moveCostLand = 2;
                        current.GetComponent<Hex_Data>().moveCostWater = -1;
                        current.GetComponent<Hex_Data>().TilePosition = new Vector2(x, y);
                        current.GetComponent<Hex_Data>().terrainData = map[x, y];
                        current.name = "[x:" + x + "|y:" + y + "]";
                        objMap[x, y] = current;
                        break;
                    case Hex.terraintype.oilWater:
                        current = Instantiate(HextileWaterOil, lastPos, new Quaternion(), this.gameObject.transform);
                        current.GetComponent<Hex_Data>().moveCostLand = -1;
                        current.GetComponent<Hex_Data>().moveCostWater = 2;
                        current.GetComponent<Hex_Data>().TilePosition = new Vector2(x, y);
                        current.GetComponent<Hex_Data>().terrainData = map[x, y];
                        current.name = "[x:" + x + "|y:" + y + "]";
                        objMap[x, y] = current;
                        break;
                    case Hex.terraintype.ore:
                        current = Instantiate(HextileOre, lastPos, new Quaternion(), this.gameObject.transform);
                        current.GetComponent<Hex_Data>().moveCostLand = 2;
                        current.GetComponent<Hex_Data>().moveCostWater = -1;
                        current.GetComponent<Hex_Data>().TilePosition = new Vector2(x, y);
                        current.GetComponent<Hex_Data>().terrainData = map[x, y];
                        current.name = "[x:" + x + "|y:" + y + "]";
                        objMap[x, y] = current;
                        break;
                }
                lastPos.x -= 3.45f;
            }
            if(y%2 == 0) 
            {
                lastPos.x = -1.7f;
            }
            else
            {
                lastPos.x = 0;
            }
            lastPos.z += 3;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
