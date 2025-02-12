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
    }

    public enum TimeState
    {
        None,
        WatchStop,
        AllStop,
    }

    public FormattedLocalizationText dayText;
    public FormattedLocalizationText watchText;
    public TextMeshProUGUI satisfactionText;
    public FadeOut fadeOutEffect;

    public EndWindow endWindow;

    public float watchTimeInterval = 10f;
    public float satisfactionInterval = 3f;

    public State CurrentState { get; private set; }
    public TimeState CurrentTimeState { get; private set; } = TimeState.None;

    public int WatchTime { get; private set; }
    private float watchTimeTimer;
    private int WatchTimeEnd;

    public int Satisfaction { get; private set; }
    private float satisfactionTimer;

    public UnityEvent OnUnsatisfied;

    private IngameGameManager gameManager;
    private IngameSoundManager soundManager;
    
    private bool endBellPlayed = false;

    private void Start()
    {
        WatchTime = 0;
        watchTimeTimer = 0f;
        satisfactionTimer = 0f;
        WatchTimeEnd = 36;

        gameManager = GetComponent<IngameGameManager>();
        soundManager = GetComponent<IngameSoundManager>();
        dayText.SetString(gameManager.tempSaveData.days.ToString());

        if (SaveLoadManager.Data.upgrades.TryGetValue("longerDay", out bool value) && value)
        {
            WatchTimeEnd += 4;
        }
        ResetSatisfaction();
        SetWatchTimeText();
    }

    private void Update()
    {
        if (WatchTime == WatchTimeEnd)
        {
            if (CurrentState == State.OrderEnd)
            {
                SetState(State.DayEnd);
                WatchTime = WatchTimeEnd + 4;
                StartCoroutine(DayEnd());
            }
            if(!endBellPlayed)
            {
                endBellPlayed=true;
                soundManager.PlayEndBell();
            }
        }

        if ((CurrentState == State.Ordering || CurrentState == State.OrderEnd)
            && (CurrentTimeState != TimeState.WatchStop && CurrentTimeState != TimeState.AllStop)
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
            && CurrentTimeState != TimeState.AllStop)
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
        if (CurrentTimeState == TimeState.AllStop)
        {
            return;
        }

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

    public void SetTimeState(TimeState state)
    {
        CurrentTimeState = state;
    }

    private void SetSatisfactionText()
    {
        gameManager.uiManager.satisafaction.SetSatisfaction(Satisfaction);
        satisfactionText.text = $"{Satisfaction}%";
    }

    private void SetWatchTimeText()
    {
        watchText.SetString($"{12 + WatchTime / 4:D2}:{WatchTime % 4 * 15:D2}");
    }

    private IEnumerator DayEnd()
    {
        if (!gameManager.npc.OrderEnd)
        {
            gameManager.npc.Disappear();
        }

        yield return new WaitUntil(() => gameManager.npc.Disappeared);
        soundManager.PlayDayEnd();
        fadeOutEffect.PlayFadeOut();
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

    public void SetPause(bool pause)
    {
        if (pause)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }
}
