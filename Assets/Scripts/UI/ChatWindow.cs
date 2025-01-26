using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChatWindow : MonoBehaviour
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
        Good,
        Normal,
        Fail,
    }

    public int[] stringIds;
    public TextMeshProUGUI text;
    public Button yesButton;
    public Button hintButton;
    public float chatSpeed = 0.05f;

    public LocalizationText hintText;

    private State state;
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
        NextTalk(0);
    }

    public IEnumerator Talk()
    {
        while (charLength < script.Length)
        {
            ++charLength;
            text.text = script.Substring(0, charLength);
            yield return wait;
        }
        state = State.Talkend;
    }

    public void NextTalk(Talks index)
    {
        talkIndex = index;
        charLength = 0;
        state = State.Talking;
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
}
