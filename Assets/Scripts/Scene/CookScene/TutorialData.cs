using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialData
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
}
