using Cinemachine;
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
    public Transform virtualCam;

    public Hall hall;
    public Kitchen kitchen;

    public float screenScrollSpeed = 10f;

    public SaveDataVC tempSaveData { get; private set; }

    public string PizzaCommand { get; private set; }
    public IngredientTable.Type IngredientType { get; private set; }
    private IngredientVat currentTub;

    private void Awake()
    {
        pointerManager = GetComponent<PointerManager>();
        timeManager = GetComponent<IngameTimeManager>();

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

    public void SetPizzaCommand(IngredientVat tub, string command, IngredientTable.Type type)
    {
        if (tub != null)
        {
            tub.SetSelected(false);
        }
        currentTub = tub;
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
