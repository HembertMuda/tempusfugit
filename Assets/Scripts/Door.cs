using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    private Animator doorAnimator = null;

    [SerializeField]
    private AudioClip openClip = null;

    [SerializeField]
    private AudioClip closeClip = null;

    public bool isOpened;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Open()
    {
        doorAnimator.SetTrigger("open");
        audioSource.PlayOneShot(openClip);
        isOpened = true;
    }

    public void Close()
    {
        doorAnimator.SetTrigger("close");
        audioSource.PlayOneShot(closeClip);
        isOpened = false;
    }
}
