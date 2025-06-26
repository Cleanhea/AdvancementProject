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
    private FMOD.Studio.EVENT_CALLBACK beatCallback;
    EventInstance bgmInstance;
    string currentBGMPath;
    public static event Action<int, int> OnBeat; // bar, beat

    void Awake()
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
    // 재생
    public void PlayMusic(string eventPath)
    {
        if (currentBGMPath == eventPath)
            return;
        StopMusic();
        bgmInstance = RuntimeManager.CreateInstance(eventPath);
        beatCallback = OnTimelineBeat;
        bgmInstance.setCallback(beatCallback, EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
        bgmInstance.start();
        currentBGMPath = eventPath;
    }
    FMOD.RESULT OnTimelineBeat(EVENT_CALLBACK_TYPE type, IntPtr eventInstPtr, IntPtr paramPtr)
    {
        if (type == EVENT_CALLBACK_TYPE.TIMELINE_BEAT)
        {
            if(!Application.isPlaying)
                return FMOD.RESULT.OK;
            if (!PimDeWitte.UnityMainThreadDispatcher.UnityMainThreadDispatcher.Exists()) {
                return FMOD.RESULT.OK;
            }
            var beat = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)
                System.Runtime.InteropServices.Marshal.PtrToStructure(paramPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));

            int bar = beat.bar;
            int beatIndex = beat.beat;
            int positionMs = beat.position;

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
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
}
