using System.Collections.Generic;
using UnityEngine;

public class TalkableCharacter : MonoBehaviour
{
    [SerializeField]
    public Transform camFramingPoint;

    [SerializeField, TextArea]
    private List<string> introSentences = new List<string>();

    [SerializeField, TextArea]
    private List<string> choices = new List<string>();

    [SerializeField]
    private int rightChoice = 0;

    [SerializeField, TextArea]
    private List<string> rightChoiceSentences = new List<string>();

    [SerializeField, TextArea]
    private List<string> badChoiceSentences = new List<string>();

    private Transform playerTransform;

    private UIManager uiManager;

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
            if (currentSentence < introSentences.Count)
            {
                uiManager.ChangeChatBoxtext(introSentences[currentSentence]);
                currentSentence++;
            }
            else
            {
                currentCharacterState = CharacterState.Asking;
                currentSentence = 0;
                uiManager.AskSomething(choices);
            }
        }
        if (currentCharacterState == CharacterState.Succeed)
        {
            if (currentSentence < rightChoiceSentences.Count)
            {
                uiManager.ChangeChatBoxtext(rightChoiceSentences[currentSentence]);
                currentSentence++;
            }
            else
            {
                currentCharacterState = CharacterState.Walking;
                currentSentence = 0;
                GameManager.Instance.ChangeState(GameManager.GameState.Walking);
            }
        }
        if (currentCharacterState == CharacterState.Fail)
        {
            if (currentSentence < badChoiceSentences.Count)
            {
                uiManager.ChangeChatBoxtext(badChoiceSentences[currentSentence]);
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
            currentCharacterState = CharacterState.Succeed;
        }
        else
        {
            currentCharacterState = CharacterState.Fail;
        }

        LetsTalk();
    }
}
