using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootHoldObjectFool : MonoBehaviour
{
    public static FootHoldObjectFool instance;
    public GameObject FootHoldPrefab;
    Queue<GameObject> footHoldQueue = new Queue<GameObject>();
    [SerializeField]
    int maxFootHoldCount = 20;

    void Awake()
    {
        instance = this;
        for (int i = 0; i < maxFootHoldCount; i++)
        {
            GameObject footHold = Instantiate(FootHoldPrefab, Vector3.zero, Quaternion.identity,transform);
            footHold.SetActive(false);
            footHoldQueue.Enqueue(footHold);
        }
    }
    public GameObject GetFootHold()
    {
        GameObject footHold = footHoldQueue.Dequeue();
        footHold.SetActive(true);
        return footHold;
    }
    public void ReturnFootHold(GameObject footHold)
    {
        footHold.SetActive(false);
        footHoldQueue.Enqueue(footHold);
    }
}
