using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum NoteState
{
    Normal,
    Ready,
    Available,
    Clear,
}
public class NoteCreate : MonoBehaviour
{
    SpriteRenderer[] sr;
    [SerializeField] SpriteRenderer footHoldImage;
    [SerializeField] SpriteRenderer footHoldCircle;
    [SerializeField] CircleRingRenderer shrinkingCircle;
    [SerializeField]
    Transform footHoldTransform;

    [SerializeField]
    Sprite[] sprite;

    public bool isLeft;
    [SerializeField] float delayTime;
    [SerializeField] float duration;
    [SerializeField] float destroyTime = 0.5f;
    float bpm;
    public NoteState noteState = NoteState.Ready;
    public Notes noteData;
    public float guideCircleSpeed;
    float guideCircleSpeed2;
    float clearTime = 0.3f;
    [SerializeField] Color defaultColor;
    [SerializeField] Color leftDefaultColor;
    [SerializeField] Color rightDefaultColor;
    [SerializeField] float defaultCircleSize = 0.5f;

    void Awake()
    {
        sr = new[] { footHoldImage, footHoldCircle };
        for (int i = 0; i < sr.Length; i++)
        {
            Color c = sr[i].color;
            c.a = 0f;
            sr[i].color = c;
        }
        defaultColor = footHoldCircle.color;
    }

    // 인버전상태 돌입시 업데이트
    void UpdateInversion(bool inversion)
    {
        int spriteIndex = 0;
        Color c = footHoldCircle.color;
        if (inversion)
        {
            c = Color.white;
            spriteIndex += 1;
        }
        else
        {
            c = defaultColor;
        }
        c.a = footHoldCircle.color.a;
        footHoldCircle.color = c;
        footHoldImage.sprite = sprite[spriteIndex];
    }

    void OnEnable()
    {
        BeatEvent.OnInversion += UpdateInversion;
        if (BeatEvent.instance.inversion)
        {
            footHoldCircle.color = Color.white;
        }
        else
        {
            footHoldCircle.color = defaultColor;
        }
        for (int i = 0; i < sr.Length; i++)
        {
            Color c = sr[i].color;
            c.a = 0f;
            sr[i].color = c;
        }
        shrinkingCircle.Color = BeatEvent.instance.inversion ? Color.white : Color.black;
        shrinkingCircle.Alpha = 0f;
        shrinkingCircle.Radius = defaultCircleSize;
        noteState = NoteState.Normal;
        bpm = BeatManager.instance.notes.bpm;
        guideCircleSpeed2 = BeatManager.instance.notes.guideCircleSpeed;
        delayTime = 60f / bpm * 2;
        duration = 60f / bpm * 1;
        StartCoroutine(CreateNote());
    }

    void OnDisable()
    {
        BeatEvent.OnInversion -= UpdateInversion;
        DOTween.Kill(this);
        StopAllCoroutines();
    }

    //노트 생성 알고리즘
    IEnumerator CreateNote()
    {
        int spriteIndex = 0;
        int type = noteData.type;
        Color cc = footHoldCircle.color;
        if (BeatEvent.instance.afterInversion)
        {
            cc = Color.white;
            spriteIndex += 1;
        }
        else
        {
            cc = defaultColor;
        }
        cc.a = footHoldCircle.color.a;
        footHoldCircle.color = cc;
        footHoldImage.sprite = sprite[spriteIndex];
        yield return new WaitForSeconds(delayTime / 2);
        noteState = NoteState.Ready;
        footHoldTransform.localScale = Vector3.one * 0.4f;
        //초기화
        for (int i = 0; i < sr.Length; i++)
        {
            Color c = sr[i].color;
            c.a = 0f;
            sr[i].color = c;
        }
        DOTween.To(
            () => shrinkingCircle.Radius,
            r => shrinkingCircle.Radius = r,
            0.9f,
            duration * 3f
        ).SetEase(Ease.OutCubic).OnUpdate(() =>
        {
            if (noteState == NoteState.Available && IsPeekOfMyQueue())
            {
                Color c;
                c = isLeft ? leftDefaultColor : rightDefaultColor;
                c.a = 1f;
                shrinkingCircle.Color = c;
            }
            else
            {
                Color c;
                Color bg = BeatEvent.instance.globalLight2D.color;
                c = new Color(1f - bg.r, 1f - bg.g, 1f - bg.b);
                shrinkingCircle.Color = c;
            }
        }).OnComplete(() => shrinkingCircle.Alpha = 0f);
        DOTween.To(
            () => shrinkingCircle.Alpha,
            a => shrinkingCircle.Alpha = a,
            1f,
            duration * 2f
        ).SetEase(Ease.OutQuad);
        Sequence seq = DOTween.Sequence();
        seq.Append(footHoldTransform.DOScale(1f, duration)
            .SetEase(Ease.OutBack, 1f));
        for (int i = 0; i < sr.Length; i++)
        {
            SpriteRenderer rend = sr[i];
            seq.Join(rend
                .DOFade(1f, duration)
                .SetEase(Ease.Linear));
        }
        yield return seq.WaitForCompletion();
        yield return new WaitForSeconds(duration / 4f * 3f);
        noteState = NoteState.Available;
        yield return new WaitForSeconds(duration / 4f);
        guideCircleSpeed = noteData.guideCircleSpeed == 0 ? 1 : noteData.guideCircleSpeed;
        float guideCircleDuration = duration / guideCircleSpeed / guideCircleSpeed2;
        yield return new WaitForSeconds((duration - guideCircleDuration) / 2);
        Tween move = BeatEvent.instance.MoveGuideCircle(noteData.direction, noteData.type, guideCircleDuration);
        if (move != null)
            yield return move.WaitForCompletion();
        shrinkingCircle.Alpha = 0f;
        if (isActiveAndEnabled)
            StartCoroutine(SetCircle());
    }

    // 노트 파괴
    public IEnumerator DestroyNote()
    {
        float elapsed = 0f;
        float tempTime = destroyTime / 3;
        while (elapsed < tempTime)
        {
            elapsed += Time.deltaTime;
            for (int i = 0; i < sr.Length; i++)
            {
                Color c = sr[i].color;
                c.a = Mathf.Clamp01(1f - elapsed / tempTime);
                sr[i].color = c;
            }
            yield return null;
        }
        for (int i = 0; i < sr.Length; i++)
        {
            Color c = sr[i].color;
            c.a = 0f;
            sr[i].color = c;
        }
        FootHoldObjectFool.instance.ReturnFootHold(this.gameObject);
    }

    // 시간초과시 게임오버 알고리즘
    IEnumerator SetCircle()
    {
        yield return new WaitForSeconds(clearTime);
        //게임오버
        if (noteState == NoteState.Available)
        {
            Debug.Log("시간초과로 인한 게임오버");
            GameManager.instance.RestartGame();
            yield break;
        }
    }

    // 노트 클리어시 동글뱅이 없애버리기
    public IEnumerator SetPointCircle()
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - (elapsed / duration));
            Color c = sr[1].color;
            c.a = alpha;
            sr[1].color = c;
            yield return null;
        }
        Color c2 = sr[1].color;
        c2.a = 0f;
        sr[1].color = c2;
    }
    
    bool IsPeekOfMyQueue()
    {
        var q = (noteData.type == 0)
            ? BeatEvent.instance.leftFootHoldQueue
            : BeatEvent.instance.rightFootHoldQueue;

        return q != null && q.Count > 0 && ReferenceEquals(q.Peek(), this);
    }
}
