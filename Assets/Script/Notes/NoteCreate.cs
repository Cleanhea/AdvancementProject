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
    [SerializeField]
    SpriteRenderer footHoldImage;
    [SerializeField]
    SpriteRenderer footHoldCircle;
    [SerializeField]
    SpriteRenderer shirkingCircle;
    [SerializeField]
    Transform footHoldTransform;

    [SerializeField]
    Sprite[] sprite;
    [SerializeField]
    Sprite[] circleSprite;

    public bool isLeft;
    [SerializeField]
    float delayTime;
    [SerializeField]
    float duration;
    [SerializeField]
    float destroyDuration = 1f;
    [SerializeField]
    float destroyTime = 0.5f;
    float bpm;
    public NoteState noteState = NoteState.Ready;
    public Notes noteData;
    public float guideCircleSpeed;
    Color defaultColor;
    [SerializeField]
    float defaultCircleSize = 0.5f;

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
    void OnEnable()
    {
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
        shirkingCircle.transform.localScale = Vector3.one * defaultCircleSize;
        Color sc = shirkingCircle.color;
        sc.a = 0f;
        shirkingCircle.color = sc;
        noteState = NoteState.Normal;
        bpm = BeatManager.instance.notes.bpm;
        delayTime = 60f / bpm * 2;
        duration = 60f / bpm * 1;
        StartCoroutine(CreateNote());
    }

    IEnumerator CreateNote()
    {
        //---스프라이트 설정 결정---
        int spriteIndex = 0;
        if (BeatEvent.instance.inversion)
        {
            spriteIndex += 3;
            shirkingCircle.sprite = circleSprite[1];
        }
        else
        {
            shirkingCircle.sprite = circleSprite[0];
        }
        yield return new WaitForSeconds(delayTime);
        if (noteData.image == "Double")
        {
            spriteIndex += 1;
        }
        else if (noteData.image == "Triple")
        {
            spriteIndex += 2;
        }
        footHoldImage.sprite = sprite[spriteIndex];
        noteState = NoteState.Ready;
        footHoldTransform.localScale = Vector3.one * 0.4f;
        //초기화
        for (int i = 0; i < sr.Length; i++)
        {
            Color c = sr[i].color;
            c.a = 0f;
            sr[i].color = c;
        }
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
        shirkingCircle.transform.DOScale(0.01f, duration * 3).OnComplete(() =>
        {
            Color c = shirkingCircle.color;
            c.a = 0f;
            shirkingCircle.color = c;
        });
        shirkingCircle.DOFade(1f, duration * 2).SetEase(Ease.OutQuad);
        yield return seq.WaitForCompletion();
        noteState = NoteState.Available;
        guideCircleSpeed = noteData.guideCircleSpeed == 0 ? 1 : noteData.guideCircleSpeed;
        float guideCircleDuration = duration / guideCircleSpeed;
        yield return new WaitForSeconds((duration - guideCircleDuration) / 2);
        Tween move = BeatEvent.instance.MoveGuideCircle(noteData.direction, noteData.type, guideCircleDuration);
        if (move != null)
            yield return move.WaitForCompletion();
        if (isActiveAndEnabled)
            StartCoroutine(SetCircle());
        if (noteData.sevent == "quick")
        {
            StartCoroutine(FastDestroyNote());
        }
        yield return new WaitForSeconds((duration - guideCircleDuration) / 2);
        if(noteData.sevent!="quick")
        {
            yield return new WaitForSeconds(destroyDuration);
            StartCoroutine(DestroyNote());
        }
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
