using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class IngameGameManager : MonoBehaviour
{
    public PointerManager pointerManager;
    public NPCBehaviour npc;
    public IngameUIManager uiManager;

    public CinemachineConfiner2D confiner;

    public PackingTable packingTable;

    public Hall hall;
    public Kitchen kitchen;

    public PizzaCommand PizzaCommand { get; set; }

    private void Start()
    {
        PizzaCommand = PizzaCommand.None;

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

    public void ChangePlace(bool isHall)
    {
        pointerManager.enableCamDrag = !isHall;
        if (isHall)
        {
            hall.Set(confiner);
            if (!npc.gameObject.activeSelf)
            {
                StartCoroutine(Spawn());
            }
            return;
        }

        kitchen.Set(confiner);
        packingTable.SetPizzaBox(1);
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
