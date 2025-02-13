using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public enum TutorialState
    {
        Dough,
        SourceCheese1,
        SourceCheese2,
        SourceSelect,
        Source,
        CheeseSelect,
        Cheese,
        OvenEnter,
        OvenCooking,
        OvenExit,
        Cutting1,
        Cutting2,
        Cutting3,
        Packing,
        Pickup,
        PickupEnd,
        PepperoniWait,
        Pepperoni,
        None,
    }

    private enum TutorialMessages
    {
        Dough = 111015,
        SourceCheese1 = 111016,
        SourceCheese2 = 111017,
        Source = 111018,
        Cheese = 111019,
        Cutting = 111020,
        Pepperoni = 111021,
        CuttingCancel = 111040,
    }

    private TutorialState tutorialState = TutorialState.None;
    public List<StoryTable.Data> storyData;

    public GameObject ScreenProtector;
    public List<GameObject> Operations;

    private Pizza pizza;

    private IngameGameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<IngameGameManager>();
        foreach (var trashbin in gameManager.kitchen.trashBins)
            trashbin.enabled = false;
    }

    private void Update()
    {
        switch (tutorialState)
        {
            case TutorialState.Dough:
                gameManager.pointerManager.enableCamDrag = false;
                if (gameManager.kitchen.ingredientTable.pizzaSlots[0].CurrentPizza != null)
                {
                    pizza = gameManager.kitchen.ingredientTable.pizzaSlots[0].CurrentPizza;
                    SetState(TutorialState.SourceCheese1);
                }
                break;
            case TutorialState.SourceCheese1:
                if (Input.touchCount == 1
                    && Input.GetTouch(0).phase == TouchPhase.Began)
                    SetState(TutorialState.SourceCheese2);
                break;
            case TutorialState.SourceCheese2:
                if (Input.touchCount == 1
                    && Input.GetTouch(0).phase == TouchPhase.Began)
                    SetState(TutorialState.SourceSelect);
                break;
            case TutorialState.SourceSelect:
                if (gameManager.PizzaCommand == "tomato")
                    SetState(TutorialState.Source);
                break;
            case TutorialState.Source:
                if (pizza.sourceLayer.Ratio > 0.9f)
                    SetState(TutorialState.CheeseSelect);
                break;
            case TutorialState.CheeseSelect:
                if (gameManager.PizzaCommand == "cheese")
                    SetState(TutorialState.Cheese);
                break;
            case TutorialState.Cheese:
                if (pizza.cheeseLayer.Ratio > 0.9f)
                    SetState(TutorialState.OvenEnter);
                break;
            case TutorialState.OvenEnter:
                if (gameManager.kitchen.ovenEnter.CurrentPizza != null)
                {
                    SetState(TutorialState.OvenCooking);
                    StartCoroutine(CamMove(Operations[(int)tutorialState].transform.position, 0.3f));
                }
                break;
            case TutorialState.OvenCooking:
                if (gameManager.kitchen.ovenExit.CurrentPizza != null)
                {
                    SetState(TutorialState.OvenExit);
                    StartCoroutine(CamMove(Operations[(int)tutorialState].transform.position, 0.3f));
                }
                break;
            case TutorialState.OvenExit:
                if (gameManager.kitchen.cuttingTableSlot.CurrentPizza != null)
                {
                    SetState(TutorialState.Cutting1);
                    StartCoroutine(CamMove(Operations[(int)tutorialState].transform.position, 0.3f));
                }
                break;
            case TutorialState.Cutting1:
                if (pizza.PizzaData.cutData.Count == 1)
                {
                    SetState(TutorialState.Cutting2);
                }
                break;
            case TutorialState.Cutting2:
                if (gameManager.kitchen.cutter.CuttingCanceled)
                {
                    gameManager.kitchen.cutter.CuttingCanceled = false;
                    SetState(TutorialState.Cutting3);
                }
                break;
            case TutorialState.Cutting3:
                if (pizza.PizzaData.cutData.Count == 3)
                {
                    SetState(TutorialState.Packing);
                    StartCoroutine(CamMove(Operations[(int)tutorialState].transform.position, 0.3f));
                }
                break;
            case TutorialState.Packing:
                if (gameManager.gamePlace == InGamePlace.Hall)
                {
                    SetState(TutorialState.Pickup);
                }
                break;
            case TutorialState.PepperoniWait:
                if (gameManager.PizzaCommand == "pepperoni")
                {
                    SetState(TutorialState.Pepperoni);
                    ScreenProtector.SetActive(true);
                }
                break;
            case TutorialState.Pepperoni:
                if (Input.touchCount == 1
                    && Input.GetTouch(0).phase == TouchPhase.Began)
                    SetState(TutorialState.None);
                break;
            case TutorialState.Pickup:
                if (gameManager.timeManager.CurrentState == IngameTimeManager.State.OrderEnd)
                    SetState(TutorialState.PickupEnd);
                break;

        }
    }


    public void SetState(TutorialState state)
    {
        if (tutorialState != TutorialState.None
            && (int)tutorialState < Operations.Count)
        {
            Operations[(int)tutorialState].SetActive(false);
            if (tutorialState == TutorialState.SourceCheese2)
                gameManager.uiManager.tutorialArrow.SetActive(false);
        }
        tutorialState = state;
        OnStateChanged();
    }

    private void OnStateChanged()
    {
        if (gameManager == null)
        {
            gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<IngameGameManager>();
        }

        if (tutorialState != TutorialState.None)
        {
            ScreenProtector.SetActive(true);
            if ((int)tutorialState < Operations.Count)
            {
                Operations[(int)tutorialState].SetActive(true);
            }
        }
        switch (tutorialState)
        {
            case TutorialState.Dough:
                gameManager.uiManager.tutorialWindow.SetActive(true);
                gameManager.uiManager.tutorialText.SetString(((int)TutorialMessages.Dough).ToString());
                break;
            case TutorialState.SourceCheese1:
                pizza.Movable = false;
                gameManager.uiManager.tutorialWindow.SetActive(true);
                gameManager.uiManager.tutorialText.SetString(((int)TutorialMessages.SourceCheese1).ToString());
                break;
            case TutorialState.SourceCheese2:
                gameManager.uiManager.tutorialWindow.SetActive(true);
                gameManager.uiManager.tutorialArrow.SetActive(true);
                gameManager.uiManager.tutorialText.SetString(((int)TutorialMessages.SourceCheese2).ToString());
                break;
            case TutorialState.Source:
                gameManager.uiManager.tutorialWindow.SetActive(true);
                gameManager.uiManager.tutorialText.SetString(((int)TutorialMessages.Source).ToString());
                break;
            case TutorialState.Cheese:
                gameManager.uiManager.tutorialWindow.SetActive(true);
                gameManager.uiManager.tutorialText.SetString(((int)TutorialMessages.Cheese).ToString());
                break;
            case TutorialState.OvenEnter:
            case TutorialState.Packing:
                pizza.Movable = true;
                gameManager.uiManager.tutorialWindow.SetActive(false);
                break;
            case TutorialState.Cutting1:
                pizza.Movable = false;
                break;
            case TutorialState.Cutting2:
                gameManager.kitchen.cutter.CuttingLock = true;
                gameManager.uiManager.tutorialWindow.SetActive(true);
                gameManager.uiManager.tutorialText.SetString(((int)TutorialMessages.CuttingCancel).ToString());
                break;
            case TutorialState.Cutting3:
                gameManager.kitchen.cutter.CuttingLock = false;
                gameManager.uiManager.tutorialWindow.SetActive(true);
                gameManager.uiManager.tutorialText.SetString(((int)TutorialMessages.Cutting).ToString());
                break;
            case TutorialState.Pepperoni:
                gameManager.uiManager.tutorialWindow.SetActive(true);
                gameManager.uiManager.tutorialText.SetString(((int)TutorialMessages.Pepperoni).ToString());
                break;
            case TutorialState.PickupEnd:
                ScreenProtector.SetActive(false);
                gameManager.uiManager.tutorialWindow.SetActive(false);
                foreach (var trashbin in gameManager.kitchen.trashBins)
                    trashbin.enabled = true;
                break;
            case TutorialState.PepperoniWait:
                foreach (var trashbin in gameManager.kitchen.trashBins)
                    trashbin.enabled = true;
                break;
            case TutorialState.None:
                ScreenProtector.SetActive(false);
                gameManager.uiManager.tutorialWindow.SetActive(false);
                gameManager.pointerManager.enableCamDrag = true;
                foreach (var trashbin in gameManager.kitchen.trashBins)
                    trashbin.enabled = true;
                break;
            case TutorialState.SourceSelect:
            case TutorialState.CheeseSelect:
            case TutorialState.OvenCooking:
            case TutorialState.OvenExit:
            case TutorialState.Pickup:
                gameManager.uiManager.tutorialWindow.SetActive(false);
                break;
        }
    }

    private IEnumerator CamMove(Vector3 targetPos, float duration)
    {
        float timer = 0f;
        Vector3 cameraPos = Camera.main.transform.position;
        float startx = cameraPos.x;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float rate = Mathf.Min(timer / duration, 1f);
            cameraPos.x = Mathf.Lerp(startx, targetPos.x, rate);
            gameManager.virtualCam.position = cameraPos;
            yield return null;
        }
    }
}
