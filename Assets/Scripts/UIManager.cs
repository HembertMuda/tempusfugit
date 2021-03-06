﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    private float dialogueWriteWait = 0.03f;

    [SerializeField]
    private CanvasGroup memoriesCanvasGroup = null;

    [SerializeField]
    private CanvasGroup tutoCanvasGroup = null;

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
    private Image nextButtonMemory = null;

    [SerializeField]
    private Transform memoriesIconParent = null;

    [SerializeField]
    public Transform vignetteIconParent = null;

    [SerializeField]
    private GameObject vignettePrefab = null;

    [SerializeField]
    private AudioClip[] writeSound = null;

    [SerializeField]
    private CanvasGroup menuCanvasGroup = null;

    [SerializeField]
    private CanvasGroup inGameCanvasGroup = null;

    [SerializeField]
    private AudioClip fadeInSound = null;

    [SerializeField]
    private AudioClip fadeOutSound = null;

    [SerializeField]
    private Image titleEndImage = null;

    public TalkableCharacter CurrentTalkableCharacter;

    private string currentSentence;

    private Memory currentMemory;

    private int currentMemorySentenceindex;

    private Coroutine talkCoroutine;

    private AudioSource uiAudioSource;

    private Tween memoryVignetteTween;

    private Tween tellMemoryTween;

    private bool alreadyTalk;

    void Start()
    {
        Player player = FindObjectOfType<Player>();
        player.onInteractionLayerChanged += ChangeInteractionText;
        GameManager.Instance.onGameStateChanged += OnGamestateChanged;
        uiAudioSource = GetComponent<AudioSource>();
    }

    void ChangeInteractionText(int interactionLayer)
    {
        switch (interactionLayer)
        {
            case 9:
                interactionText.text = "<sprite index=1> Interact";
                break;
            case 10:
                interactionText.text = "<sprite index=0> Talk";
                break;
            default:
                interactionText.text = string.Empty;
                break;
        }
    }

    void OnGamestateChanged(GameManager.GameState newGameState)
    {
        menuCanvasGroup.interactable = newGameState == GameManager.GameState.Menu;
        menuCanvasGroup.blocksRaycasts = newGameState == GameManager.GameState.Menu;
        inGameCanvasGroup.interactable = newGameState != GameManager.GameState.Menu;
        inGameCanvasGroup.blocksRaycasts = newGameState != GameManager.GameState.Menu;

        if (newGameState == GameManager.GameState.Talking)
        {
            ChangeInteractionText(0);
            dialogueBoxCanvasGroup.DOFade(1f, 0.5f).SetEase(Ease.OutCubic);
            dialogueBoxCanvasGroup.interactable = true;
            lockedCursor.enabled = false;
            if (memoryVignetteTween != null)
                memoryVignetteTween.Kill();
            memoryVignetteTween = memoriesCanvasGroup.DOFade(0f, 1f).SetEase(Ease.OutCubic);
            alreadyTalk = true;
        }
        else if (newGameState == GameManager.GameState.Walking)
        {
            dialogueBoxCanvasGroup.DOFade(0f, 0.5f).SetEase(Ease.OutCubic);
            dialogueBoxCanvasGroup.interactable = false;
            lockedCursor.enabled = true;
            if (memoryVignetteTween != null)
                memoryVignetteTween.Kill();
            memoryVignetteTween = memoriesCanvasGroup.DOFade(1f, 1f).SetEase(Ease.OutCubic);
            if (alreadyTalk && !memoriesCanvasGroup.gameObject.activeInHierarchy)
            {
                EnableMemoriesGO();
            }
        }
    }

    public void ChangeChatBoxText(string sentence)
    {
        chatBoxText.text = string.Empty;
        if (talkCoroutine == null)
            talkCoroutine = StartCoroutine(DrawDialogue(sentence));
    }

    public void EnableMemoriesGO()
    {
        memoriesCanvasGroup.gameObject.SetActive(true);
        if (memoryVignetteTween != null)
            memoryVignetteTween.Kill();
        memoriesCanvasGroup.alpha = 0f;
        memoryVignetteTween = memoriesCanvasGroup.DOFade(1f, 0.2f).SetLoops(5, LoopType.Yoyo);
    }

    private IEnumerator DrawDialogue(string sentence)
    {
        int charCount = 0;
        currentSentence = sentence;
        while (chatBoxText.text != sentence)
        {
            charCount++;
            if (!uiAudioSource.isPlaying)
            {
                uiAudioSource.PlayOneShot(writeSound[Random.Range(0, writeSound.Length)]);
            }
            //Debug.Log($"sentence = {sentence} / charcount = {charCount}");
            chatBoxText.text = sentence.Substring(0, charCount);
            yield return new WaitForSeconds(dialogueWriteWait);
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
        if (tellMemoryTween == null || !tellMemoryTween.IsActive())
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
                    tellMemoryText.text = string.Empty;
                    nextButtonMemory.enabled = false;
                    titleEndImage.DOFade(1f, 3f).SetEase(Ease.OutCubic).OnComplete(() => SoundManager.Instance.FadeMusic(false));
                    titleEndImage.DOFade(0f, 3f).SetEase(Ease.OutCubic).SetDelay(6f).OnComplete(() => GameManager.Instance.Restart());
                    //GameManager.Instance.Restart();
                }
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
                if (CurrentTalkableCharacter.GetComponent<InspectorEnd>() == null)
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
        if (tellMemoryTween != null)
        {
            tellMemoryTween.Kill();
        }

        tellMemoryTween = tellMemoryCanvas.DOFade(toWhite ? 1f : 0f, whiteFadeDuration).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                if (!toWhite)
                {
                    CurrentTalkableCharacter.CurrentCharacterState = TalkableCharacter.CharacterState.Succeed;
                    CurrentTalkableCharacter.LetsTalk();
                }
                else
                {
                    LetsTalkMemory();
                }
            });

        dialogueBoxCanvasGroup.DOFade(toWhite ? 0f : 1f, 0.5f).SetEase(Ease.OutCubic);

        AudioClip clip = toWhite ? fadeInSound : fadeOutSound;

        if (clip != null)
        {
            uiAudioSource.Stop();
            uiAudioSource.PlayOneShot(clip);
        }

        tellMemoryCanvas.blocksRaycasts = toWhite;
    }

    public void TellMemory(Memory memory)
    {
        currentMemorySentenceindex = 0;
        tellMemoryText.text = string.Empty;
        string memoryText = string.Empty;
        currentMemory = memory;

        //LetsTalkMemory();

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
        memoriesIconParent.GetChild(vignetteIconParent.childCount - 1).GetComponent<Image>().DOFade(1f, 0.5f).SetLoops(5, LoopType.Yoyo);
    }

    public void OnPlayButtonClick()
    {
        menuCanvasGroup.DOFade(0f, 1f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            inGameCanvasGroup.DOFade(1f, 1f).SetEase(Ease.OutCubic);
            GameManager.Instance.ChangeState(GameManager.GameState.Walking);
            menuCanvasGroup.interactable = false;
            menuCanvasGroup.blocksRaycasts = false;
        });

        SoundManager.Instance.StopMusic();
    }

    public void OnCreditsButtonClick()
    {
        SceneManager.LoadScene(1);
    }

    public void OnQuitButtonClick()
    {
        Application.Quit();
    }

    public void HideTuto()
    {
        if (tutoCanvasGroup.alpha == 1f)
            tutoCanvasGroup.DOFade(0f, 1f).SetEase(Ease.OutCubic);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && GameManager.Instance.CurrentGameState == GameManager.GameState.Talking && CurrentTalkableCharacter != null)
        {
            if (CurrentTalkableCharacter.CurrentCharacterState == TalkableCharacter.CharacterState.Listening)
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
