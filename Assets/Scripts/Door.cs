using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    private Animator doorAnimator = null;

    public bool isOpened;

    public void Open()
    {
        doorAnimator.SetTrigger("open");
        isOpened = true;
    }

    public void Close()
    {
        doorAnimator.SetTrigger("close");
        isOpened = false;
    }
}
