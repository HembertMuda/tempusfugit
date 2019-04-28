using DG.Tweening;
using System.Collections;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    //[SerializeField]
    //private float shakeStrengh = 0.5f;

    [SerializeField]
    private Vector2 randomStrenghX = new Vector2(0.4f, 1f);

    [SerializeField]
    private Vector2 randomStrenghZ = new Vector2(0f, 0.5f);

    [SerializeField]
    private Vector2 randomFrequency = new Vector2(0.3f, 1.2f);

    void Start()
    {
        StartCoroutine(RandomShake());
    }

    IEnumerator RandomShake()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(randomFrequency.x, randomFrequency.y));
            //if (GameManager.Instance.CurrentGameState == GameManager.GameState.Walking)
            transform.DOPunchRotation(new Vector3(Random.Range(randomStrenghX.x, randomStrenghX.y) * (Random.Range(0, 2) == 0 ? 1f : -1f), 0f, Random.Range(randomStrenghZ.x, randomStrenghZ.y)), 0.3f, 1, 0.2f);
        }
    }
}
