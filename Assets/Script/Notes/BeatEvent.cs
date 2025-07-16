using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


[DefaultExecutionOrder(1)]
public class BeatEvent : MonoBehaviour
{
    public static BeatEvent instance;
    //---------프리펩-----------
    public GameObject leftCirclePrefab;
    public GameObject rightCirclePrefab;
    public GameObject guideCirclePrefab;

    //---------게임 오브젝트-----------
    public GameObject leftGuideCircle;
    public GameObject rightGuideCircle;
    GameObject leftCircle;//유저가 조정하는것
    GameObject rightCircle;// 유저가 조정하는것

    //---------좌표-----------
    public Vector3 leftPoint;
    public Vector3 rightPoint;

    //---------카메라 관련-----------
    public Camera mainCamera;
    public float cameraSpeedOffset = 0.2f;
    public Vector3 cameraPoint = new Vector3(0, 0, -10);

    //---------시작 위치-----------
    public Vector3 startLeftCirclePosition = new Vector3(-3.5f, -3.0f, 0);
    public Vector3 startRightCirclePosition = new Vector3(3.5f, -3.0f, 0);

    //---------포인트 관련-----------
    [SerializeField]
    float pointOffset = 6.0f;
    [SerializeField]
    float footHoldOffset = 3.0f;
    static Vector3[] dirUnit = new Vector3[4]
    {
        new Vector3(-1f, 0, 0),
        new Vector3(1f, 0, 0),
        new Vector3(0, 1f, 0),
        new Vector3(0, -1f, 0)
    };
    static Vector3[] dirRotation = new Vector3[4]
    {
        new Vector3(0, 0, 90f),
        new Vector3(0, 0, 270f),
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 180f)
    };

    //---------포인트 큐-----------
    public Queue<NoteCreate> leftFootHoldQueue = new Queue<NoteCreate>();
    public Queue<NoteCreate> rightFootHoldQueue = new Queue<NoteCreate>();
    public Queue<Vector3> leftCirclePositionQueue = new Queue<Vector3>();
    public Queue<Vector3> rightCirclePositionQueue = new Queue<Vector3>();

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    void OnEnable()
    {
        if (BeatManager.instance != null)
        {
            BeatManager.instance.OnNoteSpawn += BeatHandling;
        }
    }
    void OnDisable()
    {
        if (BeatManager.instance != null)
        {
            BeatManager.instance.OnNoteSpawn -= BeatHandling;
        }
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
        if (notes.type == 0 || notes.type == 1)
        {
            CreateNote(notes);
        }
        else if (notes.type == 2)
        {
            if (notes.sevent == "save")
            {
                GameManager.instance.SaveGame();
            }
            else if (notes.sevent == "setCamera")
            {
                StartCoroutine(AbsSetCameraPos(new Vector3(notes.cameraPosition.x, notes.cameraPosition.y, -10), notes.cameraZoom));
            }
            return;
        }
        
    }

    public void CreateNote(Notes notes)
    {
        ref Vector3 point = ref leftPoint;
        ref Queue<Vector3> queue = ref leftCirclePositionQueue;
        if (notes.type == 1)
        {
            queue = ref rightCirclePositionQueue;
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
        footHold.transform.rotation = Quaternion.Euler(dirRotation[notes.direction]);
        footHold.transform.position = point + dirUnit[notes.direction] * footHoldOffset;
        point += dirUnit[notes.direction] * pointOffset;
        queue.Enqueue(point);
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
        MoveCircleStart(tr, position + dirUnit[dir] * pointOffset);
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

    public IEnumerator AbsSetCameraPos(Vector3 vec,float cameraZoom)
    {
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm * 4f);
        cameraPoint = new Vector3(vec.x, vec.y, -10);
        mainCamera.transform.DOMove(new Vector3(vec.x, vec.y, -10), cameraSpeedOffset).SetEase(Ease.InOutSine);
        mainCamera.DOOrthoSize(cameraZoom, cameraSpeedOffset).SetEase(Ease.InOutSine);
        
    }

    public CameraPos GetCameraPos()
    {
        Debug.Log("GetCameraPos: " + cameraPoint);
        CameraPos temp = new CameraPos();
        temp.x = cameraPoint.x;
        temp.y = cameraPoint.y;
        return temp;
    }
    public float GetCameraZoom()
    {
        return mainCamera.orthographicSize;
    }
    public void SetCameraZoom(float set)
    {
        mainCamera.DOOrthoSize(mainCamera.orthographicSize + set, cameraSpeedOffset).SetEase(Ease.InOutSine);
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
        Queue<Vector3> positionQueue = leftCirclePositionQueue;
        if (type == 1)
        {
            guideCircle = rightGuideCircle;
            positionQueue = rightCirclePositionQueue;
        }
        Vector3 position = positionQueue.Peek();
        positionQueue.Dequeue();
        return guideCircle.transform.DOMove(position, duration).SetEase(Ease.InOutSine);
    }
}
