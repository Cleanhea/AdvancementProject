using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    private EventInstance test;
    void Start()
    {
        test = AudioManager.instance.CreateInstance(FmodEvent.instance.LoopSFX);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("??");
            AudioManager.instance.PlayOneShot(FmodEvent.instance.KickSFX, transform.position);
        }

        UpdateSound();
    }

    private void UpdateSound()
    {
        PLAYBACK_STATE state;
        test.getPlaybackState(out state);
        if (state.Equals(PLAYBACK_STATE.STOPPED))
        {
            test.start();
        }
        else
        {
            test.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
