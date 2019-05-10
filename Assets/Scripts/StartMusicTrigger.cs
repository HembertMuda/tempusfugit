using UnityEngine;

public class StartMusicTrigger : MonoBehaviour
{
    private BoxCollider startMusicBoxCollider;

    private void Start()
    {
        startMusicBoxCollider = GetComponent<BoxCollider>();
    }

    public void StartMusic()
    {
        SoundManager.Instance.FadeMusic(true);
        startMusicBoxCollider.enabled = false;
    }
}
