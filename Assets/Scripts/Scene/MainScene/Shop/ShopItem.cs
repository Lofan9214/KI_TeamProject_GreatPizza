using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
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
            if (SaveLoadManager.Data.budget > ingdata.shopprice)
            {
                SaveLoadManager.Data.budget -= ingdata.shopprice;
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
