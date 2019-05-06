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
}
