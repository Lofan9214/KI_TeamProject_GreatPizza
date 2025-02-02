using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using SaveDataVC = SaveDataV1;

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

    public SaveDataVC tempSaveData { get; private set; }

    public string PizzaCommand { get; private set; }
    public IngredientTable.Type IngredientType { get; private set; }

    private void Awake()
    {
        pointerManager = GetComponent<PointerManager>();
        timeManager = GetComponent<IngameTimeManager>();
        timeManager.OnUnsatisfied.AddListener(Unsatisfied);

        tempSaveData = SaveLoadManager.Data.DeepCopy();
        ++tempSaveData.days;
    }

    private void Start()
    {
        PizzaCommand = string.Empty;

        kitchen.Init();

        hall.Set(confiner);

        uiManager.UpdateCurrentBudget();

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

    public void AddBudget(float add)
    {
        tempSaveData.budget += add;
        uiManager.UpdateCurrentBudget();
    }

    public void StopGame()
    {
        SceneManager.LoadScene(0);
    }
}
