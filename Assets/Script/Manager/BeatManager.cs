using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatManager : MonoBehaviour
{
    public static BeatManager instance;
    public NotesData notes;
    public Queue<Notes> noteQueue;
    public SongName Playname;
    public float bpm;

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
    public void BeatStart(SongName songName,int resumeBar = 0, int resumeBeat = 0)
    {
        LinkDisable();
        notes = NotesLoader.LoadChart(songName);
        bpm = notes.bpm;
        Playname = songName;
        noteQueue = new Queue<Notes>(notes.notes);
        RestartHandleBeat(resumeBar, resumeBeat);
        LinkEnable();
    }

    void OnDestroy() => LinkDisable();

    // 비트 발생시 처리 함수
    void HandleOnBeat(int bar, int beatIndex)
    {
        if (noteQueue.Count == 0) return;
        //Debug.Log("HandleOnBeat: " + bar + " " + beatIndex);
        //Debug.Log(noteQueue.Peek().bar + " " + noteQueue.Peek().beat);
        while (noteQueue.Count > 0 && noteQueue.Peek().bar == bar && noteQueue.Peek().beat == beatIndex)
        {
            Notes note = noteQueue.Dequeue();
            BeatEvent.instance.BeatHandling(note);
        }
        
        while (noteQueue.Count > 0 &&
        (noteQueue.Peek().bar < bar ||
        (noteQueue.Peek().bar == bar && noteQueue.Peek().beat <= beatIndex)))
        {
            noteQueue.Dequeue();
        }
    }

    public void RestartHandleBeat(int bar, int beatIndex)
    {
        while (noteQueue.Count > 0 &&
        (noteQueue.Peek().bar < bar ||
        (noteQueue.Peek().bar == bar && noteQueue.Peek().beat <= beatIndex)))
        {
            Debug.Log("RestartHandleBeat: " + noteQueue.Peek().bar + " " + noteQueue.Peek().beat);
            noteQueue.Dequeue();
        }
    }

    public void LinkDisable()
    {
        Debug.Log("LinkDisable");
        AudioManager.OnBeat -= HandleOnBeat;
    }
    public void LinkEnable()
    {
        Debug.Log("LinkEnable");
        AudioManager.OnBeat += HandleOnBeat;
    }
}
