using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class IngameTimeManager : MonoBehaviour
{
    public enum State
    {
        Ordering,
        OrderEnd,
        DayEnd,
        Pause,
        WatchStop,
        AllStop,
    }

    public TextMeshProUGUI dayText;
    public TextMeshProUGUI watchText;
    public TextMeshProUGUI satisfactionText;

    public EndWindow endWindow;

    public float watchTimeInterval = 10f;
    public float satisfactionInterval = 3f;

    public State CurrentState { get; private set; }

    public int WatchTime { get; private set; }
    private float watchTimeTimer;
    private int WatchTimeEnd;

    public int Satisfaction { get; private set; }
    private float satisfactionTimer;

    public UnityEvent OnUnsatisfied;

    private IngameGameManager gameManager;

    private void Start()
    {
        WatchTime = 0;
        watchTimeTimer = 0f;
        satisfactionTimer = 0f;
        WatchTimeEnd = 36;

        gameManager = GetComponent<IngameGameManager>();
        dayText.text = gameManager.tempSaveData.days.ToString();

        //todo 업그레이드 수정 필요
        //if (SaveLoadManager.Data.upgrades.TryGetValue(001, out bool value) && value)
        //{
        //    WatchTimeEnd += 4;
        //}
        ResetSatisfaction();
        SetWatchTimeText();
    }

    private void Update()
    {
        if (CurrentState == State.OrderEnd
            && WatchTime == WatchTimeEnd)
        {
            SetState(State.DayEnd);
            WatchTime = WatchTimeEnd + 4;
            SetWatchTimeText();

            endWindow.Show();
        }

        if ((CurrentState == State.Ordering
            || CurrentState == State.OrderEnd) 
            && WatchTime <= WatchTimeEnd)
        {
            watchTimeTimer += Time.deltaTime;
            if (watchTimeTimer > watchTimeInterval)
            {
                watchTimeTimer -= watchTimeInterval;
                WatchTime = Mathf.Min(WatchTimeEnd, ++WatchTime);

                SetWatchTimeText();
            }
        }
        if (CurrentState == State.Ordering
            || CurrentState == State.WatchStop)
        {
            satisfactionTimer += Time.deltaTime;
            if (satisfactionTimer > satisfactionInterval)
            {
                satisfactionTimer -= satisfactionInterval;
                Satisfaction = Mathf.Max(0, --Satisfaction);
                if (Satisfaction == 0)
                {
                    CurrentState = State.OrderEnd;
                    OnUnsatisfied?.Invoke();
                }
                SetSatisfactionText();
            }
        }
    }

    public void ResetSatisfaction()
    {
        Satisfaction = 100;
        CurrentState = State.OrderEnd;
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
        CurrentState = state;
    }

    private void SetSatisfactionText()
    {
        satisfactionText.text = $"{Satisfaction}%";
    }

    private void SetWatchTimeText()
    {
        watchText.text = $"{12 + WatchTime / 4:D2}:{WatchTime % 4 * 15:D2}";
    }

    public void SetWatch(int time)
    {
        if (time <= WatchTimeEnd)
        {
            WatchTime = time;
            watchTimeTimer = 0f;
            satisfactionTimer = 0f;
            SetWatchTimeText();
        }
    }
}
