using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatManager : MonoBehaviour
{
    public static BeatManager instance;
    public NotesData notes;
    Queue<Notes> noteQueue;
    string Playname;
    float bpm;

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
    public void BeatStart()
    {
        notes = NotesLoader.LoadChart(SongName.SoHappy);
        bpm = notes.bpm;
        Playname = notes.name;
        AudioManager.OnBeat += HandleOnBeat;
        noteQueue = new Queue<Notes>(notes.notes);
    }

    void OnDestroy() => AudioManager.OnBeat -= HandleOnBeat;

    // 비트 발생시 처리 함수
    void HandleOnBeat(int bar, int beatIndex)
    {
        while (noteQueue.Count > 0 && noteQueue.Peek().bar == bar && noteQueue.Peek().beat == beatIndex)
        {
            Notes note = noteQueue.Dequeue();
            BeatEvent.instance.BeatHandling(note);
        }
    }
}
