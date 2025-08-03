using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class BeatManager : MonoBehaviour
{
    public static BeatManager instance;
    public event Action<Notes> OnNoteSpawn;
    public NotesData notes;
    public Queue<Notes> noteQueue;
    public SongName Playname;
    public float bpm;
    [SerializeField]
    bool isCheck = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    // FMOD에서 비트 읽어옴
    public void BeatStart(SongName songName)
    {
        LinkDisable();
        notes = NotesLoader.LoadChart(songName);
        bpm = notes.bpm;
        Playname = songName;
        noteQueue = new Queue<Notes>(notes.notes);
        GameManager.instance.saveState.remainingNotes = new List<Notes>(noteQueue);
        GameManager.instance.saveState.CameraZoom = 15f;
        LinkEnable();
    }
    public void BeatStartFromSave(SongName songName, SaveState s)
    {
        LinkDisable();
        notes = NotesLoader.LoadChart(songName);
        bpm = notes.bpm;
        Playname = songName;
        noteQueue = new Queue<Notes>(s.remainingNotes);
        LinkEnable();
    }
    void OnDestroy() => LinkDisable();

    // 비트 발생시 처리 함수
    void HandleOnBeat(int bar, int beatIndex)
    {
        if (noteQueue.Count == 0) return;
        if(isCheck == true)
            RestartHandleBeat(bar, beatIndex);
        while (noteQueue.Count > 0 && noteQueue.Peek().bar == bar && noteQueue.Peek().beat == beatIndex)
        {
            if(isCheck)
                Debug.Log("bar = " + bar + " beat =" +beatIndex);
            Notes note = noteQueue.Dequeue();
            OnNoteSpawn?.Invoke(note);
        }
    }
    void RestartHandleBeat(int bar, int beatIndex)
    {
        while (noteQueue.Count > 0 && noteQueue.Peek().bar < bar || 
            (noteQueue.Peek().bar == bar && noteQueue.Peek().beat < beatIndex))
        {
            noteQueue.Dequeue();
        }
    }

    public void LinkDisable()
    {
        AudioManager.OnBeat -= HandleOnBeat;
    }
    public void LinkEnable()
    {
        AudioManager.OnBeat += HandleOnBeat;
    }
}
