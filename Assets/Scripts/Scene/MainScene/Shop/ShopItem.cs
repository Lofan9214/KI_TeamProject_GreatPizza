using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public Image trayImage;
    public Image itemImage;
    public LocalizationText itemName;
    public LocalizationText itemDescription;
    public TextMeshProUGUI price;
    public Toggle toggle;
    private IngredientTable.Data ingdata;
    public UnityEvent OnBought;

    public void OnToggleChanged(bool isOn)
    {
        if (isOn)
        {
            if (SaveLoadManager.Data.budget > ingdata.store_price)
            {
                SaveLoadManager.Data.budget -= ingdata.store_price;
                SaveLoadManager.Data.ingredients[ingdata.ingredientID] = true;
                SaveLoadManager.Save();
                price.text = "Bought";
                OnBought?.Invoke();

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
        trayImage.sprite = ingdata.spriteDatas.storeTray;
        itemImage.sprite = ingdata.spriteDatas.storeSprite;
        itemName.SetString(ingdata.stringID.ToString());
        itemDescription.SetString(ingdata.stringID.ToString());
        toggle.isOn = bought;

        if (bought)
        {
            price.text = "Bought";
            toggle.interactable = false;
        }
        else if (day < ingdata.day)
        {
            price.text = "Can't Buy";
            toggle.interactable = false;
        }
        else
        {
            price.text = data.store_price.ToString();
        }
    }
}
