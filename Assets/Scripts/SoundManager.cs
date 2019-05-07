using UnityEngine;
using DG.Tweening;
using System.Collections;

public class SoundManager : MonoBehaviourSingleton<SoundManager>
{
    [SerializeField]
    private AudioClip musicClip = null;

    [SerializeField]
    private float musicVolume;

    [SerializeField]
    private AudioClip ambiantClip = null;

    [SerializeField]
    private float ambiantVolume;

    private AudioSource sdAudioSource;

    private Tween musicTween;

    void Start()
    {
        sdAudioSource = GetComponent<AudioSource>();
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
