using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

public class IngameGameManager : MonoBehaviour
{
    public PointerManager pointerManager;
    public NPC npc;
    public IngameUIManager uiManager;

    public CinemachineConfiner2D confiner;

    public PackingTable packingTable;

    public Hall hall;
    public Kitchen kitchen;

    public PizzaCommand PizzaCommand { get; set; }

    private void Start()
    {
        PizzaCommand = PizzaCommand.None;

        var ingredientData = DataTableManager.IngredientTable.GetList();
        foreach (var ing in ingredientData)
        {
            if (!SaveLoadManager.Data.unlocks.ContainsKey(ing.ingredientID))
            {
                SaveLoadManager.Data.unlocks.Add(ing.ingredientID, true);
            }
        }

        hall.Set(confiner);

        StartCoroutine(Spawn());
    }

    public void SetPizzaCommand(string command)
    {
        if (Enum.TryParse(command, true, out PizzaCommand result))
        {
            PizzaCommand = result;
        }
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

        var data = DataTableManager.RecipeTable.RandomGet();

        npc.gameObject.SetActive(true);
        npc.SetSprite(DataTableManager.NPCTable.GetRandom(1));
        npc.Order(data);

        uiManager.ShowChatWindow(DataTableManager.TalkTable.GetRandomData(data.recipeID));
    }
}
