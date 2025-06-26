using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatManager : MonoBehaviour
{
    public static BeatManager instance;
    public List<Notes> notes;
    Queue<Notes> noteQueue;

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
        AudioManager.OnBeat += HandleOnBeat;
        noteQueue = new Queue<Notes>(notes);
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
