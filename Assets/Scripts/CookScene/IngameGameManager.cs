using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

public class IngameGameManager : MonoBehaviour
{
    public PointerManager pointerManager { get; private set; }
    public IngameTimeManager timeManager { get; private set; }
    public NPC npc;
    public IngameUIManager uiManager;

    public CinemachineConfiner2D confiner;

    public PackingTable packingTable;

    public Hall hall;
    public Kitchen kitchen;

    public string PizzaCommand { get; private set; }
    public IngredientTable.Type IngredientType { get; private set; }

    private void Awake()
    {
        pointerManager = GetComponent<PointerManager>();
        timeManager = GetComponent<IngameTimeManager>();
        timeManager.OnUnsatisfied.AddListener(Unsatisfied);
    }

    private void Start()
    {
        PizzaCommand = string.Empty;

        var ingredientData = DataTableManager.IngredientTable.GetList();

        bool added = false;

        foreach (var ing in ingredientData)
        {
            if (!SaveLoadManager.Data.unlocks.ContainsKey(ing.ingredientID))
            {
                SaveLoadManager.Data.unlocks.Add(ing.ingredientID, true);
                added = true;
            }
        }

        if (added)
            SaveLoadManager.Save();

        kitchen.Init();

        hall.Set(confiner);

        uiManager.UpdateCurrentCurrency();

        StartCoroutine(Spawn());
    }


    public void SetPizzaCommand(string command, IngredientTable.Type type)
    {
        PizzaCommand = command;
        IngredientType = type;
    }

    public void ChangePlace(InGamePlace place)
    {
        switch (place)
        {
            case InGamePlace.Hall:
                pointerManager.enableCamDrag = false;
                hall.Set(confiner);
                if (!npc.gameObject.activeSelf)
                {
                    StartSpawn();
                }
                break;
            case InGamePlace.Kitchen:
                pointerManager.enableCamDrag = true;
                kitchen.Set(confiner);
                packingTable.SetPizzaBox(1);
                break;
        }
    }

    public void StartSpawn()
    {
        StartCoroutine(Spawn());
    }

    public IEnumerator Spawn()
    {
        yield return new WaitForSeconds(2f);
        if (timeManager.CurrentState != IngameTimeManager.State.DayEnd)
        {
            var data = DataTableManager.RecipeTable.RandomGet();

            npc.gameObject.SetActive(true);
            npc.SetSprite(DataTableManager.NPCTable.GetRandom(1));
            npc.Order(data);

            timeManager.ResetSatisfaction();

            uiManager.ShowChatWindow(DataTableManager.TalkTable.GetRandomData(data.recipeID));
        }
    }

    public void Unsatisfied()
    {
        ChangePlace(InGamePlace.Hall);
    }

    public void AddCurrency(float add)
    {
        SaveLoadManager.Data.currency += add;
        uiManager.UpdateCurrentCurrency();
    }
}
