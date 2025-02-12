using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public Sprite normal;
    public Sprite gold;

    private ParticleSystem particleSystem;

    private IngameGameManager gameManager;
    private bool upgraded;

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        gameManager = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<IngameGameManager>();
        string id = DataTableManager.StoreTable.GetTypeList(StoreTable.Type.SpeedyOven)[0].storeID;
        upgraded = SaveLoadManager.Data.upgrades[id];
        if (upgraded)
        {
            particleSystem.textureSheetAnimation.SetSprite(0, gold);
        }
        else
        {
            particleSystem.textureSheetAnimation.SetSprite(0, normal);
        }
    }
}
