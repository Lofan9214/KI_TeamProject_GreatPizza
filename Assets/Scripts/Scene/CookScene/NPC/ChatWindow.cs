using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChatWindow : MonoBehaviour, IPointerDownHandler
{
    public enum State
    {
        None,
        Talking,
        Talkend,
    }

    public enum Talks
    {
        Order,
        Hint1,
        Hint2,
        Success,
        Normal,
        Fail,
        Additional,
    }

    public enum ButtonText
    {
        Okay = 110901,
        Pardon = 110902,
        Hint = 110903,
    }

    public int[] stringIds;
    public TextMeshProUGUI text;
    public Button yesButton;
    public Button hintButton;
    public float chatSpeed = 45f;

    public LocalizationText yesText;
    public LocalizationText hintText;

    public State TalkingState { get; private set; }
    public Talks talkIndex { get; private set; }

    private float charLength;
    private string script;
    private WaitForEndOfFrame wait;

    private IngameGameManager gm;

    public UnityEvent OnYes;

    private void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<IngameGameManager>();
        wait = new WaitForEndOfFrame();
    }

    private void Start()
    {
        yesButton.onClick.AddListener(() => StartCoroutine(YesCoroutine()));
        hintButton.onClick.AddListener(NeedHint);
    }

    public void SetStrings(int[] Ids)
    {
        stringIds = Ids.ToArray();
        yesButton.gameObject.SetActive(true);
        hintButton.gameObject.SetActive(true);
        yesText.SetString(((int)ButtonText.Okay).ToString());
        hintText.SetString(((int)ButtonText.Pardon).ToString());
        NextTalk(0);
    }

    public IEnumerator Talk()
    {
        TalkingState = State.Talking;
        while (TalkingState == State.Talking && (int)charLength < script.Length)
        {
            charLength += Time.deltaTime * chatSpeed;
            text.text = script.Substring(0, (int)charLength);
            if ((int)charLength == script.Length)
            {
                TalkingState = State.Talkend;
                break;
            }
            yield return wait;
        }
        if (TalkingState == State.Talkend && charLength < script.Length)
        {
            text.text = script;
        }
    }

    public void NextTalk(Talks index)
    {
        if ((int)index >= stringIds.Length)
        {
            return;
        }

        talkIndex = index;
        switch (talkIndex)
        {
            case Talks.Order:
                yesButton.gameObject.SetActive(true);
                hintButton.gameObject.SetActive(true);
                break;
            case Talks.Hint1:
                yesButton.gameObject.SetActive(true);
                hintButton.gameObject.SetActive(true);
                gm.timeManager.UsedHint(Talks.Hint1);
                break;
            case Talks.Hint2:
                yesButton.gameObject.SetActive(true);
                gm.timeManager.UsedHint(Talks.Hint2);
                break;
            case Talks.Success:
            case Talks.Normal:
            case Talks.Fail:
            case Talks.Additional:
                yesButton.gameObject.SetActive(false);
                hintButton.gameObject.SetActive(false);
                break;
        }

        gameObject.SetActive(true);
        charLength = 0;
        TalkingState = State.Talking;
        script = DataTableManager.StringTable.Get(stringIds[(int)index]);

        StartCoroutine(Talk());
    }

    private IEnumerator YesCoroutine()
    {
        OnYes?.Invoke();
        yield return new WaitForSeconds(0.75f);
        gm.ChangePlace(InGamePlace.Kitchen);
        gm.timeManager.SetState(IngameTimeManager.State.Ordering);
        gameObject.SetActive(false);
    }

    public void NeedHint()
    {
        switch (talkIndex)
        {
            case Talks.Order:
                NextTalk((Talks)((int)talkIndex + 1));
                yesText.SetString(((int)ButtonText.Okay).ToString());
                hintText.SetString(((int)ButtonText.Hint).ToString());
                break;
            case Talks.Hint1:
                NextTalk((Talks)((int)talkIndex + 1));
                hintButton.gameObject.SetActive(false);
                break;
        }
    }

    public void SetActiveButton(Talks talk, bool active)
    {
        if (talk == Talks.Order)
        {
            yesButton.gameObject.SetActive(active);
        }
        else if (talk == Talks.Hint1)
        {
            hintButton.gameObject.SetActive(active);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (TalkingState == State.Talking)
        {
            TalkingState = State.Talkend;
        }
    }
}
