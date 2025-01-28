using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
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
    }

    public int[] stringIds;
    public TextMeshProUGUI text;
    public Button yesButton;
    public Button hintButton;
    public float chatSpeed = 0.05f;

    public LocalizationText hintText;

    public State TalkingState { get;private set; }
    private Talks talkIndex;

    private int charLength;
    private string script;
    private WaitForSeconds wait;

    private IngameGameManager gm;

    private void Awake()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<IngameGameManager>();
        wait = new WaitForSeconds(chatSpeed);
    }

    private void Start()
    {
        yesButton.onClick.AddListener(Yes);
        hintButton.onClick.AddListener(NeedHint);
    }

    public void SetStrings(int[] Ids)
    {
        stringIds = Ids.ToArray();
        yesButton.gameObject.SetActive(true);
        hintButton.gameObject.SetActive(true);
        NextTalk(0);
    }

    public IEnumerator Talk()
    {
        TalkingState = State.Talking;
        while (TalkingState == State.Talking && charLength < script.Length)
        {
            ++charLength;
            text.text = script.Substring(0, charLength);
            if (charLength == script.Length)
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
        talkIndex = index;
        switch (talkIndex)
        {
            case Talks.Order:
            case Talks.Hint1:
                yesButton.gameObject.SetActive(true);
                hintButton.gameObject.SetActive(true);
                break;
            case Talks.Hint2:
                yesButton.gameObject.SetActive(true);
                break;
            case Talks.Success:
            case Talks.Normal:
            case Talks.Fail:
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

    public void Yes()
    {
        gm.ChangePlace(false);
        gameObject.SetActive(false);
    }

    public void NeedHint()
    {
        switch (talkIndex)
        {
            case Talks.Order:
                NextTalk((Talks)((int)talkIndex + 1));
                hintText.stringId = 110903.ToString();
                break;
            case Talks.Hint1:
                NextTalk((Talks)((int)talkIndex + 1));
                hintButton.gameObject.SetActive(false);
                break;
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
