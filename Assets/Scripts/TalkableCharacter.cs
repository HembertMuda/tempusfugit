using UnityEngine;

public class TalkableCharacter : MonoBehaviour
{
    [SerializeField]
    public Transform camFramingPoint;

    private Transform playerTransform;

    void Start()
    {
        playerTransform = FindObjectOfType<Player>().transform;
    }

    void Update()
    {
        transform.LookAt(playerTransform, Vector3.up);
    }
}
