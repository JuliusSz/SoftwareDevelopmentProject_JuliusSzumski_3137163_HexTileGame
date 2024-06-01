using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.UIElements;

public class SquadBehaviour : MonoBehaviour
{
    public enum selectedAction
    {
        none,
        attack,
        buildCity,
        buildMine,
        buildFactory,
        buildOilwell,
        buildRefinery,
        createSupplyRoute
    }

    public bool doingSupplyRuns = false;
    public bool onWayToLoadUp;

    public GameObject suppliedFrom;
    public GameObject suppliedTo;

    public GameObject cityPrefab;
    public GameObject minePrefab;
    public GameObject factoryPrefab;
    public GameObject oilewellPrefab;
    public GameObject refineryPrefab;

    public PlayerManager playerManager;
    public GameManager manager;


    public Dictionary<Equipment, int> EquipmentDir = new Dictionary<Equipment, int>();

    public SquadData squadData;

    public selectedAction currentAction = selectedAction.none;
    public GameObject SelectedTile;
    public GameObject SelectedTile2;
    public GameObject Grid;
    public Stack<GameObject> path = new Stack<GameObject>();
    public GameObject currentTile;
    public List<GameObject> openList = new List<GameObject>();
    public List<GameObject> closedList = new List<GameObject>();
    public int maxMovement;
    public int leftMovement;
    public float speed;

    public List<Equipment> selectedEquipment;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.FindWithTag("playerManager").GetComponent<PlayerManager>();
        manager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        Grid = GameObject.FindWithTag("Grid");
        currentTile.GetComponent<Hex_Data>().whatsOnThisTile = this.gameObject;
        OnTurnStart();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(currentTile.transform.position.x, transform.position.y, currentTile.transform.position.z), speed);
        if (path.Count != 0 && leftMovement > 0 && transform.position == new Vector3(currentTile.transform.position.x, transform.position.y, currentTile.transform.position.z))
        {
            if(path.Peek().GetComponent<Hex_Data>().whatsOnThisTile == null)
            {
                currentTile.GetComponent<Hex_Data>().whatsOnThisTile = null;
                currentTile = path.Pop();
                currentTile.GetComponent<Hex_Data>().whatsOnThisTile = gameObject;
                leftMovement--;
            }

        }
        if (doingSupplyRuns)
        {
            if (onWayToLoadUp)
            {
                if (GetDistanceBetwenTiles(currentTile, suppliedFrom) == 1)
                {
                    switch (suppliedFrom.GetComponent<Hex_Data>().whatsOnThisTile.tag)
                    {
                        case "mine":
                            MineScript mine = suppliedFrom.GetComponent<Hex_Data>().whatsOnThisTile.GetComponent<MineScript>();
                            if(mine.currentStoredOre > squadData.MaxOreTransport - squadData.crntOreTransport)
                            {
                                mine.currentStoredOre -= squadData.MaxOreTransport - squadData.crntOreTransport;
                                squadData.crntOreTransport += squadData.MaxOreTransport - squadData.crntOreTransport;
                            }
                            else
                            {
                                squadData.crntOreTransport += mine.currentStoredOre;
                                mine.currentStoredOre = 0;
                            }
                            break;
                        case "factory":
                            FactoryScript factory = suppliedFrom.GetComponent<Hex_Data>().whatsOnThisTile.GetComponent<FactoryScript>();
                            if (factory.currentStoredSupplies > squadData.MaxSupplyTransport - squadData.crntSupplyTransport)
                            {
                                factory.currentStoredSupplies -= squadData.MaxSupplyTransport - squadData.crntSupplyTransport;
                                squadData.crntSupplyTransport += squadData.MaxSupplyTransport - squadData.crntSupplyTransport;
                            }
                            else
                            {
                                squadData.crntSupplyTransport += factory.currentStoredSupplies;
                                factory.currentStoredSupplies = 0;
                            }
                            break;
                        case "oilWell":
                            OilwellScript oilWell = suppliedFrom.GetComponent<Hex_Data>().whatsOnThisTile.GetComponent<OilwellScript>();
                            if (oilWell.currentStoredOil > squadData.MaxOilTransport - squadData.crntOilTransport)
                            {
                                oilWell.currentStoredOil -= squadData.MaxOilTransport - squadData.crntOilTransport;
                                squadData.crntOilTransport += squadData.MaxOilTransport - squadData.crntOilTransport;
                            }
                            else
                            {
                                squadData.crntOilTransport += oilWell.currentStoredOil;
                                oilWell.currentStoredOil = 0;
                            }
                            break;
                        case "refinery":
                            RefineryScript refinery = suppliedFrom.GetComponent<Hex_Data>().whatsOnThisTile.GetComponent<RefineryScript>();
                            if (refinery.currentStoredFuel > squadData.MaxfuelTransport - squadData.crntfuelTransport)
                            {
                                refinery.currentStoredFuel -= squadData.MaxfuelTransport - squadData.crntfuelTransport;
                                squadData.crntfuelTransport += squadData.MaxfuelTransport - squadData.crntfuelTransport;
                            }
                            else
                            {
                                squadData.crntfuelTransport += refinery.currentStoredFuel;
                                refinery.currentStoredFuel = 0;
                            }
                            break;

                    }
                    Movement(suppliedTo);
                }
            }
            else
            {
                if (GetDistanceBetwenTiles(currentTile, suppliedTo) == 1)
                {
                    switch (suppliedTo.GetComponent<Hex_Data>().whatsOnThisTile.tag)
                    {
                        case "factory":
                            FactoryScript factory = suppliedFrom.GetComponent<Hex_Data>().whatsOnThisTile.GetComponent<FactoryScript>();
                            if (squadData.crntOreTransport > factory.maxOreStorage - factory.currentStoredOre)
                            {
                                squadData.crntOreTransport -= factory.maxOreStorage - factory.currentStoredOre;
                                factory.currentStoredOre += factory.maxOreStorage - factory.currentStoredOre;
                            }
                            else
                            {
                                factory.currentStoredOre += squadData.crntOreTransport;
                                squadData.crntOreTransport = 0;
                            }
                            break;
                        case "refinery":
                            RefineryScript refinery = suppliedFrom.GetComponent<Hex_Data>().whatsOnThisTile.GetComponent<RefineryScript>();
                            if (squadData.crntOilTransport > refinery.maxOilStorage -  refinery.currentStoredOil)
                            {
                                squadData.crntOilTransport -= refinery.maxOilStorage - refinery.currentStoredOil;
                                refinery.currentStoredOil += refinery.maxOilStorage - refinery.currentStoredOil;
                            }
                            else
                            {
                                refinery.currentStoredOil += squadData.crntOilTransport;
                                squadData.crntOilTransport = 0;
                            }
                            break;
                        case "city":
                            CityData city = suppliedFrom.GetComponent<Hex_Data>().whatsOnThisTile.GetComponent<CityData>();
                            if (squadData.crntfuelTransport > city.maxFuelStorage - city.storedFuel)
                            {
                                squadData.crntfuelTransport -= city.maxFuelStorage - city.storedFuel;
                                city.storedFuel += city.maxFuelStorage - city.storedFuel;
                            }
                            else
                            {
                                city.storedFuel += squadData.crntfuelTransport;
                                squadData.crntfuelTransport = 0;
                            }

                            if (squadData.crntSupplyTransport > city.maxEquipmentStorage - city.storedEquipment)
                            {
                                squadData.crntSupplyTransport -= city.maxEquipmentStorage - city.storedEquipment;
                                city.storedEquipment += city.maxEquipmentStorage - city.storedEquipment;
                            }
                            else
                            {
                                city.storedEquipment += squadData.crntSupplyTransport;
                                squadData.crntSupplyTransport = 0;
                            }

                            break;

                    }
                    Movement(suppliedFrom);
                }
            }

        }

        if (currentAction != selectedAction.none)
        {
            manager.somethingIsChosingaTarget = true;
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    SelectedTile = hit.collider.gameObject;
                    if (SelectedTile != null)
                    {
                        switch (currentAction)
                        {
                            case selectedAction.buildCity:

                                if (SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile == null)
                                {                                  
                                    if(GetDistanceBetwenTiles(currentTile,SelectedTile )== 1)
                                    {
                                        SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile =  Instantiate(cityPrefab, new Vector3(SelectedTile.transform.position.x,3, SelectedTile.transform.position.z),new Quaternion());
                                        SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile.GetComponent<CityData>().Tile = SelectedTile.transform.parent.gameObject;
                                        playerManager.cityList.Add(SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile);
                                        currentAction = selectedAction.none;
                                        manager.somethingIsChosingaTarget = false;
                                    }
                                    
                                }
                                break;

                            case selectedAction.buildMine:
                                if (SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile == null && SelectedTile.transform.parent.GetComponent<Hex_Data>().terrainData.terain == Hex.terraintype.ore)
                                {                                
                                    if (GetDistanceBetwenTiles(currentTile, SelectedTile) == 1)
                                    {
                                            
                                            SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile = Instantiate(minePrefab, new Vector3(SelectedTile.transform.position.x, 3, SelectedTile.transform.position.z), new Quaternion());
                                            SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile.GetComponent<MineScript>().Tile = SelectedTile.transform.parent.gameObject;
                                            playerManager.mineList.Add(SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile);
                                            currentAction = selectedAction.none;
                                            manager.somethingIsChosingaTarget = false;
                                            break;
                                    }
                                }
                                break;
                            case selectedAction.buildFactory:
                                if (SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile == null && (SelectedTile.transform.parent.GetComponent<Hex_Data>().terrainData.terain == Hex.terraintype.plains || SelectedTile.transform.parent.GetComponent<Hex_Data>().terrainData.terain == Hex.terraintype.forest))
                                {
                                    if(GetDistanceBetwenTiles(currentTile, SelectedTile) == 1)
                                    {

                                        SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile = Instantiate(factoryPrefab, new Vector3(SelectedTile.transform.position.x, 3, SelectedTile.transform.position.z), new Quaternion());
                                        SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile.GetComponent<FactoryScript>().Tile = SelectedTile.transform.parent.gameObject;
                                        playerManager.factoryList.Add(SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile);
                                        currentAction = selectedAction.none;
                                        manager.somethingIsChosingaTarget = false;
                                        break;
                                    }                                    
                                }
                                break;
                            case selectedAction.buildRefinery:
                                if (SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile == null && (SelectedTile.transform.parent.GetComponent<Hex_Data>().terrainData.terain == Hex.terraintype.plains || SelectedTile.transform.parent.GetComponent<Hex_Data>().terrainData.terain == Hex.terraintype.forest))
                                {
                                    if (GetDistanceBetwenTiles(currentTile, SelectedTile) == 1)
                                    {

                                        SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile = Instantiate(refineryPrefab, new Vector3(SelectedTile.transform.position.x, 3, SelectedTile.transform.position.z), new Quaternion());
                                        SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile.GetComponent<RefineryScript>().Tile = SelectedTile.transform.parent.gameObject;
                                        playerManager.refineryList.Add(SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile);
                                        currentAction = selectedAction.none;
                                        manager.somethingIsChosingaTarget = false;
                                        break;
                                    }
                                }
                                break;
                            case selectedAction.buildOilwell:
                                if (SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile == null && (SelectedTile.transform.parent.GetComponent<Hex_Data>().terrainData.terain == Hex.terraintype.oilLand || SelectedTile.transform.parent.GetComponent<Hex_Data>().terrainData.terain == Hex.terraintype.oilWater))
                                {
                                    if (GetDistanceBetwenTiles(currentTile, SelectedTile) == 1)
                                    {
                                        SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile = Instantiate(oilewellPrefab, new Vector3(SelectedTile.transform.position.x, 3, SelectedTile.transform.position.z), new Quaternion());
                                        SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile.GetComponent<OilwellScript>().Tile = SelectedTile.transform.parent.gameObject;
                                        playerManager.oilwellList.Add(SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile);
                                        currentAction = selectedAction.none;
                                        manager.somethingIsChosingaTarget = false;
                                        break;
                                    }
                                }
                                break;
                            case selectedAction.createSupplyRoute:
                                switch (SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile.tag)
                                {
                                    case "mine":
                                        if (Input.GetMouseButtonDown(0))
                                        {
                                            Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
                                            RaycastHit hit2;

                                            if (Physics.Raycast(ray2, out hit2))
                                            {
                                                SelectedTile2 = hit.collider.gameObject;
                                                if (SelectedTile2 != null && SelectedTile2.GetComponent<Hex_Data>().whatsOnThisTile.tag == "refinery")
                                                {
                                                    suppliedFrom = SelectedTile;
                                                    suppliedTo = SelectedTile2;
                                                    doingSupplyRuns = true;
                                                    Movement(suppliedFrom);

                                                }
                                            }
                                        }
                                        break;
                                    case "factory":
                                        if (Input.GetMouseButtonDown(0))
                                        {
                                            Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
                                            RaycastHit hit2;

                                            if (Physics.Raycast(ray2, out hit2))
                                            {
                                                SelectedTile2 = hit.collider.gameObject;
                                                if (SelectedTile2 != null && SelectedTile2.GetComponent<Hex_Data>().whatsOnThisTile.tag == "city")
                                                {
                                                    suppliedFrom = SelectedTile;
                                                    suppliedTo = SelectedTile2;
                                                    doingSupplyRuns = true;
                                                    Movement(suppliedFrom);
                                                }
                                            }
                                        }
                                        break;
                                    case "oilWell":
                                        if (Input.GetMouseButtonDown(0))
                                        {
                                            Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
                                            RaycastHit hit2;

                                            if (Physics.Raycast(ray2, out hit2))
                                            {
                                                SelectedTile2 = hit.collider.gameObject;
                                                if (SelectedTile2 != null && SelectedTile2.GetComponent<Hex_Data>().whatsOnThisTile.tag == "refinery")
                                                {
                                                    suppliedFrom = SelectedTile;
                                                    suppliedTo = SelectedTile2;
                                                    doingSupplyRuns = true;
                                                    Movement(suppliedFrom);
                                                }
                                            }
                                        }
                                        break;
                                    case "refinery":
                                        if (Input.GetMouseButtonDown(0))
                                        {
                                            Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
                                            RaycastHit hit2;

                                            if (Physics.Raycast(ray2, out hit2))
                                            {
                                                SelectedTile2 = hit.collider.gameObject;
                                                if (SelectedTile2 != null && SelectedTile2.GetComponent<Hex_Data>().whatsOnThisTile.tag == "city")
                                                {
                                                    suppliedFrom = SelectedTile;
                                                    suppliedTo = SelectedTile2;
                                                    doingSupplyRuns = true;
                                                    Movement(suppliedFrom);
                                                }
                                            }
                                        }
                                        break;
                                }
                                break;
                            case selectedAction.attack:
                                if (SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile.tag == "enemySquad")
                                {
                                    GameObject targetedSquad = SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile;
                                    if (GetDistanceBetwenTiles(currentTile,SelectedTile)<= selectedEquipment[0].range)
                                    {
                                        foreach (Equipment selectedEquipment in selectedEquipment) 
                                        {
                                            selectedEquipment.ready = false;
                                            //Debug.Log(EquipmentDir[selectedEquipment]);
                                            int isHit = UnityEngine.Random.Range(0,100);
                                            if (isHit <= selectedEquipment.accuracy)
                                            {
                                                //Debug.Log("Hit");
                                                List<unit> hitUnits = new List<unit>();
                                                if(targetedSquad.GetComponent<SquadData>().currentUnitState.Count > selectedEquipment.fragmentation)
                                                {
                                                    for (int j = 0; j <= selectedEquipment.fragmentation; j++)
                                                    {
                                                        hitUnits.Add(targetedSquad.GetComponent<SquadData>().currentUnitState[UnityEngine.Random.Range(0, targetedSquad.GetComponent<SquadData>().currentUnitState.Count - 1)]);
                                                    }
                                                }
                                                else
                                                {
                                                    hitUnits = targetedSquad.GetComponent<SquadData>().currentUnitState;
                                                }
                                               // Debug.Log(hitUnits.Count);
                                                foreach (unit unit in hitUnits)
                                                {

                                                    int effectiveArmor = unit.armor - selectedEquipment.armorPenetration;
                                                    int BulletPenetration = UnityEngine.Random.Range(0,100);
                                                    if(effectiveArmor <= BulletPenetration)
                                                    {
                                                        //Debug.Log("Penetrated");
                                                        unit.health -= selectedEquipment.damage;
                                                        if(unit.health <= 0)
                                                        {
                                                            //Debug.Log("killed");
                                                            targetedSquad.GetComponent<SquadData>().currentUnitState.Remove(unit);
                                                            if(targetedSquad.GetComponent<SquadData>().currentUnitState.Count == 0)
                                                            {
                                                                Destroy(targetedSquad);
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                                if(targetedSquad == null)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                selectedEquipment = null;
                                break;
                        }
                    }
                        
                }
                SelectedTile = null;
                manager.somethingIsChosingaTarget = false;
                currentAction = selectedAction.none;
            }

        }
    }
    public int GetDistanceBetwenTiles(GameObject crrntTile, GameObject targetTile)
    {
        int[] crntTile3D = convertTo3DArray((int)crrntTile.GetComponent<Hex_Data>().TilePosition.x, (int)crrntTile.GetComponent<Hex_Data>().TilePosition.y);
        int[] SelectedTile3D = convertTo3DArray((int)SelectedTile.transform.parent.GetComponent<Hex_Data>().TilePosition.x, (int)SelectedTile.transform.parent.GetComponent<Hex_Data>().TilePosition.y);

        return ((Mathf.Abs(crntTile3D[0] - SelectedTile3D[0]) + Mathf.Abs(crntTile3D[2] - SelectedTile3D[2]) + Mathf.Abs(crntTile3D[1] - SelectedTile3D[1])) / 2);
    }

    public int[] convertTo3DArray(int tileXPos, int tileYPos)
    {
        int[] newCoords = new int[3];
        newCoords[0] = tileXPos - (tileYPos - (tileYPos % 2)) / 2;
        newCoords[1] = tileYPos;
        newCoords[2] = -newCoords[0] - newCoords[1];
        return (newCoords);
    }

    public void OnTurnStart()
    {
        leftMovement = maxMovement;

        EquipmentDir.Clear();

        foreach (unit aliveUnit in squadData.currentUnitState)
        {
            if (EquipmentDir.ContainsKey(aliveUnit.equipment))
            {
                EquipmentDir[aliveUnit.equipment]++;
            }
            else
            {
                EquipmentDir.Add(aliveUnit.equipment,1);
            }
        }
    }
    public void Movement(GameObject Goal)  
    {
        bool reachedGoal = false;

        GameObject[,] map = Grid.GetComponent<Grid>().objMap;


        openList.Add(currentTile);

        while (openList.Count!= 0 && reachedGoal==false)
        {
            GameObject q = openList[0];

            foreach (var tile in openList)
            {
                if (q.GetComponent<Hex_Data>().f > tile.GetComponent<Hex_Data>().f)
                {
                    q = tile;
                }
            }
            openList.Remove(q);

            Vector2 qPos = q.GetComponent<Hex_Data>().TilePosition;

            List<GameObject> temp = new List<GameObject>();

            if(qPos.y %2 == 0)
            {
                if (qPos.x + 1 < 100)
                {
                    if (map[(int)qPos.x + 1, (int)qPos.y].GetComponent<Hex_Data>().moveCostLand > 0)
                        temp.Add(map[(int)qPos.x + 1, (int)qPos.y]);
                }

                if ( qPos.y + 1 < 100)
                {
                    if (map[(int)qPos.x, (int)qPos.y + 1].GetComponent<Hex_Data>().moveCostLand > 0)
                        temp.Add(map[(int)qPos.x, (int)qPos.y + 1]);
                }

                if ( qPos.x - 1 > 0 && qPos.y + 1 < 100)
                {
                    if (map[(int)qPos.x - 1, (int)qPos.y + 1].GetComponent<Hex_Data>().moveCostLand > 0)
                        temp.Add(map[(int)qPos.x - 1, (int)qPos.y + 1]);
                }

                if (qPos.x - 1 >= 0)
                {
                    if (map[(int)qPos.x - 1, (int)qPos.y].GetComponent<Hex_Data>().moveCostLand > 0)
                        temp.Add(map[(int)qPos.x - 1, (int)qPos.y]);
                }

                if (qPos.x - 1 >= 0 && qPos.y - 1 >= 0)
                {
                    if (map[(int)qPos.x - 1, (int)qPos.y - 1].GetComponent<Hex_Data>().moveCostLand > 0)
                        temp.Add(map[(int)qPos.x - 1, (int)qPos.y - 1]);
                }

                if (qPos.y - 1 >= 0)
                {
                    if (map[(int)qPos.x, (int)qPos.y - 1].GetComponent<Hex_Data>().moveCostLand > 0)
                        temp.Add(map[(int)qPos.x, (int)qPos.y - 1]);
                }
            }
            else
            {
                if (qPos.x + 1 < 100)
                {
                    if (map[(int)qPos.x + 1, (int)qPos.y].GetComponent<Hex_Data>().moveCostLand > 0)
                        temp.Add(map[(int)qPos.x + 1, (int)qPos.y]);
                }

                if (qPos.x + 1 < 100 && qPos.y + 1 < 100)
                {
                    if (map[(int)qPos.x + 1, (int)qPos.y + 1].GetComponent<Hex_Data>().moveCostLand > 0)
                        temp.Add(map[(int)qPos.x + 1, (int)qPos.y + 1]);
                }

                if (qPos.y + 1 < 100)
                {
                    if (map[(int)qPos.x, (int)qPos.y + 1].GetComponent<Hex_Data>().moveCostLand > 0)
                        temp.Add(map[(int)qPos.x, (int)qPos.y + 1]);
                }

                if (qPos.x - 1 >= 0)
                {
                    if (map[(int)qPos.x - 1, (int)qPos.y].GetComponent<Hex_Data>().moveCostLand > 0)
                        temp.Add(map[(int)qPos.x - 1, (int)qPos.y]);
                }

                if (qPos.y - 1 >= 0 )
                {
                    if (map[(int)qPos.x, (int)qPos.y - 1].GetComponent<Hex_Data>().moveCostLand > 0)
                        temp.Add(map[(int)qPos.x, (int)qPos.y - 1]);
                }

                if (qPos.x + 1 < 100 && qPos.y - 1 >= 0)
                {
                    if (map[(int)qPos.x + 1, (int)qPos.y - 1].GetComponent<Hex_Data>().moveCostLand > 0)
                        temp.Add(map[(int)qPos.x + 1, (int)qPos.y - 1]);
                }
            }
            for (int i = 0; i <  temp.Count; i++)
            {
                if (closedList.Contains(temp[i]))
                {
                    temp.Remove(temp[i]);
                }
            }
            foreach (GameObject successor in temp)
            {

                

                if (successor == Goal)
                {
                    reachedGoal = true;
                }

                float dx = Goal.GetComponent<Hex_Data>().TilePosition.x - successor.GetComponent<Hex_Data>().TilePosition.x;
                float dy = Goal.GetComponent<Hex_Data>().TilePosition.y - successor.GetComponent<Hex_Data>().TilePosition.y;

                successor.GetComponent<Hex_Data>().g = q.GetComponent<Hex_Data>().g + successor.GetComponent<Hex_Data>().moveCostLand;
                successor.GetComponent<Hex_Data>().h = (int)Mathf.Sqrt(dx * dx + dy * dy);               
                successor.GetComponent<Hex_Data>().f = successor.GetComponent<Hex_Data>().g + successor.GetComponent<Hex_Data>().h;

                bool skip = false;
                foreach (GameObject tile in openList)
                {
                    if (((tile.GetComponent<Hex_Data>().TilePosition == successor.GetComponent<Hex_Data>().TilePosition && successor.GetComponent<Hex_Data>().f >= tile.GetComponent<Hex_Data>().f)||(closedList.Contains(successor)&& successor.GetComponent<Hex_Data>().f >= tile.GetComponent<Hex_Data>().f) || successor.GetComponent<Hex_Data>().moveCostLand < 0))
                    {
                        skip = true;
                        break;
                    }
                    
                }
                if (!skip)
                {
                    successor.GetComponent<Hex_Data>().parent = q; 
                    openList.Add(successor);
                }
            }
            closedList.Add(q);
        }
        if(reachedGoal == false)
        {
            openList.Clear();
            closedList.Clear();
            Debug.Log("there is no clear path");
            foreach (GameObject go in map)
            {
                go.GetComponent<Hex_Data>().f = 0;
                go.GetComponent<Hex_Data>().g = 0;
                go.GetComponent<Hex_Data>().h = 0;
                go.GetComponent<Hex_Data>().parent = null;
            }
        }
        else
        {
            path.Clear();
            Debug.Log("path succesfully found");
            pathTraceback(Goal, currentTile);
            openList.Clear();
            closedList.Clear();
            foreach (GameObject go in map)
            {
                go.GetComponent<Hex_Data>().f = 0;
                go.GetComponent<Hex_Data>().g = 0;
                go.GetComponent<Hex_Data>().h = 0;
                go.GetComponent<Hex_Data>().parent = null;
            }
        }
    }
    public void pathTraceback(GameObject goal, GameObject start)
    {
        path.Clear();
        GameObject priorChild = goal;

        while (priorChild != start )
        {
            path.Push(priorChild);
            foreach (GameObject potentialChild in closedList)
            {
                if(priorChild.GetComponent<Hex_Data>().parent == potentialChild)
                {
                    priorChild = potentialChild;
                    break;
                }
            }
        }
        path.Push(priorChild);
        path.Pop();
    }

    public void loadLogiOre()
    {
        
    }
    public void Attack(List<Equipment> selectedEq)
    {
        selectedEquipment = selectedEq;
        currentAction = selectedAction.attack;
        manager.somethingIsChosingaTarget = true;
    }

    public void ConstructCity()
    {
        currentAction = selectedAction.buildCity;
        manager.somethingIsChosingaTarget = true;
    }
    public void ConstructMine()
    {
        currentAction = selectedAction.buildMine;
        manager.somethingIsChosingaTarget = true;
    }
    public void ConstructFactory()
    {
        currentAction = selectedAction.buildFactory;
        manager.somethingIsChosingaTarget = true;
    }
    public void ConstructOilWell()
    {
        currentAction = selectedAction.buildOilwell;
        manager.somethingIsChosingaTarget = true;

    }
    public void ConstructRefinery()
    {
        currentAction = selectedAction.buildRefinery;
        manager.somethingIsChosingaTarget = true;
    }
    public void CreateSupplyRoute()
    {
        currentAction = selectedAction.createSupplyRoute;
        manager.somethingIsChosingaTarget = true;
    }
}