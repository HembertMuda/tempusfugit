using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class TalkableCharacter : MonoBehaviour
{
    [SerializeField]
    public Transform camFramingPoint;

    [SerializeField, TextArea]
    private List<string> introSentences = new List<string>();

    [SerializeField, TextArea]
    private List<string> otherIntroSentences = new List<string>();

    [SerializeField, ValueDropdown("MemoriesName")]
    public List<string> choicesName = new List<string>();

    [SerializeField]
    private int rightChoice = 0;

    [SerializeField, TextArea]
    private List<string> rightChoiceSentences = new List<string>();

    [SerializeField, TextArea]
    private List<string> badChoiceSentences = new List<string>();

    [SerializeField, ValueDropdown("MemoriesName")]
    public string memoryToGive;

    [SerializeField]
    public Sprite memoryIcon;

    [SerializeField, FoldoutGroup("Sounds")]
    AudioClip[] saysomething;

    [SerializeField, FoldoutGroup("Sounds")]
    AudioClip[] asksomething;

    private Transform playerTransform;

    private UIManager uiManager;

    private bool alreadyIntroduceOnce;

    private AudioSource audioSource;

    public List<string> currentChoices = new List<string>();

    //[HideInInspector]
    //public bool alreadyGiveTheirMemory;

    public CharacterState CurrentCharacterState = CharacterState.Walking;

    public enum CharacterState
    {
        Walking,
        Introducing,
        Asking,
        Fail,
        Listening,
        Succeed
    }

    int currentSentence = 0;

    void Start()
    {
        playerTransform = FindObjectOfType<Player>().transform;
        uiManager = FindObjectOfType<UIManager>();
        audioSource = GetComponent<AudioSource>();
        currentChoices = new List<string>(choicesName);
    }

    void Update()
    {
        transform.LookAt(playerTransform, Vector3.up);
        //transform.forward = (playerTransform.position - transform.position).normalized;
        //transform.up = Vector3.up;
    }

    public void LetsTalk()
    {
        if (CurrentCharacterState == CharacterState.Walking)
        {
            CurrentCharacterState = CharacterState.Introducing;
        }

        InspectorEnd inspectorEnd = GetComponent<InspectorEnd>();
        if (inspectorEnd != null)
        {
            choicesName.Clear();
            if (uiManager.vignetteIconParent.childCount < 4)
            {
                choicesName.Add(inspectorEnd.notEnoughMemories);
                currentChoices = new List<string>(choicesName);
            }
            else
            {
                choicesName.Add(inspectorEnd.enoughMemories);
                currentChoices = new List<string>(choicesName);
            }
        }

        if (CurrentCharacterState == CharacterState.Introducing)
        {
            if (alreadyIntroduceOnce)
            {
                if (currentSentence < otherIntroSentences.Count)
                {
                    uiManager.ChangeChatBoxText(otherIntroSentences[currentSentence]);
                    currentSentence++;
                    if (!audioSource.isPlaying && saysomething != null && saysomething.Length > 0)
                    {
                        audioSource.Stop();
                        audioSource.PlayOneShot(saysomething[Random.Range(0, saysomething.Length)]);
                    }
                }
                else
                {
                    CurrentCharacterState = CharacterState.Asking;
                    currentSentence = 0;
                    if (asksomething != null && asksomething.Length > 0)
                    {
                        audioSource.Stop();
                        audioSource.PlayOneShot(asksomething[Random.Range(0, asksomething.Length)]);
                    }
                    uiManager.AskSomething(currentChoices);
                }
            }
            else
            {
                if (currentSentence < introSentences.Count)
                {
                    uiManager.ChangeChatBoxText(introSentences[currentSentence]);
                    currentSentence++;
                    if (!audioSource.isPlaying && saysomething != null && saysomething.Length > 0)
                    {
                        audioSource.Stop();
                        audioSource.PlayOneShot(saysomething[Random.Range(0, saysomething.Length)]);
                    }
                }
                else
                {
                    CurrentCharacterState = CharacterState.Asking;
                    if (asksomething != null && asksomething.Length > 0)
                    {
                        audioSource.Stop();
                        audioSource.PlayOneShot(asksomething[Random.Range(0, asksomething.Length)]);
                    }
                    currentSentence = 0;
                    alreadyIntroduceOnce = true;
                    uiManager.AskSomething(currentChoices);
                }
            }

        }
        if (CurrentCharacterState == CharacterState.Succeed)
        {
            if (currentSentence < rightChoiceSentences.Count)
            {
                uiManager.ChangeChatBoxText(rightChoiceSentences[currentSentence]);
                currentSentence++;
                if (!audioSource.isPlaying && saysomething != null && saysomething.Length > 0)
                {
                    audioSource.Stop();
                    audioSource.PlayOneShot(saysomething[Random.Range(0, saysomething.Length)]);
                }
            }
            else
            {
                CurrentCharacterState = CharacterState.Walking;
                currentSentence = 0;
                gameObject.layer = 0;
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.layer = 0;
                }
                GameManager.Instance.ChangeState(GameManager.GameState.Walking);

                uiManager.GiveMemory(memoryIcon);
                //alreadyGiveTheirMemory = true;
            }
        }
        if (CurrentCharacterState == CharacterState.Fail)
        {
            if (currentSentence < badChoiceSentences.Count)
            {
                uiManager.ChangeChatBoxText(badChoiceSentences[currentSentence]);
                currentSentence++;
                if (!audioSource.isPlaying && saysomething != null && saysomething.Length > 0)
                {
                    audioSource.Stop();
                    audioSource.PlayOneShot(saysomething[Random.Range(0, saysomething.Length)]);
                }
            }
            else
            {
                CurrentCharacterState = CharacterState.Walking;
                currentSentence = 0;
                GameManager.Instance.ChangeState(GameManager.GameState.Walking);
            }
        }
    }

    public void CheckRightChoice(int choice)
    {
        InspectorEnd inspectorEnd = GetComponent<InspectorEnd>();
        Debug.Log(choicesName.IndexOf(currentChoices[choice]));
        if (inspectorEnd == null && choicesName.IndexOf(currentChoices[choice]) == rightChoice || inspectorEnd != null && uiManager.vignetteIconParent.childCount >= 4)
        {
            uiManager.FadeWhite(true);
            CurrentCharacterState = CharacterState.Listening;
            if (inspectorEnd == null)
            {
                uiManager.TellMemory(GameManager.Instance.GetMemoryByName(currentChoices[choice]));
            }
            else
            {
                uiManager.TellMemory(inspectorEnd.endMemory);
            }
        }
        else
        {
            CurrentCharacterState = CharacterState.Fail;
            LetsTalk();
        }

        currentChoices.RemoveAt(choice);

    }

    private List<string> MemoriesName()
    {
        List<string> memories = new List<string>();

        foreach (Memory memory in GameManager.Instance.Memories)
        {
            memories.Add(memory.Name);
        }

        return memories;
    }
}
