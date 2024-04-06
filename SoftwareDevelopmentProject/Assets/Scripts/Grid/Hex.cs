using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour
{
    public terraintype terain;
    public int terrainheight;
    public enum terraintype
    {
        water,
        plains,
        forest
    }
}
