using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class SquadData : MonoBehaviour
{
    public GameObject SoliderModel;
    public GameObject egineermodel;
    public GameObject logiModel;
    public GameObject vehicleModel;
    public GameObject zombieModel;

    public bool Ambhibious = false;

    public int MaxOreTransport = 0;
    public int MaxSupplyTransport = 0;
    public int MaxOilTransport = 0;
    public int MaxfuelTransport = 0;

    public int crntOreTransport = 0;
    public int crntSupplyTransport = 0;
    public int crntOilTransport = 0;
    public int crntfuelTransport = 0;

    public SquadBehaviour SquadBehaviour;
    public UnitTemplate squadTemplate;
    public string squadName;
    public List<unit> currentUnitState;
    public int currentEquipment;
    public int MaxEquipment;
    void Start()
    {

        foreach (var unit in squadTemplate.units)
        {
            currentUnitState.Add(new unit(unit));
        }
        squadName = squadTemplate.templateName;
        int lowestMovement = currentUnitState[0].movement;
        foreach (unit unit in currentUnitState)
        {
            if (unit.unitType == "amphTransportVehicle"|| unit.unitType == "zombie")
            {
                Ambhibious = true;
            }
            if (unit.unitType == "transport" || unit.unitType == "amphTransportVehicle")
            {
                lowestMovement = unit.movement;
                break;
            }
            else if (unit.movement < lowestMovement)
            {
                lowestMovement = unit.movement;
            }

        }

        foreach (unit unit in currentUnitState)
        {
            if (unit.equipment.EquipmentType == "logi")
            {
                MaxOilTransport += unit.equipment.MaxOilTransport;
                MaxOreTransport += unit.equipment.MaxOreTransport;
                MaxSupplyTransport += unit.equipment.MaxSupplyTransport;
                MaxfuelTransport += unit.equipment.MaxfuelTransport;
            }
        }

        SquadBehaviour.maxMovement = lowestMovement;
        SquadBehaviour.leftMovement = lowestMovement;

        int tempSupplies = 0;
        foreach (unit unit in currentUnitState)
        {
            tempSupplies += unit.supplies;
        }
        MaxEquipment = tempSupplies;
        currentEquipment = MaxEquipment;

        if(this.gameObject.tag == "enemySquad")
        {
            //Instantiate(zombieModel, this.transform);
            this.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            int temp = 0;
            foreach (unit unit in squadTemplate.units)
            {
                if(unit.unitType == "civilEngineers")
                {
                    temp = 1;
                    break;
                }else if (unit.unitType == "logi")
                {
                    temp = 2;
                    break;
                }
            }

            switch (temp)
            {
                case 0:
                    Instantiate(SoliderModel, this.transform);
                    break; 
                case 1:
                    Instantiate(egineermodel, this.transform);
                    break;
                case 2:
                    this.gameObject.GetComponent<MeshRenderer>().enabled = true;
                    break;
            }
            
        }
        SquadBehaviour.OnTurnStart();
    }



    // Update is called once per frame
    void Update()
    {
        
    }

}
