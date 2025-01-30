using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IngameTimeManager : MonoBehaviour
{
    public enum State
    {
        BeforeCook,
        Ordering,
        OrderEnd,
        DayEnd,
        Pause
    }

    public TextMeshProUGUI watchText;
    public TextMeshProUGUI satisfactionText;

    public GameObject endWindow;

    public State CurrentState { get; private set; }

    public int WatchTime { get; private set; }
    private float watchTimeTimer;
    public int Satisfaction { get; private set; }
    private float satisfactionTimer;



    private void Start()
    {
        WatchTime = 0;
        watchTimeTimer = 0f;
        satisfactionTimer = 0f;
        ResetSatisfaction();
        SetWatchTimeText();
    }

    private void Update()
    {
        if (CurrentState == State.OrderEnd
            && WatchTime == 36)
        {
            SetState(State.DayEnd);
            WatchTime = 40;
            SetWatchTimeText();

            endWindow.SetActive(true);
        }

        if (CurrentState != State.Pause)
        {
            watchTimeTimer += Time.deltaTime;
            if (watchTimeTimer > 10f)
            {
                watchTimeTimer -= 10f;
                WatchTime = Mathf.Min(36, ++WatchTime);

                SetWatchTimeText();
            }

            if (CurrentState == State.Ordering)
            {
                satisfactionTimer += Time.deltaTime;
                if (satisfactionTimer > 3f)
                {
                    satisfactionTimer -= 3f;
                    Satisfaction = Mathf.Max(0, --Satisfaction);
                    if (Satisfaction == 0)
                    {

                    }
                    SetSatisfactionText();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            endWindow.SetActive(!endWindow.activeSelf);
        }
    }

    public void ResetSatisfaction()
    {
        Satisfaction = 100;
        CurrentState = State.BeforeCook;
        SetSatisfactionText();
    }

    public void UsedHint(ChatWindow.Talks talks)
    {
        switch (talks)
        {
            case ChatWindow.Talks.Hint1:
                Satisfaction -= 7;
                SetSatisfactionText();
                break;
            case ChatWindow.Talks.Hint2:
                Satisfaction -= 13;
                SetSatisfactionText();
                break;
        }
    }

    public void SetState(State state)
    {
        this.CurrentState = state;
    }

    private void SetSatisfactionText()
    {
        satisfactionText.text = $"{Satisfaction}%";
    }

    private void SetWatchTimeText()
    {
        watchText.text = $"{(12 + WatchTime / 4):D2}:{(WatchTime % 4) * 15:D2}";
    }
}
