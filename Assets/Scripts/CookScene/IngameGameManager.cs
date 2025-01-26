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

    public CinemachineConfiner2D confinder;

    public PolygonCollider2D hallCollider;
    public PolygonCollider2D kitchenCollider;

    public PizzaCommand PizzaCommand { get; set; }

    private void Start()
    {
        PizzaCommand = PizzaCommand.None;
        confinder.m_BoundingShape2D = hallCollider;

        StartCoroutine(Spawn());
    }

    public void SetPizzaCommand(string command)
    {
        if (Enum.TryParse(command, true, out PizzaCommand result))
        {
            PizzaCommand = result;
        }
    }

    public void ChangePlace(bool hall)
    {
        if(hall)
        {
            confinder.m_BoundingShape2D = hallCollider;
            StartCoroutine(Spawn());
            return;
        }

        confinder.m_BoundingShape2D = kitchenCollider;
        var campos = confinder.gameObject.transform.position;
        campos.y = kitchenCollider.gameObject.transform.position.y;
        confinder.gameObject.transform.position = campos;
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
