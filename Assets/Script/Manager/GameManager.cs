using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum GameState
{
    lobi,
    inGame,
    pause,
    option
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static Action OnPauseRequest;
    public static Action OnSaveAlarm;
    public SaveState saveState = new SaveState();
    public GameState gameState;
    public bool saveOK = false;
    public bool startGame = false;
    public int deathCount = 0;
    Coroutine saveCoroutine;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        gameState = GameState.lobi;
    }
    public void SaveGame()
    {
        if (saveCoroutine != null)
            StopCoroutine(saveCoroutine);
        saveCoroutine = StartCoroutine(SavePoint());
    }
    IEnumerator SavePoint()
    {
        SaveState temp = new SaveState();
        //saveState.remainingNotes.Clear();
        temp.currentBar = AudioManager.CurrentBar;
        temp.currentBeat = AudioManager.CurrentBeat;
        int ms;
        AudioManager.instance.bgmInstance.getTimelinePosition(out ms);
        temp.musicTime = ms - 100;
        int copied = 0;
        foreach (var note in BeatManager.instance.noteQueue)
        {
            temp.remainingNotes.Add(note);
            if ((++copied & 127) == 0)
                yield return null;
        }
        temp.isInversion = BeatEvent.instance.inversion;
        temp.afterInversion = BeatEvent.instance.afterInversion;
        temp.globalLightColor = BeatEvent.instance.globalLight2D.color;
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm * 4f);
        Debug.Log("SavePoint");
        //오차 보정
        Vector3 pl = BeatEvent.instance.leftGuideCircle.transform.position;
        Vector3 pr = BeatEvent.instance.rightGuideCircle.transform.position;
        pl.x = (float)Math.Round(pl.x, 1);
        pr.x = (float)Math.Round(pr.x, 1);
        pl.y = (float)Math.Round(pl.y, 1);
        pr.y = (float)Math.Round(pr.y, 1);
        temp.leftCirclePosition = pl;
        temp.rightCirclePosition = pr;
        temp.cameraPosition = BeatEvent.instance.GetCameraPos();
        temp.CameraZoom = BeatEvent.instance.GetCameraZoom();
        yield return new WaitUntil(() => saveOK);
        OnSaveAlarm?.Invoke();
        saveState = temp;
        saveOK = false;
    }

    public void RestartGame()
    {
        if (saveCoroutine != null)
        {
            StopCoroutine(saveCoroutine);
            saveCoroutine = null;
        }
        StartCoroutine(ResetGame());
    }
    IEnumerator ResetGame()
    {
        deathCount++;
        BeatManager.instance.LinkDisable();
        SongName playname = BeatManager.instance.Playname;
        BeatEvent.instance.SetCameraPos(new Vector3(saveState.cameraPosition.x, saveState.cameraPosition.y, -10));
        DOTween.KillAll(false);
        //오브젝트 풀 리셋
        FootHoldObjectFool.instance.ResetQueue();
        AudioManager.instance.StopMusic();
        BeatEvent.instance.SetPoint(saveState.leftCirclePosition, saveState.rightCirclePosition);
        BeatEvent.instance.leftFootHoldQueue.Clear();
        BeatEvent.instance.rightFootHoldQueue.Clear();
        BeatEvent.instance.leftCirclePositionQueue.Clear();
        BeatEvent.instance.rightCirclePositionQueue.Clear();
        BeatEvent.instance.mainCamera.orthographicSize = saveState.CameraZoom;
        BeatEvent.instance.inversion = saveState.isInversion;
        BeatEvent.instance.afterInversion = saveState.afterInversion;
        BeatEvent.instance.globalLight2D.color = saveState.globalLightColor;
        yield return null;
        BeatManager.instance.BeatStartFromSave(playname, saveState);
        yield return null;
        AudioManager.instance.PlayMusic("event:/" + BeatManager.instance.Playname, saveState.musicTime);
    }

    public void MarkSaveOK()
    {
        saveOK = true;
    }
    public void DefaultSaveData()
    {
        saveState.musicTime = 0;
        saveState.currentBar = 0;
        saveState.currentBeat = 0;
        saveState.cameraPosition.x = 0;
        saveState.cameraPosition.y = 0;
        saveState.CameraZoom = 12;
        saveState.leftCirclePosition = new Vector3(-5, 0, 0);
        saveState.rightCirclePosition = new Vector3(5, 0, 0);
        saveState.isInversion = false;
        saveState.afterInversion = false;
        saveState.globalLightColor = Color.white;
        saveState.remainingNotes.Clear();
    }
}
