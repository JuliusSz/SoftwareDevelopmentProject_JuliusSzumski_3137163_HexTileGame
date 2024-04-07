using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{

    public GameObject HextileGround;
    public GameObject HextileWater;
    public GameObject HextileForest;
    public int size = 100;

    float scale = .1f;

    Hex[,] map;

    void Start()
    {
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
                    hex.terain = Hex.terraintype.water;
                }
                else if(noisemap[y, x] > 0.5 && noisemap[y, x]<0.8f)
                {
                    hex.terain = Hex.terraintype.plains;
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
                switch (map[x, y].terain)
                {
                    case Hex.terraintype.plains:
                        Instantiate(HextileGround, lastPos, new Quaternion());
                        break;
                    case Hex.terraintype.forest:
                        Instantiate(HextileForest, lastPos, new Quaternion());
                        break;
                    case Hex.terraintype.water:
                        Instantiate(HextileWater, lastPos, new Quaternion());
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
