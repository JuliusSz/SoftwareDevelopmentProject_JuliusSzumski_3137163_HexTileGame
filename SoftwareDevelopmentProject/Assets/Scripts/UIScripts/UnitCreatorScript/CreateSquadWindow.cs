using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateSquadWindow : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text oilText;
    public TMP_Text FuelText;
    public TMP_Text OreText;
    public TMP_Text SupplyText;
    public TMP_Text costText;

    public PlayerManager PlayerManager;
    public GameManager manager;
    public Button confirmButton;
    public TMP_Dropdown templateSelector;
    public GameObject SquadCreator;
    int oldLength;
    public CityData selectedCity;
    public GameObject warning;
    
    void Start()
    {
        OnSelectedTemplateChange();
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerManager.UnitTemplateList.Count != oldLength)
        {
            List<string> templateNameList = new List<string>();
            foreach (var unitTemplate in PlayerManager.UnitTemplateList)
            {
                templateNameList.Add(unitTemplate.templateName);
            }
            templateSelector.ClearOptions();
            templateSelector.AddOptions(templateNameList);
            oldLength = PlayerManager.UnitTemplateList.Count;
        }
        oilText.text = selectedCity.storedOil + "/" + selectedCity.maxOilStorage;
        FuelText.text = selectedCity.storedFuel + "/" + selectedCity.maxFuelStorage;
        OreText.text = selectedCity.storedOre + "/" + selectedCity.maxOreStorage;
        SupplyText.text = selectedCity.storedEquipment + "/" + selectedCity.maxOilStorage;
    }

    private void OnEnable()
    {
        selectedCity = manager.SelectedTile.transform.parent.GetComponent<Hex_Data>().whatsOnThisTile.GetComponent<CityData>();
        OnSelectedTemplateChange();

        oilText.text = selectedCity.storedOil + "/" + selectedCity.maxOilStorage;
        FuelText.text = selectedCity.storedFuel + "/" + selectedCity.maxFuelStorage;
        OreText.text = selectedCity.storedOre + "/" + selectedCity.maxOreStorage;
        SupplyText.text = selectedCity.storedEquipment + "/" + selectedCity.maxOilStorage;
    }

    public void OpenSquadCreator()
    {
        SquadCreator.SetActive (true);
    }

    public void OnSelectedTemplateChange()
    {
        string selecetedTemplate = templateSelector.options[templateSelector.value].text;

        foreach (var unitTemplate in PlayerManager.UnitTemplateList)
        {
            if (unitTemplate.templateName == selecetedTemplate)
            {
                costText.text = "cost: " + unitTemplate.templateCost;
                if (unitTemplate.templateCost > selectedCity.storedEquipment)
                {
                    confirmButton.interactable = false;
                    warning.SetActive(true);
                }
                else
                {
                    confirmButton.interactable = true;
                    warning.SetActive(false);
                }
            }
            
        }
    }

    public void onEnque()
    {
        string selecetedTemplate = templateSelector.options[templateSelector.value].text;

        foreach(var unitTemplate in PlayerManager.UnitTemplateList) 
        {
            if(selecetedTemplate == unitTemplate.templateName)
            {
                selectedCity.storedEquipment -= unitTemplate.templateCost;
                selectedCity.BuildQueue.Enqueue(new UnitTemplate(unitTemplate));

            }
        }
        OnSelectedTemplateChange();
    }
}
