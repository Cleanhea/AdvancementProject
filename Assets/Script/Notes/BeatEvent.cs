using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BeatEvent : MonoBehaviour
{
    public static BeatEvent instance;
    public GameObject leftCirclePrefab;
    public GameObject rightCirclePrefab;
    public GameObject guideCirclePrefab;

    public GameObject leftGuideCircle;
    public GameObject rightGuideCircle;
    GameObject leftCircle;//유저가 조정하는것
    GameObject rightCircle;// 유저가 조정하는것
    public Vector3 leftPoint;
    public Vector3 rightPoint;
    public Camera mainCamera;
    public float cameraSpeedOffset = 0.2f;
    public Vector3 cameraPoint = new Vector3(0, 0, -10);

    public Vector3 startLeftCirclePosition = new Vector3(-3.5f, -3.0f, 0);
    public Vector3 startRightCirclePosition = new Vector3(3.5f, -3.0f, 0);

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
        leftCircle = Instantiate(leftCirclePrefab, startLeftCirclePosition, Quaternion.identity);
        rightCircle = Instantiate(rightCirclePrefab, startRightCirclePosition, Quaternion.identity);
        leftGuideCircle = Instantiate(guideCirclePrefab, startLeftCirclePosition, Quaternion.identity);
        rightGuideCircle = Instantiate(guideCirclePrefab, startRightCirclePosition, Quaternion.identity);
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

    void MoveCircleStart(Transform tr, Vector3 vec)
    {
        tr.DOMove(vec, 0.08f);
    }

    public void SetPoint(Vector3 left, Vector3 right)
    {
        leftPoint = left;
        rightPoint = right;
        leftCircle.transform.position = leftPoint;
        rightCircle.transform.position = rightPoint;
        leftGuideCircle.transform.position = leftPoint;
        rightGuideCircle.transform.position = rightPoint;
    }
    public void MoveCamera(Vector3 vec)
    {
        cameraPoint += new Vector3(vec.x, vec.y);
        mainCamera.transform.DOMove(mainCamera.transform.position + new Vector3(vec.x, vec.y, -10), cameraSpeedOffset).SetEase(Ease.InOutSine);
    }

    public void SetCameraPos(Vector3 vec)
    {
        mainCamera.transform.position = new Vector3(vec.x, vec.y, -10);
        cameraPoint = new Vector3(vec.x, vec.y, -10);
    }

    public CameraPos GetCameraPos()
    {
        Debug.Log("GetCameraPos: " + cameraPoint);
        CameraPos temp = new CameraPos();
        temp.x = cameraPoint.x;
        temp.y = cameraPoint.y;
        return temp;
    }

    public void SetCameraZoom(float set)
    {
        mainCamera.DOOrthoSize(mainCamera.orthographicSize+set,cameraSpeedOffset).SetEase(Ease.InOutSine);
    }

    public void SetCircle()
    {
        leftCircle.transform.position = startLeftCirclePosition;
        rightCircle.transform.position = startRightCirclePosition;
        leftPoint = startLeftCirclePosition;
        rightPoint = startRightCirclePosition;
    }

    public Tween MoveGuideCircle(int dir, int type, float duration)
    {
        GameObject guideCircle = leftGuideCircle;
        if (type == 1)
        {
            guideCircle = rightGuideCircle;
        }
        Transform tr = guideCircle.transform;
        Vector3 position = tr.position;
        if (dir == 0)
        {
            return guideCircle.transform.DOLocalMove(position + new Vector3(-pointOffset, 0, 0), duration).SetEase(Ease.InOutSine);
        }
        else if (dir == 1)
        {
            return guideCircle.transform.DOLocalMove(position + new Vector3(pointOffset, 0, 0), duration).SetEase(Ease.InOutSine);
        }
        else if (dir == 2)
        {
            return guideCircle.transform.DOLocalMove(position + new Vector3(0, pointOffset, 0), duration).SetEase(Ease.InOutSine);

        }
        else if (dir == 3)
        {
            return guideCircle.transform.DOLocalMove(position + new Vector3(0, -pointOffset, 0), duration).SetEase(Ease.InOutSine);
        }
        else
        {
            return null;
        }
    }
}
