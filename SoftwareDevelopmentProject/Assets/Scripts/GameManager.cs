using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject BuildingUI;
    public GameObject SquadUI;
    public GameObject CityUI;
    public bool somethingIsChosingaTarget = false;
    public GameObject SelectedTile;
    public List<unit> ResearchedUnits;
    public List<unit> ResearchedEquipment;
    public List<GameObject> Playerlist;
    public PlayerManager currentPlayerTurn;
    public int playerIndex = 0;
    public GameObject AlwaysOn;
    public GameObject VictoryScreen;

    public bool victory = false;

    // Start is called before the first frame update
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        if (!somethingIsChosingaTarget && currentPlayerTurn.realPlayer == true)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        BuildingUI.SetActive(false);
                        CityUI.SetActive(false);
                        SquadUI.SetActive(false);
                        if (SelectedTile != null)
                        {
                            SelectedTile.transform.parent.GetChild(1).gameObject.SetActive(false);
                        }
                        SelectedTile = hit.collider.gameObject;
                        SelectedTile.transform.parent.GetChild(1).gameObject.SetActive(true);
                        if (SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile != null && SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile.tag == "city")
                        {
                            BuildingUI.SetActive(false);
                            SquadUI.SetActive(false);
                            CityUI.SetActive(true);
                        }
                        else if (SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile != null && SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile.tag == "squad")
                        {
                            BuildingUI.SetActive(false);
                            CityUI.SetActive(false);
                            SquadUI.SetActive(true);
                        }
                        else if (SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile != null && (SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile.tag == "mine" || SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile.tag == "factory" || SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile.tag == "oilWell"|| SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile.tag == "refinery"))
                        {
                            CityUI.SetActive(false);
                            SquadUI.SetActive(false);
                            BuildingUI.GetComponent<RessourceBuildingUi>().building = SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile;
                            BuildingUI.SetActive(true);
                        }
                    }

                }
                if (Input.GetMouseButtonDown(1))
                {
                    if (SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile != null)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        {
                            if (SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile.tag == "squad")
                            {
                                if (SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile.GetComponent<SquadData>().Ambhibious == true)
                                {
                                    SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile.GetComponent<SquadBehaviour>().AmphibiousMovement(hit.collider.gameObject.transform.parent.gameObject);
                                }
                                else if(SelectedTile.transform.parent.GetComponent<Hex_Data>().moveCostLand > 0)
                                {
                                    SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile.GetComponent<SquadBehaviour>().Movement(hit.collider.gameObject.transform.parent.gameObject);
                                }
                            }
                        }
                    }
                }
            }
           
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ChangeTurn()
    {
        playerIndex++;
        if (playerIndex == Playerlist.Count )
        {
            playerIndex = 0;
        }
        Playerlist[playerIndex].GetComponent<PlayerManager>().OnTurnStart();

        BuildingUI.SetActive(false);
        CityUI.SetActive(false);
        SquadUI.SetActive(false);
        if (!Playerlist[playerIndex].GetComponent<PlayerManager>().realPlayer && Playerlist[playerIndex].GetComponent<PlayerManager>().cityList.Count == 0)
        {
            victory = true;
            AlwaysOn.SetActive(false);
            VictoryScreen.SetActive(true);
        }
        
    }

}

[System.Serializable]
public class Equipment
{
    public bool ready = true;

    public int MaxOreTransport = 0;
    public int MaxSupplyTransport = 0;
    public int MaxOilTransport = 0;
    public int MaxfuelTransport = 0;

    public string EquipmentType;
    public string name;
    public int salvoSize;
    public int fragmentation;
    public int accuracy;
    public int armorPenetration;
    public int damage;
    public string specialTraits;
    public int supplyUsage;
    public int range;

    public Equipment(Equipment toBeClonedEquipment)
    {
        MaxOreTransport = toBeClonedEquipment.MaxOreTransport;
        MaxSupplyTransport = toBeClonedEquipment.MaxSupplyTransport;
        MaxOilTransport = toBeClonedEquipment.MaxOilTransport;
        MaxfuelTransport= toBeClonedEquipment.MaxfuelTransport;
        if(toBeClonedEquipment != null)
        {
            EquipmentType = toBeClonedEquipment.EquipmentType;
        }
        name = toBeClonedEquipment.name;
        salvoSize = toBeClonedEquipment.salvoSize;
        fragmentation = toBeClonedEquipment.fragmentation;
        accuracy = toBeClonedEquipment.accuracy;
        armorPenetration = toBeClonedEquipment.armorPenetration;
        damage = toBeClonedEquipment.damage;
        specialTraits = toBeClonedEquipment.specialTraits;
        supplyUsage = toBeClonedEquipment.supplyUsage;
        range = toBeClonedEquipment.range;
    }
    public Equipment(string Name, int SalvoSize, int Fragmentation, int Accuracy, int ArmorPenetration, int Damage, string SpecialTraits, int SupplyUsage, int Range, string equipmentType)
    {
        name = Name;
        salvoSize = SalvoSize;
        fragmentation = Fragmentation;
        accuracy = Accuracy;
        armorPenetration = ArmorPenetration;
        damage = Damage;
        specialTraits = SpecialTraits;
        supplyUsage = SupplyUsage;
        range = Range;
        EquipmentType = equipmentType;

        if(EquipmentType == "logi")
        {
            XmlDocument equipmentData = new XmlDocument();
            TextAsset equipmentdataText = Resources.Load<TextAsset>("XML/Unit Data/Equipment");
            equipmentData.LoadXml(equipmentdataText.text);

            XmlNodeList eqList = equipmentData.GetElementsByTagName("unitEquipment");
            foreach(XmlNode eqData in eqList)
            {
                if(eqData["Name"].InnerText == name)
                {
                    MaxOreTransport = int.Parse(eqData["OreTransport"].InnerText);
                    MaxSupplyTransport = int.Parse(eqData["SupplyTransport"].InnerText);
                    MaxOilTransport = int.Parse(eqData["OilTransport"].InnerText);
                    MaxfuelTransport = int.Parse(eqData["FuelTransport"].InnerText);
                }
            }

        }
        else
        {
            MaxOreTransport = 0;
            MaxSupplyTransport = 0;
            MaxOilTransport = 0;
            MaxfuelTransport = 0;
        }
    }
}
[System.Serializable]
public class unit
{
    public int productionTime;
    public string unitType;
    public string unitName;
    public int health;
    public int armor;
    public int unitSize;
    public int stealth;
    public int optics;
    public int movement;
    public int supplies;
    public Equipment equipment;
    public int cost;

    public unit() { }

    public unit(unit toBeClonedUnit) 
    {
        this.unitName = toBeClonedUnit.unitName;
        this.health = toBeClonedUnit.health;
        this.armor = toBeClonedUnit.armor;
        this.unitSize = toBeClonedUnit.unitSize;
        this.stealth = toBeClonedUnit.stealth;
        this.optics = toBeClonedUnit.optics;
        this.movement = toBeClonedUnit.movement;
        this.supplies = toBeClonedUnit.supplies;
        this.unitType = toBeClonedUnit.unitType;
        this.cost = toBeClonedUnit.cost;
        this.productionTime = toBeClonedUnit.productionTime;
        if(toBeClonedUnit.equipment != null) 
        {
            this.equipment = new Equipment(toBeClonedUnit.equipment);
        }
        
    }
    public unit(string unitName, int health, int armor, int unitSize, int stealth, int optics, int movement, int supplies, string unitType,int cost, int productionTime)
    {
        this.unitName = unitName;
        this.health = health;
        this.armor = armor;
        this.unitSize = unitSize;
        this.stealth = stealth;
        this.optics = optics;
        this.movement = movement;
        this.supplies = supplies;
        this.unitType = unitType;
        this.cost = cost;
        this.productionTime = productionTime;
    }
}
[System.Serializable]
public class UnitTemplate
{
    public int productionTime;
    public string templateName;
    public List<unit> units;
    public int templateCost;

    public UnitTemplate(UnitTemplate toBeCloned)
    {
        this.templateName = toBeCloned.templateName;
        this.units = toBeCloned.units;
        this.templateCost = toBeCloned.templateCost;
        this.productionTime = toBeCloned.productionTime;
    }

    public UnitTemplate(string templateName, List<unit> units, int templateCost, int productionTime)
    {
        this.templateName = templateName;
        this.units = units;
        this.templateCost = templateCost;
        this.productionTime = productionTime;
    }
    
}
