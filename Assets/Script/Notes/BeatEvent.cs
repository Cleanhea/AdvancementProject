using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

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
    Vector3 leftCirclePosition = new Vector3(-3.5f, -3.0f, 0);
    [SerializeField]
    Vector3 rightCirclePosition = new Vector3(3.5f, -3.0f, 0);
    [SerializeField]
    float pointOffset = 6.0f;
    [SerializeField]
    float footHoldOffset = 3.0f;
    public Queue<NoteCreate> leftFootHoldQueue = new Queue<NoteCreate>();
    public Queue<NoteCreate> rightFootHoldQueue = new Queue<NoteCreate>();

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
        if (notes.type == 1)
        {
            point = ref rightPoint;
        }
        GameObject footHold = FootHoldObjectFool.instance.GetFootHold();
        NoteCreate noteCreate = footHold.GetComponent<NoteCreate>();
        noteCreate.noteData = notes;
        if (notes.type == 0)
        {
            noteCreate.isLeft = true;
            leftFootHoldQueue.Enqueue(noteCreate);
        }
        else
        {
            noteCreate.isLeft = false;
            rightFootHoldQueue.Enqueue(noteCreate);
        }
        switch (notes.direction)
        {
            case 0:
                footHold.transform.rotation = Quaternion.Euler(0, 0, 90f);
                footHold.transform.position = point + new Vector3(-footHoldOffset, 0, 0);
                point += new Vector3(-pointOffset, 0, 0);
                break;
            case 1:
                footHold.transform.rotation = Quaternion.Euler(0, 0, 270f);
                footHold.transform.position = point + new Vector3(footHoldOffset, 0, 0);
                point += new Vector3(pointOffset, 0, 0);
                break;
            case 2:
                footHold.transform.rotation = Quaternion.Euler(0, 0, 0);
                footHold.transform.position = point + new Vector3(0, footHoldOffset, 0);
                point += new Vector3(0, pointOffset, 0);
                break;
            case 3:
                footHold.transform.rotation = Quaternion.Euler(0, 0, 180f);
                footHold.transform.position = point + new Vector3(0, -footHoldOffset, 0);
                point += new Vector3(0, -pointOffset, 0);
                break;
        }
    }
    public void MoveCircle(int dir, int type)
    {
        GameObject circle = leftCircle;
        if (type == 1)
        {
            circle = rightCircle;
        }
        Transform tr = circle.transform;
        Vector3 position = tr.position;
        if (dir == 0)
        {
            MoveCircleStart(tr, position + new Vector3(-pointOffset, 0, 0));
        }
        else if (dir == 1)
        {
            MoveCircleStart(tr, position + new Vector3(pointOffset, 0, 0));
        }
        else if (dir == 2)
        {
            MoveCircleStart(tr, position + new Vector3(0, pointOffset, 0));
        }
        else if (dir == 3)
        {
            MoveCircleStart(tr, position + new Vector3(0, -pointOffset, 0));
        }
    }

    void MoveCircleStart(Transform tr,Vector3 vec) {
        tr.DOMove(vec, 0.08f);
    }
}
