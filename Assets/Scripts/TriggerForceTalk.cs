using UnityEngine;

public class TriggerForceTalk : MonoBehaviour
{
    [SerializeField]
    public TalkableCharacter forceTalkableCharacter = null;

    private BoxCollider boxCollider;

    void Start()
    {
        boxCollider = GetComponentInChildren<BoxCollider>();
    }

    public void Disable()
    {

        boxCollider.enabled = false;
    }
}
