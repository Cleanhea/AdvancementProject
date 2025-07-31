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
    Color defaultColor;
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

    IEnumerator CreateNote()
    {
        int spriteIndex = 0;
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
        ).OnComplete(() => shrinkingCircle.Alpha = 0f);
        DOTween.To(
            () => shrinkingCircle.Alpha,
            a => shrinkingCircle.Alpha = a,
            1f,
            duration * 2f
        ).SetEase(Ease.OutQuad).OnUpdate(() =>
        {
            Color c;
            Color bg = BeatEvent.instance.globalLight2D.color;
            c = new Color(1f - bg.r, 1f - bg.g, 1f - bg.b);
            shrinkingCircle.Color = c;
        });
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
        yield return new WaitForSeconds(delayTime / 2);
        noteState = NoteState.Available;
        guideCircleSpeed = noteData.guideCircleSpeed == 0 ? 1 : noteData.guideCircleSpeed;
        float guideCircleDuration = duration / guideCircleSpeed;
        yield return new WaitForSeconds((duration - guideCircleDuration) / 2);
        Tween move = BeatEvent.instance.MoveGuideCircle(noteData.direction, noteData.type, guideCircleDuration);
        if (move != null)
            yield return move.WaitForCompletion();
        shrinkingCircle.Alpha = 0f;
        if (isActiveAndEnabled)
            StartCoroutine(SetCircle());
        StartCoroutine(FastDestroyNote());
        yield return new WaitForSeconds((duration - guideCircleDuration) / 2);
    }
    IEnumerator FastDestroyNote()
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
    IEnumerator DestroyNote()
    {
        float elapsed = 0f;
        while (elapsed < destroyTime)
        {
            elapsed += Time.deltaTime;
            for (int i = 0; i < sr.Length; i++)
            {
                Color c = sr[i].color;
                c.a = Mathf.Clamp01(1f - elapsed / destroyTime);
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

    IEnumerator SetCircle()
    {
        yield return new WaitForSeconds(0.1f);
        //게임오버
        if (noteState == NoteState.Available)
        {
            Debug.Log("시간초과로 인한 게임오버");
            GameManager.instance.RestartGame();
            yield break;
        }
    }
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
}
