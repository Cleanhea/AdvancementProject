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
    SpriteRenderer footHoldImage;
    [SerializeField]
    Sprite[] sprite;
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

    void Awake()
    {
        sr = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
        footHoldImage = transform.GetChild(0).GetComponent<SpriteRenderer>();
        for (int i = 0; i < sr.Length; i++)
        {
            Color c = sr[i].color;
            c.a = 0f;
            sr[i].color = c;
        }
    }
    void OnEnable()
    {
        for (int i = 0; i < sr.Length; i++)
        {
            Color c = sr[i].color;
            c.a = 0f;
            sr[i].color = c;
        }
        noteState = NoteState.Normal;
        bpm = BeatManager.instance.notes.bpm;
        delayTime = 60f / bpm * 2;
        duration = 60f / bpm * 1;
        StartCoroutine(CreateNote());
    }

    IEnumerator CreateNote()
    {
        yield return new WaitForSeconds(delayTime);
        if (noteData.image == "Double")
        {
            footHoldImage.sprite = sprite[1];
        }
        else if (noteData.image == "Triple")
        {
            footHoldImage.sprite = sprite[2];
        }
        else
        {
            footHoldImage.sprite = sprite[0];
        }
        noteState = NoteState.Ready;
        transform.localScale = Vector3.one * 0.4f;
        for (int i = 0; i < sr.Length; i++)
        {
            Color c = sr[i].color;
            c.a = 0f;
            sr[i].color = c;
        }
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(1f, duration)
            .SetEase(Ease.OutBack, 1f));
        for (int i = 0; i < sr.Length; i++)
        {
            SpriteRenderer rend = sr[i];
            seq.Join(rend
                .DOFade(1f, duration)
                .SetEase(Ease.Linear));
        }
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
        yield return new WaitForSeconds((duration - guideCircleDuration) / 2);
        if (noteData.sevent == "quick")
        {
            StartCoroutine(DestroyNote());
        }
        else
        {
            yield return new WaitForSeconds(destroyDuration);
            StartCoroutine(DestroyNote());
        }
    }

    IEnumerator DestroyNote()
    {
        float elapsed = 0f;
        while (elapsed < destroyTime)
        {
            elapsed += Time.deltaTime;
            for (int i = 0; i < sr.Length-1; i++)
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
