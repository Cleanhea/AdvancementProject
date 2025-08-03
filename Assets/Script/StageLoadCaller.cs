using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[DefaultExecutionOrder(1000)]
public class StageLoadCaller : MonoBehaviour
{
    [SerializeField] bool testMode = false;
    [SerializeField] int startTime = 0;
    void Start()
    {
        if (testMode)
        {
            StageLoadContext.songName = BeatManager.instance.Playname;
        }
        StartCoroutine(RunSceneLoad());
    }
    public void MusicStart(SongName songName)
    {
        if (BeatManager.instance.noteQueue != null)
        {
            BeatManager.instance.noteQueue.Clear();
        }
        AudioManager.instance.PlayMusic("event:/" + songName.ToString(), startTime);
        BeatManager.instance.BeatStart(songName);
    }
    IEnumerator RunSceneLoad()
    {
        var song = StageLoadContext.songName;
        BeatEvent.instance.BeatStart();
        if (!testMode)
        {
            InGameUIManager.instance.SetMusicInformation(song);
            AudioManager.instance.StartCoroutine(AudioManager.instance.VolumeFadeOut());
            yield return new WaitForSeconds(1.5f);
        }
        BeatEvent.instance.SetLightParent(song);
        yield return new WaitForSeconds(1.5f);
        AudioManager.instance.SetMusicVolume(AudioManager.instance.MusicVolume);
        MusicStart(song);
        GameManager.instance.gameState = GameState.inGame;
    }
}
