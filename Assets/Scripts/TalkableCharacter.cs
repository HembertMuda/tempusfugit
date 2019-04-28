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
                uiManager.AskSomething(choices);
            }
        }

        if (currentCharacterState == CharacterState.Introducing)
        {

        }
    }
}
