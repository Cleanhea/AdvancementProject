using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class FootHoldObjectFool : MonoBehaviour
{
    public static FootHoldObjectFool instance;
    public GameObject FootHoldPrefab;
    Queue<GameObject> footHoldQueue = new Queue<GameObject>();
    List<GameObject> footHoldList = new List<GameObject>();
    [SerializeField]
    int maxFootHoldCount = 40;

    void Awake()
    {
        instance = this;
        for (int i = 0; i < maxFootHoldCount; i++)
        {
            GameObject footHold = Instantiate(FootHoldPrefab, Vector3.zero, Quaternion.identity, transform);
            footHold.SetActive(false);
            footHoldQueue.Enqueue(footHold);
            footHoldList.Add(footHold);
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
    public void ResetQueue()
    {
        foreach (var footHold in footHoldList)
        {
            if (footHold.activeSelf)
            {
                footHold.SetActive(false);
                footHoldQueue.Enqueue(footHold);
            }
        }
    }

    public void AllReturnFootHold()
    {
        foreach (var footHold in footHoldList)
        {
            if (footHold.activeSelf)
            {
                footHold.SetActive(false);
                footHoldQueue.Enqueue(footHold);
            }
        }
    }
}
