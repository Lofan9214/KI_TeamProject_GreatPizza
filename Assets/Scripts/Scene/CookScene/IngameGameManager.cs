using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using SaveDataVC = SaveDataV1;

public class IngameGameManager : MonoBehaviour
{
    public enum State
    {
        Story,
        Random,
    }

    public PointerManager pointerManager { get; private set; }
    public IngameTimeManager timeManager { get; private set; }
    public NPC npc;
    public IngameUIManager uiManager;

    public CinemachineConfiner2D confiner;
    public Transform virtualCam;

    public Hall hall;
    public Kitchen kitchen;

    public float screenScrollSpeed = 10f;

    public SaveDataVC tempSaveData { get; private set; }

    public string PizzaCommand { get; private set; }
    public IngredientTable.Type IngredientType { get; private set; }
    private IngredientVat currentTub;

    private State state;
    private TutorialData tutorialManager;

    private void Awake()
    {
        pointerManager = GetComponent<PointerManager>();
        timeManager = GetComponent<IngameTimeManager>();

        tempSaveData = SaveLoadManager.Data.DeepCopy();
        ++tempSaveData.days;
        if (DataTableManager.StoryTable.IsExistData(tempSaveData.days))
        {
            state = State.Story;
            tutorialManager = new TutorialData();
            tutorialManager.storyData = DataTableManager.StoryTable.GetAtDay(tempSaveData.days);
        }
        else
        {
            state = State.Random;
        }
    }

    private void Start()
    {
        PizzaCommand = string.Empty;

        kitchen.Init();

        hall.Set(confiner);

        uiManager.UpdateCurrentBudget();

        StartCoroutine(Spawn());
    }

    public void SetPizzaCommand(IngredientVat tub, string command, IngredientTable.Type type)
    {
        if (currentTub != null
            && currentTub != tub)
        {
            currentTub.SetSelected(false);
        }
        currentTub = tub;
        PizzaCommand = command;
        IngredientType = type;

        foreach (var slot in kitchen.ingredientTable.pizzaSlots)
        {
            slot.CurrentPizza.ingredientGuide.SetActive(IngredientType == IngredientTable.Type.Ingredient);
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
                uiManager.SetOrderButtonActive(false);
                break;
            case InGamePlace.Kitchen:
                pointerManager.enableCamDrag = true;
                kitchen.Set(confiner);
                kitchen.SetPizzaBox();
                uiManager.SetOrderButtonActive(true);
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
        if (state == State.Random)
        {
            if (timeManager.CurrentState != IngameTimeManager.State.DayEnd)
            {
                var data = DataTableManager.RecipeTable.RandomGet();

                npc.gameObject.SetActive(true);
                npc.SetData(DataTableManager.NPCTable.GetRandom(1));
                npc.Order(data);

                timeManager.ResetSatisfaction();

                uiManager.ShowChatWindow(DataTableManager.TalkTable.GetRandomData(data.recipeID));
            }
        }
        else if (state == State.Story)
        {
            var found = tutorialManager.storyData.FindAll(p => ((p.timestart / 100 - 12) * 4 + (p.timestart % 100 / 15)) == timeManager.WatchTime);
            yield return new WaitUntil(() => found.Count > 0);
            npc.gameObject.SetActive(true);
            npc.SetData(found[0]);
            npc.Order(DataTableManager.RecipeTable.Get(found[0].recipeID));

            timeManager.ResetSatisfaction();
            if (found[0].timelock == 1
                && found[0].satisfactionlock == 1)
            {
                timeManager.SetState(IngameTimeManager.State.AllStop);
            }
            if (found[0].timelock == 1
                && found[0].satisfactionlock == 0)
            {
                timeManager.SetState(IngameTimeManager.State.WatchStop);
            }

            var chats = DataTableManager.TalkTable.GetByGroupId(found[0].groupID).Select(p => p.stringID).ToList();
            if(chats.Count == 3)
            {
                chats.AddRange(DataTableManager.TalkTable.GetResultTalk());
            }
            uiManager.ShowChatWindow(chats.ToArray());
        }
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

    public void ScrollScreen()
    {
        Vector3 viewPortPos = Camera.main.ScreenToViewportPoint(MultiTouchManager.Instance.TouchPosition);
        if (Mathf.Abs(viewPortPos.x - 0.5f) > 0.25f)
        {
            float mul = (viewPortPos.x - 0.5f) * 2f;
            Vector3 cameraPos = Camera.main.transform.position;
            cameraPos.x += screenScrollSpeed * Time.deltaTime * mul;
            virtualCam.position = cameraPos;
        }
    }
}
