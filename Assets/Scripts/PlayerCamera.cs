using DG.Tweening;
using System.Collections;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private float shakeStrengh = 0.5f;

    private Vector3 camLocalPosInit;

    void Start()
    {
        camLocalPosInit = transform.localPosition;
        StartCoroutine(RandomShake());
    }

    public void AdaptCam(TalkableCharacter talkableCharacter)
    {
        transform.DOMove(talkableCharacter.camFramingPoint.position, 1f).SetEase(Ease.OutCubic);
        transform.DORotate(talkableCharacter.camFramingPoint.eulerAngles, 1f).SetEase(Ease.OutCubic);
    }

    IEnumerator RandomShake()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.3f, 1.2f));
            if (GameManager.Instance.CurrentGameState == GameManager.GameState.Walking)
                transform.DOPunchRotation(new Vector3(shakeStrengh * (Random.Range(0, 2) == 0 ? 1f : -1f), 0f, 0f), 0.3f, 1, 0.2f);
        }
    }
}
