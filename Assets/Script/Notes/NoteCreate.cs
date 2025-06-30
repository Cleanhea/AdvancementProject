using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class NoteCreate : MonoBehaviour
{
    SpriteRenderer[] sr;
    SpriteRenderer circleRenderer;
    GameObject guideCircle;
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
    

    void Awake()
    {
        sr = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
        circleRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        guideCircle = transform.GetChild(1).gameObject;
        for (int i = 1; i < sr.Length; i++)
        {
            Color c = sr[i].color;
            c.a = 0f;
            sr[i].color = c;
        }
        Color d = circleRenderer.color;
        d.a = 0f;
        circleRenderer.color = d;
    }
    void OnEnable()
    {
        bpm = BeatManager.instance.notes.bpm;
        delayTime = 60f / bpm * 2;
        duration = 60f / bpm * 1;
        StartCoroutine(CreateNote());
    }

    IEnumerator CreateNote()
    {
        yield return new WaitForSeconds(delayTime);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float alpha = Mathf.Pow(t, 0.7f);
            for (int i = 1; i < sr.Length; i++)
            {
                Color c = sr[i].color;
                c.a = alpha;
                sr[i].color = c;
            }
            Color d = circleRenderer.color;
            d.a = alpha * 0.5f;
            circleRenderer.color = d;
            yield return null;
        }
        for (int i = 1; i < sr.Length; i++)
        {
            Color c = sr[i].color;
            c.a = 1f;
            sr[i].color = c;
        }
        guideCircle.transform.DOLocalMove(new Vector3(0, 3f, 0), duration).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            StartCoroutine(SetCircle());
        });

        yield return new WaitForSeconds(destroyDuration);
        StartCoroutine(DestroyNote());
    }

    IEnumerator DestroyNote()
    {
        float elapsed = 0f;
        while (elapsed < destroyTime)
        {
            elapsed += Time.deltaTime;
            for (int i = 1; i < sr.Length; i++)
            {
                Color c = sr[i].color;
                c.a = Mathf.Clamp01(1f - elapsed / destroyTime);
                sr[i].color = c;
            }
            Color e = circleRenderer.color;
            e.a = Mathf.Clamp01(1f - elapsed / destroyTime);
            circleRenderer.color = e / 2;
            yield return null;
        }
        for (int i = 1; i < sr.Length; i++)
        {
            Color c = sr[i].color;
            c.a = 0f;
            sr[i].color = c;
        }
        Color d = circleRenderer.color;
        d.a = 0f;
        circleRenderer.color = d;
        Debug.Log("Destroy Note");
        FootHoldObjectFool.instance.ReturnFootHold(this.gameObject);
    }
    IEnumerator SetCircle()
    {
        yield return new WaitForSeconds(0.1f);
        guideCircle.SetActive(false);
    }
}
