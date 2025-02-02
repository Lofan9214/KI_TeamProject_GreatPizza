using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public Image itemImage;
    public LocalizationText itemName;
    public LocalizationText itemDescription;
    public TextMeshProUGUI price;
    public Toggle toggle;
    private IngredientTable.Data ingdata;

    public void OnToggleChanged(bool isOn)
    {
        if (isOn)
        {
            if (SaveLoadManager.Data.currency > ingdata.shopprice)
            {
                SaveLoadManager.Data.currency -= ingdata.shopprice;
                SaveLoadManager.Data.ingredients[ingdata.ingredientID] = true;
                SaveLoadManager.Save();
                price.text = "Bought";

                toggle.interactable = false;
            }
            else
            {
                toggle.isOn = false;
            }
        }
    }

    public void Init(IngredientTable.Data data, bool bought, int day)
    {
        ingdata = data;
        itemImage.sprite = ingdata.Sprite;
        itemName.SetString(ingdata.stringID.ToString());
        itemDescription.SetString(ingdata.stringID.ToString());
        toggle.isOn = bought;

        if (bought)
        {
            price.text = "Bought";
            toggle.interactable = false;
        }
        else if (day < ingdata.unlockday)
        {
            price.text = "Can't Buy";
            toggle.interactable = false;
        }
        else
        {
            price.text = data.shopprice.ToString();
        }
    }
}
