using UnityEngine;

public class InspectorEnd : MonoBehaviour
{
    [SerializeField] public string notEnoughMemories = "I didn't collect enough memories";
    [SerializeField] public string enoughMemories = "Tell all memories";
    [SerializeField] public Memory endMemory;

    private TalkableCharacter talkableCharacter;

    void Start()
    {
        talkableCharacter = GetComponent<TalkableCharacter>();
    }

    //void Update()
    //{
    //    if (talkableCharacter.CurrentCharacterState == TalkableCharacter.CharacterState.Introducing)
    //    {
    //        talkableCharacter.choicesName = new List<string>();
    //        if (FindObjectOfType<UIManager>().vignetteIconParent.childCount < 4)
    //        {
    //            talkableCharacter.choicesName.Add(notEnoughMemories);
    //        }
    //        else
    //        {
    //            talkableCharacter.choicesName.Add(enoughMemories);
    //        }
    //    }
    //}
}
