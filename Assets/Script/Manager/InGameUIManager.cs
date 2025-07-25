using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameUIManager : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject timeCount;
    [SerializeField] GameObject saveText;
    [SerializeField] GameObject saveCircle;
    [SerializeField] float saveCircleTime = 3f;
    [SerializeField] float saveCircleSize = 25f;
    Vector3 originSaveCircleSize;
    bool isPaused = false;

    void Awake()
    {
        originSaveCircleSize = saveCircle.transform.localScale;
    }
    void OnEnable()
    {
        GameManager.OnPauseRequest += TogglePause;
        GameManager.OnSaveAlarm += SaveAlarmSet;
    }
    void OnDisable()
    {
        GameManager.OnPauseRequest -= TogglePause;
        GameManager.OnSaveAlarm += SaveAlarmSet;
    }

    void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    void Pause()
    {
        GameManager.instance.gameState = GameState.pause;
        AudioManager.instance.bgmInstance.setPaused(true);
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        DOTween.PauseAll();
    }

    public void Resume()
    {
        GameObject btn = pausePanel.transform.GetChild(0).gameObject;
        btn.SetActive(true);
        GameObject optionPanel = pausePanel.transform.GetChild(1).gameObject;
        optionPanel.SetActive(false);
        StartCoroutine(StartResume());
    }
    IEnumerator StartResume()
    {
        pausePanel.SetActive(false);
        TextMeshProUGUI text = timeCount.GetComponent<TextMeshProUGUI>();
        Color c;
        Color bg = BeatEvent.instance.mainCamera.backgroundColor;
        c = new Color(1f - bg.r, 1f - bg.g, 1f - bg.b);
        text.color = c;
        text.text = "3";
        timeCount.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);
        text.text = "2";
        yield return new WaitForSecondsRealtime(1f);
        text.text = "1";
        yield return new WaitForSecondsRealtime(1f);
        timeCount.SetActive(false);
        GameManager.instance.gameState = GameState.inGame;
        AudioManager.instance.bgmInstance.setPaused(false);
        isPaused = false;
        Time.timeScale = 1f;
        DOTween.PlayAll();
    }

    public void GoMainManu()
    {
        Time.timeScale = 1f;
        AudioManager.instance.PlayMusic("event:/Lobi", 0);
        SceneManager.LoadScene("Lobby");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void SaveAlarmSet()
    {
        StartCoroutine(SaveAlarmGo());
    }

    IEnumerator SaveAlarmGo()
    {
        TextMeshProUGUI text = saveText.GetComponent<TextMeshProUGUI>();
        Color c = text.color;
        c.a = 0f;
        text.color = c;
        saveText.SetActive(true);
        float t = 0f;
        StartCoroutine(SaveCircleSet());
        while (t < 0.4f)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / 0.2f);
            Color bg = BeatEvent.instance.mainCamera.backgroundColor;
            c = new Color(1f - bg.r, 1f - bg.g, 1f - bg.b, alpha);
            text.color = c;
            yield return null;
        }
        t = 0f;
        yield return null;
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            Color bg = BeatEvent.instance.mainCamera.backgroundColor;
            c = new Color(1f - bg.r, 1f - bg.g, 1f - bg.b, 1f);
            text.color = c;
            yield return null;
        }
        t = 0f;
        yield return null;
        while (t < 0.4f)
        {
            t += Time.deltaTime;
            c.a = Mathf.Clamp01(1f - t / 0.2f);
            text.color = c;
            yield return null;
        }
        saveText.SetActive(false);
    }

    IEnumerator SaveCircleSet()
    {
        float t = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 targetScale = Vector3.one * saveCircleSize;
        float circleSize;
        saveCircle.SetActive(true);
        while (t < saveCircleTime)
        {
            t += Time.deltaTime;
            circleSize = Mathf.Clamp01(t / saveCircleTime);
            float eased = Mathf.SmoothStep(0f, 1f, circleSize);
            saveCircle.transform.localScale = Vector3.Lerp(startScale, targetScale, eased);
            yield return null;
        }

        saveCircle.SetActive(false);
    }
}
