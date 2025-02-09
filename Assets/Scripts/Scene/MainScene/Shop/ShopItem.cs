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
    private IngredientTable.Data ingdata;
    public UnityEvent OnBought;

    public GameObject buyButton;
    public GameObject boughtMark;

    public GameObject lockMask;
    public FormattedLocalizationText lockText;

    public void OnBuyButtonClick()
    {
        if (SaveLoadManager.Data.budget > ingdata.store_price)
        {
            SaveLoadManager.Data.budget -= ingdata.store_price;
            SaveLoadManager.Data.ingredients[ingdata.ingredientID] = true;
            SaveLoadManager.Save();
            SetBought();
            OnBought?.Invoke();
        }
    }

    public void Init(IngredientTable.Data data, bool bought, int day)
    {
        ingdata = data;
        trayImage.sprite = ingdata.spriteDatas.storeTray;
        itemImage.sprite = ingdata.spriteDatas.storeSprite;
        itemName.SetString(ingdata.stringID.ToString());
        //itemDescription.SetString(ingdata.stringID.ToString());
        price.text = data.store_price.ToString();

        if (bought)
        {
            SetBought();
        }
        else if (day < ingdata.day)
        {
            lockMask.SetActive(true);
            lockText.SetString(ingdata.day.ToString());
        }
    }

    public void SetBought()
    {
        itemDescription.TextEnabled = false;
        buyButton.SetActive(false);
        boughtMark.SetActive(true);
    }
}
