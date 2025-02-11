using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    private enum DataType
    {
        Ingredient,
        Upgrade,
    }

    public Image trayImage;
    public SpriteRotator itemImageRotator;
    public LocalizationText itemName;
    public LocalizationText itemDescription;
    public TextMeshProUGUI price;
    public UnityEvent OnBought;

    public GameObject buyButton;
    public GameObject boughtMark;

    private DataType dataType;

    private float storePrice;
    private string itemId;

    public GameObject lockMask;
    public FormattedLocalizationText lockText;

    public void OnBuyButtonClick()
    {
        if (SaveLoadManager.Data.budget > storePrice)
        {
            SaveLoadManager.Data.budget -= storePrice;
            if (dataType == DataType.Ingredient)
                SaveLoadManager.Data.ingredients[itemId] = true;
            else if (dataType == DataType.Upgrade)
                SaveLoadManager.Data.upgrades[itemId] = true;
            SaveLoadManager.Save();
            SetBought();
            OnBought?.Invoke();
        }
    }

    public void Init(IngredientTable.Data data, bool bought, int day)
    {
        dataType = DataType.Ingredient;
        storePrice = data.store_price;
        itemId = data.ingredientID;
        trayImage.sprite = data.spriteDatas.storeTray;
        itemImageRotator.targetSprite = data.spriteDatas.storeSprite;
        itemImageRotator.RotateSprite();
        itemName.SetString(data.stringID.ToString());
        //itemDescription.SetString(ingdata.stringID.ToString());
        price.text = data.store_price.ToString();

        if (bought)
        {
            SetBought();
        }
        else if (day < data.day)
        {
            lockMask.SetActive(true);
            lockText.SetString(data.day.ToString());
        }
    }

    public void Init(StoreTable.Data data, bool bought)
    {
        dataType = DataType.Upgrade;

        storePrice = data.price;
        itemId = data.storeID;

        trayImage.sprite = null;
        itemImageRotator.targetSprite = data.spriteRotatorData.sprites[0];
        itemImageRotator.rotate = data.spriteRotatorData.rotates[0];
        itemImageRotator.RotateSprite();
        itemName.SetString(data.NameID.ToString());
        itemDescription.SetString(data.descriptionID.ToString());
        price.text = data.price.ToString();

        if (bought)
        {
            SetBought();
        }
    }

    public void SetBought()
    {
        itemDescription.TextEnabled = false;
        buyButton.SetActive(false);
        boughtMark.SetActive(true);
    }
}
