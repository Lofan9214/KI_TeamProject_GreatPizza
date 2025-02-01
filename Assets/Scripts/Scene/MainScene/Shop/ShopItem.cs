using System.Collections;
using System.Collections.Generic;
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

        price.text = data.shopprice.ToString();
        if (bought || day < ingdata.unlockday)
        {
            toggle.interactable = false;
        }
    }
}
