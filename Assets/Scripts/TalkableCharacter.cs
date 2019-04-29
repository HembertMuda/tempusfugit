using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TalkableCharacter : MonoBehaviour
{
    [SerializeField]
    public Transform camFramingPoint;

    [SerializeField, TextArea]
    private List<string> introSentences = new List<string>();

    [SerializeField, TextArea]
    private List<string> otherIntroSentences = new List<string>();

    [SerializeField, HideInInspector]
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

    private Transform playerTransform;

    private UIManager uiManager;

    private bool alreadyIntroduceOnce;

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

        if (CurrentCharacterState == CharacterState.Introducing)
        {
            if (alreadyIntroduceOnce)
            {
                if (currentSentence < otherIntroSentences.Count)
                {
                    uiManager.ChangeChatBoxText(otherIntroSentences[currentSentence]);
                    currentSentence++;
                }
                else
                {
                    CurrentCharacterState = CharacterState.Asking;
                    currentSentence = 0;
                    uiManager.AskSomething(choicesName);
                }
            }
            else
            {
                if (currentSentence < introSentences.Count)
                {
                    uiManager.ChangeChatBoxText(introSentences[currentSentence]);
                    currentSentence++;
                }
                else
                {
                    CurrentCharacterState = CharacterState.Asking;
                    currentSentence = 0;
                    alreadyIntroduceOnce = true;
                    uiManager.AskSomething(choicesName);
                }
            }

        }
        if (CurrentCharacterState == CharacterState.Succeed)
        {
            if (currentSentence < rightChoiceSentences.Count)
            {
                uiManager.ChangeChatBoxText(rightChoiceSentences[currentSentence]);
                currentSentence++;
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
        if (inspectorEnd == null && choice == rightChoice || inspectorEnd != null && uiManager.vignetteIconParent.childCount >= 4)
        {
            uiManager.FadeWhite(true);
            CurrentCharacterState = CharacterState.Listening;
            if(inspectorEnd == null)
            {
            uiManager.TellMemory(GameManager.Instance.GetMemoryByName(choicesName[choice]));
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

        choicesName.RemoveAt(choice);

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
