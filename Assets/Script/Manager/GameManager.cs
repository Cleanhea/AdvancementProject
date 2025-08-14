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
    option,
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static Action OnPauseRequest;
    public static Action OnSaveAlarm;
    public static Action<int> OnRestartRequest;
    public SaveState saveState = new SaveState();
    public GameState gameState;
    public int saveOK = 0;
    public bool startGame = false;
    public int deathCount = 0;
    public bool isFirstGame = true;
    public float delayTempoOffset = 0f;
    public bool isInputEnabled = true;
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
        var snapshot = BeatManager.instance.noteQueue.ToArray();
        for(int i = 0; i < snapshot.Length; i++)
        {
            temp.remainingNotes.Add(snapshot[i]);
            if ((++copied & 127) == 0)
                yield return null;
        }
        temp.isInversion = BeatEvent.instance.inversion;
        temp.afterInversion = BeatEvent.instance.afterInversion;
        temp.globalLightColor = BeatEvent.instance.globalLight2D.color;
        //오차 보정
        yield return null;
        yield return new WaitUntil(() => saveOK >= 2);
        Debug.Log("SavePoint");
        Vector3 pl = BeatEvent.instance.currentLeftCirclePosition;
        Vector3 pr = BeatEvent.instance.currentRightCirclePosition;
        temp.leftCirclePosition = pl;
        temp.rightCirclePosition = pr;
        temp.cameraPosition = new CameraPos();
        temp.cameraPosition.x = BeatEvent.instance.cameraPoint.x;
        temp.cameraPosition.y = BeatEvent.instance.cameraPoint.y;
        temp.CameraZoom = BeatEvent.instance.cameraZoomPoint;
        OnSaveAlarm?.Invoke();
        saveState = temp;
        saveOK = 0;
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
        saveOK = 0;
        deathCount++;
        OnRestartRequest?.Invoke(deathCount);
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
        FootHoldObjectFool.instance.AllReturnFootHold();
        BeatEvent.instance.mainCamera.orthographicSize = saveState.CameraZoom;
        BeatEvent.instance.inversion = saveState.isInversion;
        BeatEvent.instance.afterInversion = saveState.afterInversion;
        BeatEvent.instance.globalLight2D.color = saveState.globalLightColor;
        BeatEvent.instance.StopAllCoroutines();
        yield return null;
        BeatManager.instance.BeatStartFromSave(playname, saveState);
        yield return null;
        AudioManager.instance.PlayMusic("event:/" + BeatManager.instance.Playname, saveState.musicTime);
    }

    public void MarkSaveOK()
    {
        saveOK++;
    }
    public void DefaultSaveData()
    {
        deathCount = 0;
        saveState.musicTime = 0;
        saveState.currentBar = 0;
        saveState.currentBeat = 0;
        saveState.cameraPosition.x = 0;
        saveState.cameraPosition.y = 0;
        saveState.CameraZoom = 15f;
        saveState.leftCirclePosition = new Vector3(-4.5f, 0, 0);
        saveState.rightCirclePosition = new Vector3(4.5f, 0, 0);
        saveState.isInversion = false;
        saveState.afterInversion = false;
        saveState.globalLightColor = Color.white;
        saveState.remainingNotes.Clear();
    }
}
