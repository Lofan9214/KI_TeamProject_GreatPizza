using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public enum TutorialState
    {
        None,
        Dough,
        Source,
        Cheese,
        OvenEnter,
        OvenOut,
        Cutting,
        Boxing,
    }

    private TutorialState tutorialState = TutorialState.None;
    public List<StoryTable.Data> storyData;

    public List<GameObject> Operations;

    public void SetState(TutorialState state)
    {
        Operations[(int)tutorialState].SetActive(false);
        tutorialState = state;
        Operations[(int)tutorialState].SetActive(true);
    }
}
