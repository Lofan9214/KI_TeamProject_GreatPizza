using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRotator))]
public class OvenSpriteChanger : MonoBehaviour
{
    public SpriteRotatorData spritesData;
    private SpriteRotator rotator;
    private IngameGameManager gameManager;
    private bool upgraded;

    private void Awake()
    {
        rotator = GetComponent<SpriteRotator>();
    }

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<IngameGameManager>();
        string id = DataTableManager.StoreTable.GetTypeList(StoreTable.Type.SpeedyOven)[0].storeID;
        upgraded = SaveLoadManager.Data.upgrades[id];
        if (upgraded)
        {
            rotator.targetSprite = spritesData.sprites[1];
            rotator.rotate = spritesData.rotates[1];
        }
        else
        {
            rotator.targetSprite = spritesData.sprites[0];
            rotator.rotate = spritesData.rotates[0];
        }
        rotator.RotateSprite();
    }
}
