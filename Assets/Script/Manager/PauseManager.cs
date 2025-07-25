using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField]
    GameObject pausePanel;
    [SerializeField]
    GameObject timeCount;
    bool isPaused = false;

    void OnEnable()
    {
        GameManager.OnPauseRequest += TogglePause;
    }
    void OnDisable()
    {
        GameManager.OnPauseRequest -= TogglePause;
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
}
