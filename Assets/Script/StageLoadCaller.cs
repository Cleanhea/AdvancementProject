using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[DefaultExecutionOrder(1000)]
public class StageLoadCaller : MonoBehaviour
{
    [SerializeField]
    bool testMode = false;
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
        AudioManager.instance.PlayMusic("event:/" + songName.ToString(), 0);
        BeatManager.instance.BeatStart(songName);
    }
    IEnumerator RunSceneLoad()
    {
        var song = StageLoadContext.songName;
        InGameUIManager.instance.SetMusicInformation(song);
        float t = 0f;
        while (t < 2f)
        {
            t += Time.unscaledDeltaTime;
            float volume = Mathf.Lerp(AudioManager.instance.MusicVolume, 0f, t / 2f);
            AudioManager.instance.SetMusicVolumeInGame(volume);
            yield return null;
        }
        BeatEvent.instance.BeatStart();
        yield return new WaitForSeconds(1.5f);
        AudioManager.instance.SetMusicVolume(AudioManager.instance.MusicVolume);
        MusicStart(song);
        GameManager.instance.gameState = GameState.inGame;
    }
}
