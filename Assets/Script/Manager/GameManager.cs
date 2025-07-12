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
    public void SavePoint(int bar, int beat)
    {
        Debug.Log("SavePoint");
        saveState.currentBar = bar;
        saveState.currentBeat = beat;
        saveState.leftCirclePosition = BeatEvent.instance.leftPoint;
        saveState.rightCirclePosition = BeatEvent.instance.rightPoint;
        saveState.cameraPosition = BeatEvent.instance.GetCameraPos();
        saveState.leftFootHoldQueue.Clear();
        saveState.rightFootHoldQueue.Clear();
        foreach (var item in BeatEvent.instance.leftFootHoldQueue)
        {
            saveState.leftFootHoldQueue.Enqueue(item);
        }
        foreach (var item in BeatEvent.instance.rightFootHoldQueue)
        {
            saveState.rightFootHoldQueue.Enqueue(item);
        }
        int ms;
        AudioManager.instance.bgmInstance.getTimelinePosition(out ms);
        saveState.musicTime = ms-1000;
    }

    public void RestartGame()
    {
        SongName playname = BeatManager.instance.Playname;
        BeatEvent.instance.SetCameraPos(new Vector3(saveState.cameraPosition.x,saveState.cameraPosition.y,-10));
        DOTween.KillAll(false);
        //오브젝트 풀 리셋
        FootHoldObjectFool.instance.ResetQueue();
        AudioManager.instance.StopMusic();
        BeatManager.instance.BeatStart(playname);
        BeatManager.instance.RestartHandleBeat(saveState.currentBar, saveState.currentBeat);
        BeatEvent.instance.SetPoint(saveState.leftCirclePosition, saveState.rightCirclePosition);
        BeatEvent.instance.leftFootHoldQueue = new Queue<NoteCreate>(saveState.leftFootHoldQueue);
        BeatEvent.instance.rightFootHoldQueue = new Queue<NoteCreate>(saveState.rightFootHoldQueue);
        AudioManager.instance.PlayMusic("event:/" + BeatManager.instance.Playname, saveState.musicTime);
    }    
}
