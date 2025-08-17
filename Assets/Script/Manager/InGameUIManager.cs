using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager instance;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject timeCount;
    [SerializeField] GameObject saveText;
    [SerializeField] GameObject saveCircle;
    [SerializeField] GameObject musicNameInform;
    [SerializeField] GameObject GuideButton;
    [SerializeField] TextMeshProUGUI deathText;
    [SerializeField] Image sliderFill;
    [SerializeField] Image sliderBackGround;
    [SerializeField] Slider songDistanceSlider;
    [SerializeField] float saveCircleTime = 3f;
    [SerializeField] float saveCircleSize = 25f;
    bool isPaused = false;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    void Start()
    {
        saveCircle.SetActive(false);
        saveText.SetActive(false);
    }
    void Update()
    {
        if (BeatEvent.instance == null) return;
        Color bg = BeatEvent.instance.globalLight2D.color;
        deathText.color = new Color(1f - bg.r, 1f - bg.g, 1f - bg.b);
        sliderFill.color = new Color(1f - bg.r, 1f - bg.g, 1f - bg.b);
        sliderBackGround.color = new Color(bg.r, bg.g, bg.b);
    }
    void OnEnable()
    {
        GameManager.OnPauseRequest += TogglePause;
        GameManager.OnSaveAlarm += SaveAlarmSet;
        GameManager.OnRestartRequest += DeathTextSet;
        AudioManager.OnBeat += SliderUpdate;
    }
    void OnDisable()
    {
        GameManager.OnPauseRequest -= TogglePause;
        GameManager.OnSaveAlarm -= SaveAlarmSet;
        GameManager.OnRestartRequest -= DeathTextSet;
        AudioManager.OnBeat -= SliderUpdate;
    }
    //퍼즈 토글
    void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    //퍼즈기능
    void Pause()
    {
        GameManager.instance.gameState = GameState.pause;
        AudioManager.instance.bgmInstance.setPaused(true);
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        DOTween.PauseAll();
    }
    //재시작 기능
    public void Resume()
    {
        GameObject btn = pausePanel.transform.GetChild(0).gameObject;
        btn.SetActive(true);
        GameObject optionPanel = pausePanel.transform.GetChild(1).gameObject;
        optionPanel.SetActive(false);
        StartCoroutine(StartResume());
    }

    //재시작 3초 카운트
    IEnumerator StartResume()
    {
        pausePanel.SetActive(false);
        TextMeshProUGUI text = timeCount.GetComponent<TextMeshProUGUI>();
        Color c;
        Color bg = BeatEvent.instance.globalLight2D.color;
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

    //메인메뉴로 돌아가기
    public void GoMainManu()
    {
        GameManager.instance.DefaultSaveData();
        GameManager.instance.StopAllCoroutines();
        Time.timeScale = 1f;
        GameManager.instance.gameState = GameState.lobi;
        AudioManager.instance.PlayMusic("event:/Lobi", 0);
        SceneManager.LoadScene("Lobby");
    }

    //게임 종료
    public void QuitGame()
    {
        Application.Quit();
    }

    //세이브 알람
    void SaveAlarmSet()
    {
        StartCoroutine(SaveAlarmGo());
    }

    //세이브 알람 코루틴
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
            float alpha = Mathf.Clamp01(t / 0.4f);
            Color bg = BeatEvent.instance.globalLight2D.color;
            c = new Color(1f - bg.r, 1f - bg.g, 1f - bg.b, alpha);
            text.color = c;
            yield return null;
        }
        t = 0f;
        yield return null;
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            Color bg = BeatEvent.instance.globalLight2D.color;
            c = new Color(1f - bg.r, 1f - bg.g, 1f - bg.b, 1f);
            text.color = c;
            yield return null;
        }
        t = 0f;
        yield return null;
        while (t < 0.4f)
        {
            t += Time.deltaTime;
            c.a = Mathf.Clamp01(1f - t / 0.4f);
            text.color = c;
            yield return null;
        }
        saveText.SetActive(false);
    }

    //세이브 연출 애니메이션
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
    
    // 초기 음악 이름 띄어주는 함수
    public void SetMusicInformation(SongName songName)
    {
        string name = SplitCamelCase(songName.ToString());
        TextMeshProUGUI text = musicNameInform.GetComponent<TextMeshProUGUI>();
        text.text = name;
        if (GameManager.instance.isFirstGame)
        {
            GameManager.instance.isFirstGame = false;
            StartCoroutine(FadeInOutGuideButton());
        }
        Color c = deathText.color;
        c.a = 0f;
        deathText.color = c;
        deathText.DOFade(1f, 2f);
        StartCoroutine(FadeInMusicInformation());
    }

    // 처음 게임 시작시 버튼 안내 함수
    IEnumerator FadeInOutGuideButton()
    {
        float t = 0f;
        Image[] images = GuideButton.GetComponentsInChildren<Image>(true);
        foreach (Image img in images)
        {
            Color c = img.color;
            c.a = 0f;
            img.color = c;
        }
        GuideButton.SetActive(true);
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            foreach (Image img in images)
            {
                Color c = img.color;
                c.a = Mathf.Clamp01(t / 0.5f);
                img.color = c;
            }
            yield return null;
        }
        t = 0f;
        yield return new WaitForSecondsRealtime(2f);
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            foreach (Image img in images)
            {
                Color c = img.color;
                c.a = Mathf.Clamp01(1f - t / 0.5f);
                img.color = c;
            }
            yield return null;
        }
        GuideButton.SetActive(false);
    }

    // 음악 정보 페이드 인
    IEnumerator FadeInMusicInformation()
    {
        TextMeshProUGUI text = musicNameInform.GetComponent<TextMeshProUGUI>();
        float t = 0f;
        Color c = text.color;
        c.a = 0f;
        text.color = c;
        musicNameInform.SetActive(true);
        while (t < 1f)
        {
            t += Time.deltaTime;
            c.a = Mathf.Clamp01(t / 1f);
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
    //데스카운터 업데이트
    public void DeathTextSet(int deathCount)
    {
        deathText.text = deathCount.ToString();
    }

    public void SliderUpdate(int bar, int beat)
    {
        float clearBeat = BeatManager.instance.notes.clearBeat;
        float cal = (float)bar / clearBeat;
        songDistanceSlider.value = cal;
    }
    public Tween FadeOutDeathText()
    {
        return deathText.DOFade(0f, 1f);
    }


    //--------------부가기능-----------------
    //대문자 띄어주는 함수
    public static string SplitCamelCase(string input)
    {
        return Regex.Replace(input, "(\\B[A-Z])", " $1");
    }
}
