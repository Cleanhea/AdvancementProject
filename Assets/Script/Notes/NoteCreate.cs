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
    

    void Awake()
    {
        sr = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
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
        float elapsed = 0f;
        noteState = NoteState.Ready;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float alpha = Mathf.Pow(t, 0.7f);
            for (int i = 0; i < sr.Length; i++)
            {
                Color c = sr[i].color;
                c.a = alpha;
                sr[i].color = c;
            }
            yield return null;
        }
        for (int i = 0; i < sr.Length; i++)
        {
            Color c = sr[i].color;
            c.a = 1f;
            sr[i].color = c;
        }
        noteState = NoteState.Available;
        Tween move = BeatEvent.instance.MoveGuideCircle(noteData.direction, noteData.type, duration);
        if (move != null)
            yield return move.WaitForCompletion();
        if (isActiveAndEnabled)
                StartCoroutine(SetCircle());
        yield return new WaitForSeconds(destroyDuration);
        StartCoroutine(DestroyNote());
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
        if (noteData.sevent == "save")
        {
            Debug.Log("SavePoint");
            GameManager.instance.SavePoint();
        }
        yield return new WaitForSeconds(0.1f);
        //게임오버
        if (noteState == NoteState.Available)
        {
            Debug.Log("시간초과로 인한 게임오버");
            GameManager.instance.RestartGame();
            yield break;
        }
    }
}
