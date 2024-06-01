using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;
using JetBrains.Annotations;
using System;

public class UnitCreatorManager : MonoBehaviour
{
    public GameObject SquadCreator;
    public CreateSquadWindow squadProduction;
    public TMP_InputField nameInput;
    public GameObject warning;
    public Button Confirm;
    public TextMeshProUGUI squadSizeText;
    public PlayerManager playerManager;
    public GameObject SelectorPrefab;
    public GameObject SelcetorGrid;
    public int currentSelectors = 1;
    public List<GameObject> SelectorList;
    public int CurrentSquadSize;
    public int maxSquadsize = 20;

    // Start is called before the first frame update
    void Start()
    {
        updateSelectors();
        updateEquipmentLists();
        foreach (var selector in SelectorList)
        {
            selector.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate { updateEquipmentLists(); });
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void updateSelectors()
    {
        List<string> temp = new List<string>();
        foreach (unit units in playerManager.unlockedUnits)
        {
            temp.Add(units.unitName);
        }
        foreach (var selector in SelectorList)
        {
            selector.GetComponent<TMP_Dropdown>().ClearOptions();
            selector.GetComponent<TMP_Dropdown>().AddOptions(temp);
        }
    }
    public void removeSelector()
    {
        if(SelectorList.Count > 1)
        {
            Destroy(SelectorList.Last());
            SelectorList.RemoveAt(SelectorList.Count - 1);
            currentSelectors--;
            updateEquipmentLists();
        }
    }
    public void AddNewSelector()
    {
        if (currentSelectors < maxSquadsize)
        {
            SelectorList.Add(Instantiate(SelectorPrefab, SelcetorGrid.transform));
            updateSelectors();
            updateEquipmentLists();
            currentSelectors++;    
        }
        foreach (var selector in SelectorList)
        {
            selector.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate { updateEquipmentLists(); });
        }
    }
    public void updateEquipmentLists()
    {    
        CurrentSquadSize = 0;
        foreach (var selector in SelectorList) 
        {
            List<string> temp = new List<string>();
            selector.transform.GetChild(3).GetComponent<TMP_Dropdown>().ClearOptions();
            string selectedName = selector.GetComponent<TMP_Dropdown>().options[selector.GetComponent<TMP_Dropdown>().value].text;
            unit selected = new unit();
            foreach (var unit in playerManager.unlockedUnits)
            {
                if(selectedName == unit.unitName)
                {
                    selected = unit;
                }
            }
            if(selected.unitName != null)
            {
                foreach (Equipment eq in playerManager.unlockedEquipment)
                {
                    if (eq.EquipmentType == selected.unitType)
                        temp.Add(eq.name);
                }
            }
            selector.transform.GetChild(3).GetComponent<TMP_Dropdown>().AddOptions(temp);
            CurrentSquadSize += selected.unitSize;
            squadSizeText.text = CurrentSquadSize + "/" + maxSquadsize;
            if (CurrentSquadSize > maxSquadsize)
            {
                warning.SetActive(true);
                Confirm.interactable = false;
            }
            else
            {
                warning.SetActive(false);
                Confirm.interactable = true;
            }
        }
    }
    public void OnCancel()
    {
        for (int i = SelectorList.Count; i > 1; i--)
        {
            if (SelectorList.Count > 1)
            {
                Destroy(SelectorList.Last());
                SelectorList.RemoveAt(SelectorList.Count - 1);
                currentSelectors--;
            }
        }
        updateEquipmentLists();
        SquadCreator.SetActive(false);
    }

    public void OnConfirm()
    {
        List<unit> UnitTemplate = new List<unit>();
        foreach (var selector in SelectorList)
        {
            string selectedName = selector.GetComponent<TMP_Dropdown>().options[selector.GetComponent<TMP_Dropdown>().value].text;
            foreach (unit units in playerManager.unlockedUnits)
            {
                if (selectedName == units.unitName)
                {
                    UnitTemplate.Add(new unit(units));
                    string equipmentName = selector.transform.GetChild(3).GetComponent<TMP_Dropdown>().options[selector.transform.GetChild(3).GetComponent<TMP_Dropdown>().value].text;
                    foreach (Equipment eq in playerManager.unlockedEquipment)
                    {
                        if (eq.name == equipmentName)
                        {
                            UnitTemplate.Last<unit>().equipment = eq;
                            break;
                        }
                    } 
                }
                break;
            }
        }
        int cost = 0;
        foreach (var unit in UnitTemplate)
        {
            cost += unit.cost;
        }

        int time = 0;
        foreach (var unit in UnitTemplate)
        {
            if(unit.productionTime > time)
            {
                time = unit.productionTime;
            }
        }

        UnitTemplate template = new UnitTemplate(nameInput.text, UnitTemplate,cost,time);
        squadProduction.PlayerManager.UnitTemplateList.Add(template);

        for (int i = SelectorList.Count; i> 1; i-- )
        {
            if (SelectorList.Count > 1)
            {
                Destroy(SelectorList.Last());
                SelectorList.RemoveAt(SelectorList.Count - 1);
                currentSelectors--;
            }
        }
        updateEquipmentLists();
        SquadCreator.SetActive(false);
    }
}

