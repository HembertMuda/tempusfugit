using UnityEngine;

public class ObjectParallax : MonoBehaviour
{
    private Train train;
    private MovingObjects movingObjects;

    void Awake()
    {
        train = FindObjectOfType<Train>();
        movingObjects = FindObjectOfType<MovingObjects>();
    }

    void Update()
    {
        if (transform.position.z > movingObjects.PosZToDisappear)
        {
            transform.Translate(-Vector3.forward * (1 / Mathf.Abs(transform.position.x)) * train.MoveSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            InitObject();
        }
    }

    public void InitObject()
    {
        transform.position = new Vector3(Random.Range(movingObjects.rangeXToSpawn.x, movingObjects.rangeXToSpawn.y) * (Random.Range(0, 2) == 0 ? 1f : -1f), 0f, Random.Range(movingObjects.rangeZToSpawn.x, movingObjects.rangeZToSpawn.y));
    }
}
