using System.Collections.Generic;
using UnityEngine;

public class MovingObjects : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> objectsToInstantiate = new List<GameObject>();

    [SerializeField]
    public Vector2 rangeXToSpawn = new Vector2(4f, 15f);

    [SerializeField]
    public Vector2 rangeZToSpawn = new Vector2(30f, 60f);

    [SerializeField]
    private Vector2Int randomNumberObjectToSpawn = new Vector2Int(5, 20);

    [SerializeField]
    public float PosZToDisappear = -50f;

    //private List<Transform> objectsInstantiate = new List<Transform>();

    void Start()
    {
        for (int i = 0; i < objectsToInstantiate.Count; i++)
        {
            for (int j = 0; j < Random.Range(randomNumberObjectToSpawn.x, randomNumberObjectToSpawn.y); j++)
            {
                ObjectParallax newMovingObject = Instantiate(objectsToInstantiate[i], this.transform).GetComponent<ObjectParallax>();
                newMovingObject.InitObject();
                //newMovingObject.transform.position = new Vector3(Random.Range(rangeXToSpawn.x, rangeXToSpawn.y), 0f, Random.Range(rangeXToSpawn.x, rangeXToSpawn.y));
            }
        }
    }

    //void Update()
    //{

    //}
}
