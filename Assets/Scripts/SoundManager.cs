using DG.Tweening;
using UnityEngine;

public class SoundManager : MonoBehaviourSingleton<SoundManager>
{
    [SerializeField]
    private AudioClip musicClip = null;

    [SerializeField]
    private float musicVolume = 0f;

    [SerializeField]
    private AudioClip ambiantClip = null;

    [SerializeField]
    private float ambiantVolume = 0f;

    private AudioSource sdAudioSource;

    private Tween musicTween;

    protected override void Awake()
    {
        base.Awake();
        sdAudioSource = GetComponent<AudioSource>();
        sdAudioSource.volume = musicVolume;
    }

    public void StopMusic()
    {
        if (musicTween != null)
        {
            musicTween.Kill();
        }

        musicTween = sdAudioSource.DOFade(0f, 3f).SetEase(Ease.OutCubic);
    }

    public void FadeMusic(bool ambiant)
    {
        if (ambiant && sdAudioSource.clip == ambiantClip || !ambiant && sdAudioSource.clip == musicClip)
        {
            return;
        }

        if (musicTween != null)
        {
            musicTween.Kill();
        }

        if (sdAudioSource.clip != null)
        {
            musicTween = sdAudioSource.DOFade(0f, 3f).SetEase(Ease.OutCubic).OnComplete(() => SetMusic(ambiant));
        }
        else
        {
            SetMusic(ambiant);
        }
    }

    private void SetMusic(bool ambiant)
    {
        sdAudioSource.clip = ambiant ? ambiantClip : musicClip;

        sdAudioSource.Play();

        musicTween = sdAudioSource.DOFade(ambiant ? ambiantVolume : musicVolume, 3f).SetEase(Ease.OutCubic);
    }
}
