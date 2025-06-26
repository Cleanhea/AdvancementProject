using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatEvent : MonoBehaviour
{
    public static BeatEvent instance;
    public GameObject leftCirclePrefab;
    public GameObject rightCirclePrefab;
    public GameObject leftCircle;//유저가 조정하는것
    public GameObject rightCircle;// 유저가 조정하는것
    Vector3 leftPoint;
    Vector3 rightPoint;
    [SerializeField]
    Vector3 leftCirclePosition = new Vector3(-1.0f, -3.0f, 0);
    [SerializeField]
    Vector3 rightCirclePosition = new Vector3(1.0f, -3.0f, 0);
    [SerializeField]
    float pointOffset = 6.0f;
    [SerializeField]
    float footHoldOffset = 3.0f;
    Queue<GameObject> leftFootHoldQueue = new Queue<GameObject>();
    Queue<GameObject> rightFootHoldQueue = new Queue<GameObject>();

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        leftCircle = Instantiate(leftCirclePrefab, leftCirclePosition, Quaternion.identity);
        rightCircle = Instantiate(rightCirclePrefab, rightCirclePosition, Quaternion.identity);
        leftPoint = leftCircle.transform.position;
        rightPoint = rightCircle.transform.position;
    }
    // 비트 발생시 처리 기능
    public void BeatHandling(Notes notes)
    {
        ref Vector3 point = ref leftPoint;
        ref Queue<GameObject> footHoldQueue = ref leftFootHoldQueue;
        if (notes.type == 1)
        {
            point = ref rightPoint;
            footHoldQueue = ref rightFootHoldQueue;
        }
        GameObject footHold = FootHoldObjectFool.instance.GetFootHold();
        footHoldQueue.Enqueue(footHold);
        switch (notes.direction)
        {
            case 0:
                footHold.transform.rotation = Quaternion.Euler(0, 0, 90f);
                footHold.transform.position = point + new Vector3(footHoldOffset, 0, 0);
                point += new Vector3(pointOffset, 0, 0);
                break;
            case 1:
                footHold.transform.rotation = Quaternion.Euler(0, 0, 90f);
                footHold.transform.position = point + new Vector3(-footHoldOffset, 0, 0);
                point += new Vector3(-pointOffset, 0, 0);
                break;
            case 2:
                footHold.transform.rotation = Quaternion.Euler(0, 0, 0);
                footHold.transform.position = point + new Vector3(0, footHoldOffset, 0);
                point += new Vector3(0, pointOffset, 0);
                break;
            case 3:
                footHold.transform.rotation = Quaternion.Euler(0, 0, 0);
                footHold.transform.position = point + new Vector3(0, -footHoldOffset, 0);
                point += new Vector3(0, -pointOffset, 0);
                break;
        }
        if (footHoldQueue.Count > 3)
        {
            FootHoldObjectFool.instance.ReturnFootHold(footHoldQueue.Dequeue());
        }
    }
}
