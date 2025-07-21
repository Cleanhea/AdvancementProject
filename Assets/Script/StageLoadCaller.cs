using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[DefaultExecutionOrder(1000)]
public class StageLoadCaller : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(RunSceneLoad());
    }
    public void MusicStart(SongName songName)
    {
        if (BeatManager.instance.noteQueue != null)
        {
            BeatManager.instance.noteQueue.Clear();
        }
        AudioManager.instance.PlayMusic("event:/" + songName.ToString(), 0);
        BeatManager.instance.BeatStart(songName);
    }
    IEnumerator RunSceneLoad()
    {
        yield return new WaitForSeconds(0.2f);
        var song = StageLoadContext.songName;
        MusicStart(song);
    }
}
