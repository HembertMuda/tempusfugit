using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TalkableCharacter : MonoBehaviour
{
    [SerializeField]
    public Transform camFramingPoint;

    [SerializeField, TextArea]
    private List<string> introSentences = new List<string>();

    [SerializeField, TextArea, ValueDropdown("MemoriesName")]
    private List<string> otherIntroSentences = new List<string>();

    [SerializeField]
    private List<string> choicesName = new List<string>();

    [SerializeField]
    private int rightChoice = 0;

    [SerializeField, TextArea]
    private List<string> rightChoiceSentences = new List<string>();

    [SerializeField, TextArea]
    private List<string> badChoiceSentences = new List<string>();

    private Transform playerTransform;

    private UIManager uiManager;

    private bool alreadyIntroduceOnce;

    //[HideInInspector]
    //public bool alreadyGiveTheirMemory;

    private CharacterState currentCharacterState = CharacterState.Walking;

    public enum CharacterState
    {
        Walking,
        Introducing,
        Asking,
        Fail,
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
        if (currentCharacterState == CharacterState.Walking)
        {
            currentCharacterState = CharacterState.Introducing;
        }

        if (currentCharacterState == CharacterState.Introducing)
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
                    currentCharacterState = CharacterState.Asking;
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
                    currentCharacterState = CharacterState.Asking;
                    currentSentence = 0;
                    alreadyIntroduceOnce = true;
                    uiManager.AskSomething(choicesName);
                }
            }

        }
        if (currentCharacterState == CharacterState.Succeed)
        {
            if (currentSentence < rightChoiceSentences.Count)
            {
                uiManager.ChangeChatBoxText(rightChoiceSentences[currentSentence]);
                currentSentence++;
            }
            else
            {
                currentCharacterState = CharacterState.Walking;
                currentSentence = 0;
                gameObject.layer = 0;
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.layer = 0;
                }
                GameManager.Instance.ChangeState(GameManager.GameState.Walking);
                //alreadyGiveTheirMemory = true;
            }
        }
        if (currentCharacterState == CharacterState.Fail)
        {
            if (currentSentence < badChoiceSentences.Count)
            {
                uiManager.ChangeChatBoxText(badChoiceSentences[currentSentence]);
                currentSentence++;
            }
            else
            {
                currentCharacterState = CharacterState.Walking;
                currentSentence = 0;
                GameManager.Instance.ChangeState(GameManager.GameState.Walking);
            }
        }
    }

    public void CheckRightChoice(int choice)
    {
        if (choice == rightChoice)
        {
            uiManager.FadeWhite(true);
            currentCharacterState = CharacterState.Succeed;
            uiManager.TellMemory(GameManager.Instance.GetMemoryByName(choicesName[choice]));
        }
        else
        {
            currentCharacterState = CharacterState.Fail;
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
