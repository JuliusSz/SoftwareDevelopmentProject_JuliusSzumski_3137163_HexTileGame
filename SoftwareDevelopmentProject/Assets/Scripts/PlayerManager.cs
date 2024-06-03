using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using System;
using Unity.VisualScripting;
using JetBrains.Annotations;

public class PlayerManager : MonoBehaviour
{
    public GameObject zombieCityPrefab;
    public GameObject Squadprefab;
    public Grid map;
    public List<UnitTemplate> UnitTemplateList = new List<UnitTemplate>();
    public GameObject cityUi;
    public GameManager GameManager;
    public bool realPlayer;
    public List<unit> unlockedUnits = new List<unit>();
    public List<Equipment> unlockedEquipment = new List<Equipment>();
    public List<string> completedResearch;

    public List<GameObject> squadList;
    public List<GameObject> cityList;
    public List<GameObject> mineList;
    public List<GameObject> factoryList;
    public List<GameObject> oilwellList;
    public List<GameObject> refineryList;

    XmlDocument unitData = new XmlDocument();
    XmlDocument equipmentData = new XmlDocument();
    // Start is called before the first frame update
    void Start()
    {
        TextAsset unitdataText = Resources.Load<TextAsset>("XML/Unit Data/UnitData");
        unitData.LoadXml(unitdataText.text);
        TextAsset equipmentdataText = Resources.Load<TextAsset>("XML/Unit Data/Equipment");
        equipmentData.LoadXml(equipmentdataText.text);
        OnTurnStart();
        createStartingTemplate();

        if (realPlayer)
        {
            foreach (GameObject tile in map.objMap)
            {
                List<GameObject> SurroundingTiles = new List<GameObject>();
                Vector2 tilePos = tile.GetComponent<Hex_Data>().TilePosition;
                if (tilePos.y % 2 == 0)
                {
                    if (tilePos.x + 1 < map.size)
                    {
                        SurroundingTiles.Add(map.objMap[(int)tilePos.x + 1, (int)tilePos.y]);
                    }

                    if (tilePos.y + 1 < map.size)
                    {
                        SurroundingTiles.Add(map.objMap[(int)tilePos.x, (int)tilePos.y + 1]);
                    }

                    if (tilePos.x - 1 > 0 && tilePos.y + 1 < map.size)
                    {
                        SurroundingTiles.Add(map.objMap[(int)tilePos.x - 1, (int)tilePos.y + 1]);
                    }

                    if (tilePos.x - 1 >= 0)
                    {
                        SurroundingTiles.Add(map.objMap[(int)tilePos.x - 1, (int)tilePos.y]);
                    }

                    if (tilePos.x - 1 >= 0 && tilePos.y - 1 >= 0)
                    {
                        SurroundingTiles.Add(map.objMap[(int)tilePos.x - 1, (int)tilePos.y - 1]);
                    }

                    if (tilePos.y - 1 >= 0)
                    {
                        SurroundingTiles.Add(map.objMap[(int)tilePos.x, (int)tilePos.y - 1]);
                    }
                }
                else
                {
                    if (tilePos.x + 1 < map.size)
                    {
                        SurroundingTiles.Add(map.objMap[(int)tilePos.x + 1, (int)tilePos.y]);
                    }
                    if (tilePos.x + 1 < map.size && tilePos.y + 1 < map.size)
                    {
                        SurroundingTiles.Add(map.objMap[(int)tilePos.x + 1, (int)tilePos.y + 1]);
                    }
                    if (tilePos.y + 1 < map.size)
                    {
                        SurroundingTiles.Add(map.objMap[(int)tilePos.x, (int)tilePos.y + 1]);
                    }
                    if (tilePos.x - 1 >= 0)
                    {
                        SurroundingTiles.Add(map.objMap[(int)tilePos.x - 1, (int)tilePos.y]);
                    }
                    if (tilePos.y - 1 >= 0)
                    {
                        SurroundingTiles.Add(map.objMap[(int)tilePos.x, (int)tilePos.y - 1]);
                    }
                    if (tilePos.x + 1 < map.size && tilePos.y - 1 >= 0)
                    {
                        SurroundingTiles.Add(map.objMap[(int)tilePos.x + 1, (int)tilePos.y - 1]);
                    }
                }
                bool start = true;

                foreach (GameObject surrtile in SurroundingTiles)
                {
                    if (surrtile.GetComponent<Hex_Data>().moveCostWater == 1 || SurroundingTiles.Count < 6 && surrtile.GetComponent<Hex_Data>().whatsOnThisTile == null)
                    {
                        start = false;
                        break;
                    }
                }
                if (start == true)
                {
                    GameObject riflePrefab = Squadprefab;
                    GameObject engineerSquadPrefab = Squadprefab;
                    GameObject NewEngineerSquadPrefab = Instantiate(engineerSquadPrefab, new Vector3(tile.transform.position.x, 3, tile.transform.position.z), new Quaternion(), this.transform);
                    GameObject NewRiflePrefab = Instantiate(riflePrefab, new Vector3(SurroundingTiles[0].transform.position.x, 3, SurroundingTiles[0].transform.position.z), new Quaternion(), this.transform);

                    NewEngineerSquadPrefab.GetComponent<SquadData>().squadTemplate = UnitTemplateList[1];
                    NewRiflePrefab.GetComponent<SquadData>().squadTemplate = UnitTemplateList[0];
                    NewEngineerSquadPrefab.GetComponent<SquadBehaviour>().currentTile = tile;
                    NewRiflePrefab.GetComponent<SquadBehaviour>().currentTile = SurroundingTiles[0];
                    squadList.Add(NewEngineerSquadPrefab);
                    squadList.Add(NewRiflePrefab);
                    Camera.main.transform.position = new Vector3(tile.transform.position.x, Camera.main.transform.position.y, tile.transform.position.z);
                    break;
                }
            }
        }
        else{
            foreach(GameObject tile in map.objMap)
            {
                if (tile.GetComponent<Hex_Data>().terrainData.terain == Hex.terraintype.plains && tile.GetComponent<Hex_Data>().whatsOnThisTile == null) 
                {
                    float randResult = UnityEngine.Random.Range(0,100);
                    if(randResult < 0.1) 
                    {
                        tile.GetComponent<Hex_Data>().whatsOnThisTile = Instantiate(zombieCityPrefab,new Vector3(tile.transform.position.x,3, tile.transform.position.z),new Quaternion(),this.transform);
                        cityList.Add(tile.GetComponent<Hex_Data>().whatsOnThisTile);
                        tile.GetComponent<Hex_Data>().whatsOnThisTile.GetComponent<zombieCity>().Tile = tile;
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
   
    } 
    public void OnTurnStart()
    {
        completedResearch.Clear();
        unlockedUnits.Clear();
        unlockedEquipment.Clear();
        if (realPlayer)
        {
            completedResearch.Add("none");
        }
        else
        {
            completedResearch.Add("zombie");
        }
        

        XmlNodeList unitList = unitData.GetElementsByTagName("Unit");
        XmlNodeList equipmentList = equipmentData.GetElementsByTagName("unitEquipment");

        foreach (string research in completedResearch)
        {
            foreach (XmlNode unit in unitList)
            {
                if (unit.Attributes["research"].Value == research)
                {
                    unlockedUnits.Add(
                        new unit(
                            unit["Name"].InnerText,
                            int.Parse(unit["health"].InnerText),
                            int.Parse(unit["Armor"].InnerText),
                            int.Parse(unit["UnitSize"].InnerText),
                            int.Parse(unit["Stealth"].InnerText),
                            int.Parse(unit["Optics"].InnerText),
                            int.Parse(unit["Movement"].InnerText),
                            int.Parse(unit["Supplies"].InnerText),
                            unit.Attributes["Type"].Value,
                            int.Parse(unit["Cost"].InnerText),
                            int.Parse(unit["trainingTime"].InnerText)
                        )
                    );
                }
            }

            foreach (XmlNode equipment in equipmentList)
            {
                if (equipment.Attributes["research"].Value == research)
                {
                    unlockedEquipment.Add(
                        new Equipment(
                            equipment["Name"].InnerText,
                            int.Parse(equipment["SalvoSize"].InnerText),
                            int.Parse(equipment["Fragmentation"].InnerText),
                            int.Parse(equipment["Accuracy"].InnerText),
                            int.Parse(equipment["ArmorPenetration"].InnerText),
                            int.Parse(equipment["Damage"].InnerText),
                            equipment["SpecialTraits"].InnerText,
                            int.Parse(equipment["SupplyUsage"].InnerText),
                            int.Parse(equipment["Range"].InnerText),
                            equipment.Attributes["Type"].Value
                        )
                    );
                }
            }
            foreach(GameObject squad in squadList)
            {
                if (realPlayer)
                {
                    squad.GetComponent<SquadBehaviour>().OnTurnStart();
                }
                else
                {
                    squad.GetComponent<SquadBehaviour>().OnZombieTurnStart();
                }
                
            }
            foreach(GameObject city in cityList)
            {
                if (realPlayer)
                {
                    city.GetComponent<CityData>().OnTurnStart();
                }
                else
                {
                    city.GetComponent<zombieCity>().OnTurnStart();
                }
                
            }
            foreach (GameObject mine in mineList)
            {
                mine.GetComponent<MineScript>().OnTurnStart();
            }
            foreach (GameObject refinery in refineryList)
            {
                refinery.GetComponent<RefineryScript>().OnTurnStart();
            }
            foreach (GameObject oilwell in oilwellList)
            {
                oilwell.GetComponent<OilwellScript>().OnTurnStart();
            }
            foreach (GameObject factory in factoryList)
            {
                factory.GetComponent<FactoryScript>().OnTurnStart();
            }
            if(realPlayer== false && GameManager.currentPlayerTurn == this)
            {
                GameManager.ChangeTurn();
            }
        }
    }
    public void createStartingTemplate()
    {
        if (realPlayer ==true)
        {
            List<unit> rifleUnits = new List<unit>();
            List<unit> engineerUnit = new List<unit>();
            UnitTemplate startMilitiaTemplate;
            UnitTemplate startEngineerTemplate;

            for (int i = 0; i < 20; i++)
            {
                foreach (unit unlockedUnit in unlockedUnits)
                {
                    if (unlockedUnit.unitName == "Militia")
                    {
                        unit tempUnit = unlockedUnit;
                        foreach (Equipment equipment in unlockedEquipment)
                        {
                            if (equipment.name == "Ar-16")
                            {
                                tempUnit.equipment = new Equipment(equipment);
                                rifleUnits.Add(unlockedUnit);
                                break;
                            }
                        }
                    }
                }
            }

            foreach (unit unlockedUnit in unlockedUnits)
            {
                if (unlockedUnit.unitName == "Civil Engineering Squad")
                {
                    unit tempUnit = unlockedUnit;
                    foreach (Equipment equipment in unlockedEquipment)
                    {
                        if (equipment.name == "Civil Build tools")
                        {
                            tempUnit.equipment = new Equipment(equipment);
                            engineerUnit.Add(unlockedUnit);
                            break;
                        }
                    }
                }
            }
            int rifleCost = 0;
            foreach (var unit in rifleUnits)
            {
                rifleCost += unit.cost;
            }
            int engineerCost = 0;
            foreach (var unit in engineerUnit)
            {
                engineerCost += unit.cost;
            }
            int rifleTime = 0;
            foreach (var unit in rifleUnits)
            {
                if (unit.productionTime > rifleTime)
                {
                    rifleTime = unit.productionTime;
                }
            }
            int engineerTime = 0;
            foreach (var unit in engineerUnit)
            {
                if (unit.productionTime > engineerTime)
                {
                    engineerTime = unit.productionTime;
                }
            }
            startMilitiaTemplate = new UnitTemplate("Basic Militia", rifleUnits, rifleCost, rifleTime);
            startEngineerTemplate = new UnitTemplate("civil Engineers", engineerUnit, engineerCost, engineerTime);
            UnitTemplateList.Add(startMilitiaTemplate);
            UnitTemplateList.Add(startEngineerTemplate);
        }
        else
        {
            List<unit> zombies = new List<unit>();
            for (int i = 0; i < 20; i++)
            {
                foreach (unit unlockedUnit in unlockedUnits)
                {
                    if (unlockedUnit.unitName == "walker")
                    {
                        unit tempUnit = unlockedUnit;
                        foreach (Equipment equipment in unlockedEquipment)
                        {
                            if (equipment.name == "bite")
                            {
                                tempUnit.equipment = new Equipment(equipment);
                                zombies.Add(unlockedUnit);
                                break;
                            }
                        }
                    }
                }
            }
            UnitTemplate zombieHordeTemplate = new UnitTemplate("Zombie horde", zombies, 0, 0);
            UnitTemplateList.Add(zombieHordeTemplate);
        }
        
    }
}
