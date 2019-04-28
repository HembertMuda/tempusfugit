using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI interactionText = null;

    [SerializeField]
    private CanvasGroup dialogueBoxCanvasGroup = null;

    [SerializeField]
    private TextMeshProUGUI chatBoxText = null;

    [SerializeField]
    private List<Text> choicesText = null;

    [SerializeField]
    private Image lockedCursor = null;

    public TalkableCharacter CurrentTalkableCharacter;

    private string currentSentence;

    private Coroutine talkCoroutine;

    void Start()
    {
        Player player = FindObjectOfType<Player>();
        player.onInteractionLayerChanged += ChangeInteractionText;
        GameManager.Instance.onGameStateChanged += OnGamestateChanged;
    }

    void ChangeInteractionText(int interactionLayer)
    {
        switch (interactionLayer)
        {
            case 9:
                interactionText.text = "Open / Close";
                break;
            case 10:
                interactionText.text = "Talk";
                break;
            default:
                interactionText.text = string.Empty;
                break;
        }
    }

    void OnGamestateChanged(GameManager.GameState newGameState)
    {
        if (newGameState == GameManager.GameState.Talking)
        {
            ChangeInteractionText(0);
            dialogueBoxCanvasGroup.DOFade(1f, 0.5f).SetEase(Ease.OutCubic);
            lockedCursor.enabled = false;
        }
        else if (newGameState == GameManager.GameState.Walking)
        {
            dialogueBoxCanvasGroup.DOFade(0f, 0.5f).SetEase(Ease.OutCubic);
            lockedCursor.enabled = true;
        }
    }

    public void ChangeChatBoxText(string sentence)
    {
        chatBoxText.text = string.Empty;
        talkCoroutine = StartCoroutine(DrawDialogue(sentence));
    }

    private IEnumerator DrawDialogue(string sentence)
    {
        int charCount = 0;
        currentSentence = sentence;
        while (chatBoxText.text != sentence)
        {
            charCount++;
            chatBoxText.text = sentence.Substring(0, charCount);
            yield return new WaitForSeconds(0.05f);
        }
        talkCoroutine = null;
    }

    public void OnNextButtonClicked()
    {
        if (talkCoroutine != null)
        {
            StopCoroutine(talkCoroutine);
            talkCoroutine = null;
            chatBoxText.text = currentSentence;
        }
        else
        {
            CurrentTalkableCharacter.LetsTalk();
        }
    }

    public void AskSomething(List<string> choices)
    {
        chatBoxText.text = string.Empty;

        for (int i = 0; i < choicesText.Count; i++)
        {
            if (i < choices.Count)
            {
                choicesText[i].transform.parent.gameObject.SetActive(true);
                choicesText[i].text = choices[i];
            }
            else
            {
                choicesText[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }

    public void OnChoiceButtonClicked(int choiceindex)
    {
        for (int i = 0; i < choicesText.Count; i++)
        {
            choicesText[i].transform.parent.gameObject.SetActive(false);
        }

        CurrentTalkableCharacter.CheckRightChoice(choiceindex);
    }
}
