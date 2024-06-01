using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SquadUIHandler : MonoBehaviour
{
    public TMP_Text SquadTitle;
    public TMP_Text EquipmentText;
    public TMP_Text movementText;
    public GameObject UiEquipmentPrefab;
    public List<GameObject> uiPoints = new List<GameObject>();
    public GameObject selectedSquad;
    public GameManager gameManager;
    Dictionary<Equipment, List<Equipment>> EquipmentDir = new Dictionary<Equipment, List<Equipment>>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movementText.text = "Movement: "+selectedSquad.GetComponent<SquadBehaviour>().leftMovement + "/" + selectedSquad.GetComponent<SquadBehaviour>().maxMovement;
        
    }
    private void OnEnable()
    {
        selectedSquad = gameManager.SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile;
        SquadTitle.text = selectedSquad.GetComponent<SquadData>().squadName;
        EquipmentText.text = "Supplies: "+selectedSquad.GetComponent<SquadData>().currentEquipment + "/" + selectedSquad.GetComponent<SquadData>().MaxEquipment;

        EquipmentDir.Add(selectedSquad.GetComponent<SquadData>().currentUnitState[0].equipment, new List<Equipment>());
        foreach (unit aliveUnit in selectedSquad.GetComponent<SquadData>().currentUnitState)
        {
            bool equipmentFound = false;
            var keys = new List<Equipment>(EquipmentDir.Keys);

            for (int i = 0; i < keys.Count; i++)
            {
                Equipment key = keys[i];
                if (key.name == aliveUnit.equipment.name)
                {
                    EquipmentDir[key].Add(aliveUnit.equipment);
                    equipmentFound = true;
                    break;
                }
            }

            if (!equipmentFound)
            {
                EquipmentDir.Add(aliveUnit.equipment, new List<Equipment> { aliveUnit.equipment });
            }

        }
        foreach (KeyValuePair<Equipment, List<Equipment>> eqWithCount in EquipmentDir)
        {
            if(eqWithCount.Key.specialTraits == "CivilEngineering")
            {
                GameObject temp;
                temp = Instantiate(UiEquipmentPrefab, this.transform);
                temp.transform.GetChild(0).GetComponent<TMP_Text>().text = "Build a city";
                temp.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { selectedSquad.GetComponent<SquadBehaviour>().ConstructCity(); });
                uiPoints.Add(temp);

                temp = Instantiate(UiEquipmentPrefab, this.transform);
                temp.transform.GetChild(0).GetComponent<TMP_Text>().text = "Build a Mine";
                temp.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { selectedSquad.GetComponent<SquadBehaviour>().ConstructMine(); });
                uiPoints.Add(temp);

                temp = Instantiate(UiEquipmentPrefab, this.transform);
                temp.transform.GetChild(0).GetComponent<TMP_Text>().text = "Build a Factory";
                temp.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { selectedSquad.GetComponent<SquadBehaviour>().ConstructFactory(); });
                uiPoints.Add(temp);

                temp = Instantiate(UiEquipmentPrefab, this.transform);
                temp.transform.GetChild(0).GetComponent<TMP_Text>().text = "Build an Oilwell";
                temp.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { selectedSquad.GetComponent<SquadBehaviour>().ConstructOilWell(); });
                uiPoints.Add(temp);

                temp = Instantiate(UiEquipmentPrefab, this.transform);
                temp.transform.GetChild(0).GetComponent<TMP_Text>().text = "Build a refinery";
                temp.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { selectedSquad.GetComponent<SquadBehaviour>().ConstructRefinery(); });
                uiPoints.Add(temp);
            }else if(eqWithCount.Key.specialTraits == "logi")
            {
                GameObject temp;
                temp = Instantiate(UiEquipmentPrefab, this.transform);
                temp.transform.GetChild(0).GetComponent<TMP_Text>().text = "Select two buildings to establish a supply route between them" ;
                temp.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { selectedSquad.GetComponent<SquadBehaviour>().CreateSupplyRoute(); });
                uiPoints.Add(temp);
            }
            else{
                GameObject temp;
                temp = Instantiate(UiEquipmentPrefab, this.transform);
                temp.transform.GetChild(0).GetComponent<TMP_Text>().text = eqWithCount.Key.name + " x " + eqWithCount.Value.Count;
                temp.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { selectedSquad.GetComponent<SquadBehaviour>().Attack(eqWithCount.Value); });
                uiPoints.Add(temp);

            }
            
            
        }
    }
    private void OnDisable()
    {
        EquipmentDir.Clear();
        foreach (GameObject item in uiPoints)
        {
            Destroy(item);
        }
        uiPoints.Clear();
        selectedSquad = null;
    }
}
