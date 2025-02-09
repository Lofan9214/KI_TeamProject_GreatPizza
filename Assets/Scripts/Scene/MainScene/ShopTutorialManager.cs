using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TutorialManager;
using static UnityEngine.Rendering.HDROutputUtils;

public class ShopTutorialManager : MonoBehaviour
{
    public enum State
    {
        StoreSelect,
        IngredientSelect,
        End,
    }

    public State tutorialState { get; private set; }
    public GameObject[] operations;

    private MainGameManager gameManager;
    public ShopWindow shopWindow;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MainGameManager>();
    }

    public void SetState(State state)
    {
        if ((int)tutorialState < operations.Length)
        {
            operations[(int)tutorialState].SetActive(false);
        }
        tutorialState = state;
        if ((int)tutorialState < operations.Length)
        {
            operations[(int)tutorialState].SetActive(true);
        }
    }

    private void Update()
    {
        switch (tutorialState)
        {
            case State.StoreSelect:
                if (shopWindow.gameObject.activeSelf)
                    SetState(State.IngredientSelect);
                break;
            case State.IngredientSelect:
                if (SaveLoadManager.Data.ingredients["pepperoni"])
                    SetState(State.End);
                break;
        }
    }
}
