using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using PimDeWitte.UnityMainThreadDispatcher;
using UnityEngine.SceneManagement;

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

    //지정 사운드들
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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Lobby")
        {
            PlayMusic("event:/Lobi", 0);
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

    // 현재 마디 콜백 함수
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

    // 볼륨 설정
    public void SetMusicVolume(float v)
    {
        musicVol = Mathf.Clamp01(v);
        ApplyMusicVolume();
        PlayerPrefs.SetFloat("MusicVol", musicVol);
        PlayerPrefs.Save();
    }

    // 인게임 내에서 볼륨 설정
    public void SetMusicVolumeInGame(float v)
    {
        if (!EnsureVCA(ref musicVCA, MUSIC_VCA_PATH)) return;
        musicVCA.setVolume(v);
    }
    void ApplyMusicVolume()
    {
        if (!EnsureVCA(ref musicVCA, MUSIC_VCA_PATH)) return;
        musicVCA.setVolume(musicVol);
    }

    // 게임 끝났을때 볼륨 페이드 아웃
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
    // 효과음 재생
    public void PlaySFX(EventReference sfx)
    {
        if (sfx.IsNull) return;
        RuntimeManager.PlayOneShot(sfx);
    }

    // 효과음 볼륨 설정
    public void SetSFXVolume(float v)
    {
        sfxVol = Mathf.Clamp01(v);
        ApplySFXVolume();
        PlayerPrefs.SetFloat("SFXVol", sfxVol);
        PlayerPrefs.Save();
    }

    void ApplySFXVolume()
    {
        if (!EnsureVCA(ref musicVCA, MUSIC_VCA_PATH)) return;
        sfxVCA.setVolume(sfxVol);
    }

    public void PlayUISet()
    {
        RuntimeManager.PlayOneShot(uiSet);
    }
    #endregion

    bool EnsureVCA(ref VCA vca, string path)
    {
        if (vca.isValid()) return true;
        var r = RuntimeManager.StudioSystem.getVCA(path, out vca);
        return r == FMOD.RESULT.OK && vca.isValid();
    }
}
