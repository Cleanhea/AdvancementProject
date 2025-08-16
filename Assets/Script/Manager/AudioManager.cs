using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using PimDeWitte.UnityMainThreadDispatcher;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    // 음악 관련
    string MUSIC_VCA_PATH = "vca:/Music";
    VCA musicVCA;
    float musicVol = 1f;
    public float MusicVolume => musicVol;

    // 효과음 관련
    string SFX_VCA_PATH = "vca:/SFX";
    VCA sfxVCA;
    float sfxVol = 1f;
    public float SFXVolume => sfxVol;
    public EventReference uiSet;
    public EventReference uiClick;
    public EventReference uiCancel;
    public EventReference goStage;
    public EventReference logo;
    private FMOD.Studio.EVENT_CALLBACK beatCallback;
    public EventInstance bgmInstance;
    string currentBGMPath;
    public static event Action<int, int> OnBeat; // bar, beat
    public static int CurrentBar { get; private set; }
    public static int CurrentBeat { get; private set; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            musicVCA = RuntimeManager.GetVCA(MUSIC_VCA_PATH);
            sfxVCA = RuntimeManager.GetVCA(SFX_VCA_PATH);

            musicVol = PlayerPrefs.GetFloat("MusicVol", 1f);
            sfxVol = PlayerPrefs.GetFloat("SFXVol", 1f);
            ApplyMusicVolume();
            ApplySFXVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region ------------------Music-----------------
    // 재생
    public void PlayMusic(string eventPath, int startTime)
    {
        if (currentBGMPath == eventPath && bgmInstance.isValid())
        {
            bgmInstance.setTimelinePosition(startTime);
            return;
        }
        StopMusic();
        bgmInstance = RuntimeManager.CreateInstance(eventPath);
        beatCallback = OnTimelineBeat;
        bgmInstance.setTimelinePosition(startTime);
        bgmInstance.setCallback(beatCallback, EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
        bgmInstance.start();
        currentBGMPath = eventPath;
    }

    FMOD.RESULT OnTimelineBeat(EVENT_CALLBACK_TYPE type, IntPtr eventInstPtr, IntPtr paramPtr)
    {
        if (type == EVENT_CALLBACK_TYPE.TIMELINE_BEAT)
        {
            if (!Application.isPlaying)
                return FMOD.RESULT.OK;
            if (!PimDeWitte.UnityMainThreadDispatcher.UnityMainThreadDispatcher.Exists())
            {
                return FMOD.RESULT.OK;
            }
            var beat = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)
                System.Runtime.InteropServices.Marshal.PtrToStructure(paramPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));

            int bar = beat.bar;
            int beatIndex = beat.beat;
            int positionMs = beat.position;
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                CurrentBar = bar;
                CurrentBeat = beatIndex;
                OnBeat?.Invoke(bar, beatIndex);
            });
        }
        return FMOD.RESULT.OK;
    }

    // 중지
    public void StopMusic()
    {
        if (bgmInstance.isValid())
        {
            bgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            bgmInstance.release();
        }
        currentBGMPath = null;
    }

    public void SetMusicVolume(float v)
    {
        musicVol = Mathf.Clamp01(v);
        ApplyMusicVolume();
        PlayerPrefs.SetFloat("MusicVol", musicVol);
        PlayerPrefs.Save();
    }

    public void SetMusicVolumeInGame(float v)
    {
        musicVCA.setVolume(v);
    }
    void ApplyMusicVolume()
    {
        musicVCA.setVolume(musicVol);
    }

    public IEnumerator VolumeFadeOut()
    {
        float t = 0f;
        while (t < 2f)
        {
            t += Time.unscaledDeltaTime;
            float volume = Mathf.Lerp(MusicVolume, 0f, t / 2f);
            SetMusicVolumeInGame(volume);
            yield return null;
        }
    }
    #endregion

    #region ------------------SFX-----------------
    public void PlaySFX(EventReference sfx)
    {
        if (sfx.IsNull) return;
        RuntimeManager.PlayOneShot(sfx);
    }

    public void SetSFXVolume(float v)
    {
        sfxVol = Mathf.Clamp01(v);
        ApplySFXVolume();
        PlayerPrefs.SetFloat("SFXVol", sfxVol);
        PlayerPrefs.Save();
    }

    void ApplySFXVolume()
    {
        sfxVCA.setVolume(sfxVol);
    }
    #endregion
}
