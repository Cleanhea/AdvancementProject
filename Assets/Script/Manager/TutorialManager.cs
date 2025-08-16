using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] GameObject tutorialText;
    void OnEnable()
    {
        BeatEvent.OnTutorial += TutorialStart;
    }
    void OnDisable()
    {
        BeatEvent.OnTutorial -= TutorialStart;
    }
    void TutorialStart(Notes notes)
    {
        if (notes.bar == 1 && notes.beat == 3)
        {
            StartCoroutine(FadeInOutInformation("Double Circle에 오신것을 환영합니다."));
        }
        else if (notes.bar == 3 && notes.beat == 3)
        {
            StartCoroutine(FadeInOutInformation("함께 조작을 배워보겠습니다."));
        }
        else if (notes.bar == 5 && notes.beat == 3)
        {
            StartCoroutine(FadeInOutInformation("빨간 원은 W,A,S,D로 움직일 수 있습니다."));
        }
        else if (notes.bar == 6 && notes.beat == 1)
        {
            StartCoroutine(TutorialControl1(notes));
        }
        else if (notes.bar == 9 && notes.beat == 1)
        {
            StartCoroutine(TutorialControl2(notes));
        }
        else if (notes.bar == 12 && notes.beat == 3)
        {
            StartCoroutine(FadeInOutInformation("다음은 파란원을 움직여 보겠습니다."));
        }
        else if (notes.bar == 14 && notes.beat == 3)
        {
            StartCoroutine(FadeInOutInformation("파란원은 I,J,K,L키로 움직일 수 있습니다."));
        }
        else if (notes.bar == 15 && notes.beat == 1)
        {
            StartCoroutine(TutorialControl3(notes));
        }
        else if (notes.bar == 18 && notes.beat == 3)
        {
            StartCoroutine(FadeInOutInformation("다음은 동시에 가보겠습니다."));
        }
        else if (notes.bar == 20 && notes.beat == 3)
        {
            StartCoroutine(FadeInOutInformation("준비되셨나요?"));
        }
        else if (notes.bar == 21 && notes.beat == 1)
        {
            StartCoroutine(TutorialControl4(notes));
        }
        else if (notes.bar == 24 && notes.beat == 1)
        {
            StartCoroutine(FadeInOutInformation("튜토리얼은 이걸로 끝입니다."));
        }
        else if (notes.bar == 26 && notes.beat == 1)
        {
            StartCoroutine(FadeInOutInformation("이제 머리가 아파오는 Double Circle을 즐겨보세요!"));
        }
        else if (notes.bar == 28 && notes.beat == 1)
        {
            StartCoroutine(FadeInOutInformation("자 그럼 시작해볼까요?"));
        }
    }
    IEnumerator TutorialControl1(Notes notes)
    {
        GameManager.instance.isInputEnabled = false;
        TextMeshProUGUI text = tutorialText.GetComponent<TextMeshProUGUI>();
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm * 4f);
        yield return new WaitUntil(() =>
        BeatEvent.instance.leftFootHoldQueue.Count > 0 &&
        BeatEvent.instance.leftFootHoldQueue.Peek().noteState == NoteState.Available
        );
        AudioManager.instance.bgmInstance.setPaused(true);
        Time.timeScale = 0f;
        yield return StartCoroutine(FadeInUnscaledTime("바깥원이 색이 바뀌었을때 원을 이동시킬 수 있습니다."));
        yield return new WaitForSecondsRealtime(1f);
        yield return StartCoroutine(FadeOutUnscaledTime());
        AudioManager.instance.bgmInstance.setPaused(false);
        Time.timeScale = 1f;
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm);
        AudioManager.instance.bgmInstance.setPaused(true);
        Time.timeScale = 0f;
        yield return StartCoroutine(FadeInUnscaledTime("지금 원이 겹쳐졌을때 D키를 눌러 빨간원을 움직여 보세요."));
        GameManager.instance.isInputEnabled = true;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.D));
        AudioManager.instance.bgmInstance.setPaused(false);
        Time.timeScale = 1f;
        text.text = "잘하셨습니다!";
    }
    IEnumerator TutorialControl2(Notes notes)
    {
        GameManager.instance.isInputEnabled = false;
        TextMeshProUGUI text = tutorialText.GetComponent<TextMeshProUGUI>();
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm * 4f);
        yield return new WaitUntil(() =>
        BeatEvent.instance.leftFootHoldQueue.Count > 0 &&
        BeatEvent.instance.leftFootHoldQueue.Peek().noteState == NoteState.Available
        );
        AudioManager.instance.bgmInstance.setPaused(true);
        Time.timeScale = 0f;
        yield return StartCoroutine(FadeInUnscaledTime("다음은 W,A,S로 빨간원을 움직입시다."));
        yield return new WaitForSecondsRealtime(1f);
        yield return StartCoroutine(FadeOutUnscaledTime());
        AudioManager.instance.bgmInstance.setPaused(false);
        Time.timeScale = 1f;
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm);
        AudioManager.instance.bgmInstance.setPaused(true);
        Time.timeScale = 0f;
        Color c = text.color;
        c.a = 1f;
        text.color = c;
        GameManager.instance.isInputEnabled = true;
        text.text = "지금 W키를 눌러보세요.";
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.W));
        GameManager.instance.isInputEnabled = false;
        AudioManager.instance.bgmInstance.setPaused(false);
        Time.timeScale = 1f;
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm);
        AudioManager.instance.bgmInstance.setPaused(true);
        GameManager.instance.isInputEnabled = true;
        Time.timeScale = 0f;
        text.text = "이젠 A키!";

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.A));
        AudioManager.instance.bgmInstance.setPaused(false);
        GameManager.instance.isInputEnabled = false;
        Time.timeScale = 1f;

        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm);
        AudioManager.instance.bgmInstance.setPaused(true);
        GameManager.instance.isInputEnabled = true;
        Time.timeScale = 0f;
        text.text = "마지막으로 S키!";

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.S));
        AudioManager.instance.bgmInstance.setPaused(false);
        GameManager.instance.isInputEnabled = false;
        Time.timeScale = 1f;
        text.text = "GOOD!";
    }
    IEnumerator TutorialControl3(Notes notes)
    {
        GameManager.instance.isInputEnabled = false;
        TextMeshProUGUI text = tutorialText.GetComponent<TextMeshProUGUI>();
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm * 4f);
        yield return new WaitUntil(() =>
        BeatEvent.instance.rightFootHoldQueue.Count > 0 &&
        BeatEvent.instance.rightFootHoldQueue.Peek().noteState == NoteState.Available
        );
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm);
        AudioManager.instance.bgmInstance.setPaused(true);
        Time.timeScale = 0f;
        Color c = text.color;
        c.a = 1f;
        text.color = c;
        text.text = "지금 L키!";
        GameManager.instance.isInputEnabled = true;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.L));
        AudioManager.instance.bgmInstance.setPaused(false);
        Time.timeScale = 1f;
        GameManager.instance.isInputEnabled = false;
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm);
        AudioManager.instance.bgmInstance.setPaused(true);
        Time.timeScale = 0f;
        text.text = "다음 I키!";
        GameManager.instance.isInputEnabled = true;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.I));
        AudioManager.instance.bgmInstance.setPaused(false);
        Time.timeScale = 1f;
        GameManager.instance.isInputEnabled = false;
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm);
        AudioManager.instance.bgmInstance.setPaused(true);
        Time.timeScale = 0f;
        text.text = "계속 J키!";
        GameManager.instance.isInputEnabled = true;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.J));
        AudioManager.instance.bgmInstance.setPaused(false);
        Time.timeScale = 1f;
        GameManager.instance.isInputEnabled = false;
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm);
        AudioManager.instance.bgmInstance.setPaused(true);
        Time.timeScale = 0f;
        text.text = "마지막으로 K키!";
        GameManager.instance.isInputEnabled = true;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.K));
        AudioManager.instance.bgmInstance.setPaused(false);
        Time.timeScale = 1f;
        GameManager.instance.isInputEnabled = false;
        text.text = "GOOD!";
    }
    IEnumerator TutorialControl4(Notes notes)
    {
        GameManager.instance.isInputEnabled = false;
        TextMeshProUGUI text = tutorialText.GetComponent<TextMeshProUGUI>();
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm * 4f);
        yield return new WaitUntil(() =>
        BeatEvent.instance.rightFootHoldQueue.Count > 0 &&
        BeatEvent.instance.rightFootHoldQueue.Peek().noteState == NoteState.Available
        );
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm);
        AudioManager.instance.bgmInstance.setPaused(true);
        Time.timeScale = 0f;
        Color c = text.color;
        c.a = 1f;
        text.color = c;
        text.text = "하나!";
        GameManager.instance.isInputEnabled = true;
        yield return StartCoroutine(WaitForTwoKey(KeyCode.A,KeyCode.L));
        AudioManager.instance.bgmInstance.setPaused(false);
        Time.timeScale = 1f;
        GameManager.instance.isInputEnabled = false;
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm);
        AudioManager.instance.bgmInstance.setPaused(true);
        Time.timeScale = 0f;
        text.text = "둘!";
        GameManager.instance.isInputEnabled = true;
        yield return StartCoroutine(WaitForTwoKey(KeyCode.W,KeyCode.I));
        AudioManager.instance.bgmInstance.setPaused(false);
        Time.timeScale = 1f;
        GameManager.instance.isInputEnabled = false;
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm);
        AudioManager.instance.bgmInstance.setPaused(true);
        Time.timeScale = 0f;
        text.text = "셋!";
        GameManager.instance.isInputEnabled = true;
        yield return StartCoroutine(WaitForTwoKey(KeyCode.D,KeyCode.J));
        AudioManager.instance.bgmInstance.setPaused(false);
        Time.timeScale = 1f;
        GameManager.instance.isInputEnabled = false;
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm);
        AudioManager.instance.bgmInstance.setPaused(true);
        Time.timeScale = 0f;
        text.text = "넷!";
        GameManager.instance.isInputEnabled = true;
        yield return StartCoroutine(WaitForTwoKey(KeyCode.S,KeyCode.K));
        AudioManager.instance.bgmInstance.setPaused(false);
        Time.timeScale = 1f;
        GameManager.instance.isInputEnabled = false;
        text.text = "GOOD!";
    }
    IEnumerator FadeInOutInformation(string dis)
    {
        TextMeshProUGUI text = tutorialText.GetComponent<TextMeshProUGUI>();
        text.text = dis;
        float t = 0f;
        Color c = text.color;
        c.a = 0f;
        text.color = c;
        tutorialText.SetActive(true);
        while (t < 1f)
        {
            t += Time.deltaTime;
            c.a = Mathf.Clamp01(t / 0.5f);
            text.color = c;
            yield return null;
        }
        t = 0f;
        yield return new WaitForSecondsRealtime(2f);
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            c.a = Mathf.Clamp01(1f - t / 0.5f);
            text.color = c;
            yield return null;
        }
    }
    IEnumerator FadeInUnscaledTime(string dis)
    {
        TextMeshProUGUI text = tutorialText.GetComponent<TextMeshProUGUI>();
        text.text = dis;
        float t = 0f;
        Color c = text.color;
        c.a = 0f;
        text.color = c;
        tutorialText.SetActive(true);
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Clamp01(t / 0.5f);
            text.color = c;
            yield return null;
        }
        t = 0f;
    }
    IEnumerator FadeOutUnscaledTime()
    {
        TextMeshProUGUI text = tutorialText.GetComponent<TextMeshProUGUI>();
        float t = 0f;
        Color c = text.color;
        while (t < 0.5f)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Clamp01(1f - t / 0.5f);
            text.color = c;
            yield return null;
        }
    }
    IEnumerator WaitForTwoKey(params KeyCode[] keys)
    {
        int seen = 0;
        while (seen < 2)
        {
            foreach (var k in keys)
                if (Input.GetKeyDown(k)) seen++;
            yield return null;
        }
    }
}
