using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public SaveState saveState = new SaveState();


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
    }
    public void SaveGame()
    {
        StartCoroutine(SavePoint());
    }
    IEnumerator SavePoint()
    {
        saveState.remainingNotes.Clear();
        saveState.currentBar = AudioManager.CurrentBar;
        saveState.currentBeat = AudioManager.CurrentBeat;
        int ms;
        AudioManager.instance.bgmInstance.getTimelinePosition(out ms);
        saveState.musicTime = ms;
        foreach (var note in BeatManager.instance.noteQueue)
        {
            saveState.remainingNotes.Add(note);
        }
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm * 4f);
        Debug.Log("SavePoint");
        //오차 보정
        Vector3 pl = BeatEvent.instance.leftGuideCircle.transform.position;
        Vector3 pr = BeatEvent.instance.rightGuideCircle.transform.position;
        pl.x = (float)Math.Round(pl.x, 1);
        pr.x = (float)Math.Round(pr.x, 1);
        pl.y = (float)Math.Round(pl.y, 1);
        pr.y = (float)Math.Round(pr.y, 1);
        saveState.leftCirclePosition = pl;
        saveState.rightCirclePosition = pr;
        saveState.cameraPosition = BeatEvent.instance.GetCameraPos();
        saveState.CameraZoom = BeatEvent.instance.GetCameraZoom();
    }

    public void RestartGame()
    {
        StartCoroutine(ResetGame());
    }
    IEnumerator ResetGame()
    {
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
        yield return null;
        BeatManager.instance.BeatStartFromSave(playname, saveState);
        yield return null;
        AudioManager.instance.PlayMusic("event:/" + BeatManager.instance.Playname, saveState.musicTime);
    }    
}
