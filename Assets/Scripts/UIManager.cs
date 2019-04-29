using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private float whiteFadeDuration = 2f;

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

    [SerializeField]
    private CanvasGroup tellMemoryCanvas = null;

    [SerializeField]
    private TextMeshProUGUI tellMemoryText = null;

    [SerializeField]
    private Transform memoriesIconParent = null;

    [SerializeField]
    public Transform vignetteIconParent = null;

    [SerializeField]
    private GameObject vignettePrefab = null;

    public TalkableCharacter CurrentTalkableCharacter;

    private string currentSentence;

    private Memory currentMemory;

    private int currentMemorySentenceindex;

    private Coroutine talkCoroutine;
    
    //private Coroutine memoryCoroutine; 

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

    public void OnMemoryNextButtonClicked()
    {
        if (currentMemorySentenceindex < currentMemory.Sentences.Count)
        {
            //StopCoroutine(memoryCoroutine);
            //memoryCoroutine = null;
            LetsTalkMemory();
            //tellMemoryText.text = currentMemorySentence;
        }
        else 
        {
            if (CurrentTalkableCharacter.GetComponent<InspectorEnd>() == null)
            {
                FadeWhite(false);
            CurrentTalkableCharacter.LetsTalk();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
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
                if(CurrentTalkableCharacter.GetComponent<InspectorEnd>() == null)
                {
                    choicesText[i].text = GameManager.Instance.GetMemoryByName(choices[i]).ChoiceText;
                }
                else
                {
                    choicesText[i].text = choices[i];
                }
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

    public void FadeWhite(bool toWhite)
    {
        tellMemoryCanvas.DOFade(toWhite ? 1f : 0f, whiteFadeDuration).SetEase(Ease.OutCubic).OnComplete(() => 
            {
                if(!toWhite)
                {
                    CurrentTalkableCharacter.CurrentCharacterState = TalkableCharacter.CharacterState.Succeed;
                    CurrentTalkableCharacter.LetsTalk();
                }
            });
        tellMemoryCanvas.interactable = toWhite;
    }

    public void TellMemory(Memory memory)
    {
        currentMemorySentenceindex = 0;
        tellMemoryText.text = string.Empty;
        string memoryText = string.Empty;
        currentMemory = memory;

        LetsTalkMemory();

        //memoryCoroutine = StartCoroutine(DrawMemory(memoryText));
    }

    public void LetsTalkMemory()
    {
        if (currentMemorySentenceindex > 0)
            tellMemoryText.text = tellMemoryText.text + "\n\n";
        tellMemoryText.text = tellMemoryText.text + currentMemory.Sentences[currentMemorySentenceindex];
        currentMemorySentenceindex++;

        //if(currentMemorySentenceindex == currentMemory.Sentences.Count)


        //tellMemoryText.text = memoryText;
    }

    //private IEnumerator DrawMemory(string sentence)
    //{
    //    int charCount = 0;
    //    currentMemorySentence = sentence;
    //    while (tellMemoryText.text != sentence)
    //    {
    //        charCount++;
    //        tellMemoryText.text = sentence.Substring(0, charCount);
    //        yield return new WaitForSeconds(0.05f);
    //    }
    //    memoryCoroutine = null;
    //}

    public void GiveMemory(Sprite memorySprite)
    {
        RectTransform vignetteTransform = Instantiate(vignettePrefab, vignetteIconParent).GetComponent<RectTransform>();
        vignetteTransform.GetComponent<Image>().sprite = memorySprite;
        vignetteTransform.DOMove(memoriesIconParent.GetChild(vignetteIconParent.childCount - 1).GetComponent<RectTransform>().position, 1f).SetEase(Ease.OutCubic);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && GameManager.Instance.CurrentGameState == GameManager.GameState.Talking && CurrentTalkableCharacter != null)
        {
            if(CurrentTalkableCharacter.CurrentCharacterState == TalkableCharacter.CharacterState.Listening)
            {
                OnMemoryNextButtonClicked();
            }
            else
            {
                OnNextButtonClicked();
            }
        }
    }
}
