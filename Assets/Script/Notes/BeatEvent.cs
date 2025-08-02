using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;


[DefaultExecutionOrder(1)]
public class BeatEvent : MonoBehaviour
{
    public static BeatEvent instance;
    //---------프리펩-----------
    [Header("---------게임 오브젝트 프리펩-----------")]
    public GameObject leftCirclePrefab;
    public GameObject rightCirclePrefab;
    public GameObject guideCirclePrefab;
    [Header("---------스테이지 연출 프리펩-----------")]
    public GameObject haiPhutHonLightPrefab;

    //---------게임 오브젝트-----------
    [HideInInspector] public GameObject leftGuideCircle;
    [HideInInspector] public GameObject rightGuideCircle;
    [HideInInspector] public GameObject stageLight;
    GameObject leftCircle;//유저가 조정하는것
    GameObject rightCircle;// 유저가 조정하는것

    //---------좌표-----------
    [HideInInspector] public Vector3 leftPoint;
    [HideInInspector] public Vector3 rightPoint;

    //---------카메라 관련-----------
    [Header("---------카메라 관련-----------")]
    public Camera mainCamera;
    public float cameraSpeedOffset = 0.2f;
    public Vector3 cameraPoint = new Vector3(0, 0, -10);

    //---------시작 위치-----------
    [HideInInspector] public Vector3 startLeftCirclePosition = new Vector3(-3.5f, -3.0f, 0);
    [HideInInspector] public Vector3 startRightCirclePosition = new Vector3(3.5f, -3.0f, 0);

    //---------포인트 관련-----------
    [Header("---------포인트 관련-----------")]
    [SerializeField]
    Color defaultColor;
    [SerializeField]
    Color inversionColor;
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

    //---------모드 bool값-----------
    [Header("---------연출 관련 값-----------")]
    public bool inversion = false;
    public bool afterInversion = false;

    //---------연출 관련 값-----------
    public static Action<bool> OnInversion;
    [SerializeField] float ClearCircleTime = 0.05f;
    [SerializeField] float ClearCircleSize = 1.15f;
    public Light2D globalLight2D;
    public GameObject lightTransform;
    Light2D leftLight;
    Light2D rightLight;
    Vector3 originCircleSize;


    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        originCircleSize = transform.localScale;
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

    public void BeatStart()
    {
        StartCoroutine(MusicStartSetCircle());
    }
    IEnumerator MusicStartSetCircle()
    {
        leftCircle = Instantiate(leftCirclePrefab, startLeftCirclePosition, Quaternion.identity);
        rightCircle = Instantiate(rightCirclePrefab, startRightCirclePosition, Quaternion.identity);
        leftGuideCircle = Instantiate(guideCirclePrefab, startLeftCirclePosition, Quaternion.identity);
        rightGuideCircle = Instantiate(guideCirclePrefab, startRightCirclePosition, Quaternion.identity);
        SpriteRenderer l = leftCircle.GetComponent<SpriteRenderer>();
        SpriteRenderer r = rightCircle.GetComponent<SpriteRenderer>();
        SpriteRenderer lg = leftGuideCircle.GetComponent<SpriteRenderer>();
        SpriteRenderer rg = rightGuideCircle.GetComponent<SpriteRenderer>();
        Color lc = l.color;
        Color rc = r.color;
        Color lgc = lg.color;
        Color rgc = rg.color;
        lc.a = 0f;
        rc.a = 0f;
        lgc.a = 0f;
        rgc.a = 0f;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / 1f);
            lc.a = alpha;
            rc.a = alpha;
            lgc.a = alpha;
            rgc.a = alpha;
            l.color = lc;
            r.color = rc;
            lg.color = lgc;
            rg.color = rgc;
            yield return null;
        }
        leftPoint = leftCircle.transform.position;
        rightPoint = rightCircle.transform.position;
    }
    IEnumerator ClearSetCircle()
    {
        SpriteRenderer l = leftCircle.GetComponent<SpriteRenderer>();
        SpriteRenderer r = rightCircle.GetComponent<SpriteRenderer>();
        SpriteRenderer lg = leftGuideCircle.GetComponent<SpriteRenderer>();
        SpriteRenderer rg = rightGuideCircle.GetComponent<SpriteRenderer>();
        Color lc = l.color;
        Color rc = r.color;
        Color lgc = lg.color;
        Color rgc = rg.color;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f-t / 1f);
            lc.a = alpha;
            rc.a = alpha;
            lgc.a = alpha;
            rgc.a = alpha;
            l.color = lc;
            r.color = rc;
            lg.color = lgc;
            rg.color = rgc;
            yield return null;
        }
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
            else if (notes.sevent == "setCameraZoom")
            {
                StartCoroutine(AbsSetCameraZoom(notes.cameraZoom));
            }
            else if (notes.sevent == "setCamera")
            {
                StartCoroutine(AbsSetCameraPos(new Vector3(notes.cameraPosition.x, notes.cameraPosition.y, -10)));
            }
            else if (notes.sevent == "inversion")
            {
                StartCoroutine(InversionMode());
            }
            else if (notes.sevent == "disInversion")
            {
                StartCoroutine(DefaultMode());
            }
            else if (notes.sevent == "clear")
            {
                StartCoroutine(Clear());
            }
            return;
        }
    }

    //--------------------------------------------노트 생성--------------------------------------------------
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

    //---------------------------------------원 이동 관련 로직--------------------------------------------
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

    //------------------------------------카메라 관련 로직--------------------------------------
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
    public IEnumerator AbsSetCameraZoom(float cameraZoom)
    {
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm * 4f);
        mainCamera.DOOrthoSize(cameraZoom, cameraSpeedOffset).SetEase(Ease.InOutSine);
    }

    public IEnumerator AbsSetCameraPos(Vector3 vec)
    {
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm * 4f);
        cameraPoint = new Vector3(vec.x, vec.y, -10);
        mainCamera.transform.DOMove(new Vector3(vec.x, vec.y, -10), cameraSpeedOffset).SetEase(Ease.InOutSine);
    }

    public CameraPos GetCameraPos()
    {
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

    //---------------------------------------- 연출 관련 로직 -----------------------------------------------------
    IEnumerator Clear()
    {
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm * 4f);
        DOTween.KillAll();
        PlayerPrefs.SetInt(BeatManager.instance.Playname + "Death", GameManager.instance.deathCount);
        PlayerPrefs.SetInt(BeatManager.instance.Playname + "Clear", 1);
        AudioManager.instance.StartCoroutine(AudioManager.instance.VolumeFadeOut());
        StartCoroutine(ClearSetCircle());
        yield return new WaitForSeconds(2f);
        AudioManager.instance.SetMusicVolume(AudioManager.instance.MusicVolume);
        AudioManager.instance.PlayMusic("event:/Lobi", 0);
        GameManager.instance.DefaultSaveData();
        GameManager.instance.deathCount = 0;
        SceneManager.LoadScene("Lobby");
    }
    IEnumerator InversionMode()
    {
        inversion = true;
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm * 4f);
        globalLight2D.color = inversionColor;
        OnInversion?.Invoke(inversion);
        afterInversion = true;
    }
    IEnumerator DefaultMode()
    {
        inversion = false;
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm * 4f);
        globalLight2D.color = defaultColor;
        OnInversion?.Invoke(inversion);
        afterInversion = false;
    }
    public void ClearNote(int type)
    {
        Transform circle = leftCircle.transform;
        if (type == 1)
        {
            circle = rightCircle.transform;
        }
        circle.DOKill();
        circle.localScale = originCircleSize;
        circle.DOScale(originCircleSize * ClearCircleSize, ClearCircleTime).SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo);
    }
    public void LightOn(string dir)
    {
        var target = leftLight;
        var sub = rightLight;
        float original = leftLight.intensity;
        float subOriginal = rightLight.intensity;
        if (dir == "right")
        {
            original = rightLight.intensity;
            target = rightLight;
        }
        float peak = 1f;
        float total = 0.25f;
        float half = total * 0.5f;
        DOTween.Kill(target);

        Sequence seq = DOTween.Sequence().SetTarget(target);
        seq.Append(
            DOTween.To(() => target.intensity, v => target.intensity = v, peak, half)
                .SetEase(Ease.OutQuad)
        );
        seq.Append(
            DOTween.To(() => target.intensity, v => target.intensity = v, original, half)
                .SetEase(Ease.InQuad)
        );
        if (dir == "all")
        {
            Sequence seqSub = DOTween.Sequence().SetTarget(sub);
            seqSub.Append(
                DOTween.To(() => sub.intensity, v => sub.intensity = v, peak, half)
                    .SetEase(Ease.OutQuad)
            );
            seqSub.Append(
                DOTween.To(() => sub.intensity, v => sub.intensity = v, subOriginal, half)
                    .SetEase(Ease.InQuad)
            );
        }
    }

    public void SetLightParent(SongName songName)
    {
        switch (songName)
        {
            case SongName.HaiPhutHon:
                {
                    stageLight = Instantiate(haiPhutHonLightPrefab, lightTransform.transform);
                    leftLight = stageLight.transform.GetChild(0).GetComponent<Light2D>();
                    rightLight = stageLight.transform.GetChild(1).GetComponent<Light2D>();
                    break;
                }
        }
    }
}
