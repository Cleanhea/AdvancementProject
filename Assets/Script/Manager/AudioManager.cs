using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Instance")]
    public static AudioManager instance;

    [Header("SFX")]
    FMOD.ChannelGroup sfxChannelGroup;
    FMOD.Sound[] sfxs;
    FMOD.Channel[] sfxChannels;

    public float sfxVolume = 1f;
    public float masterVolume = 1f;

    void Awake()
    {
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
        LoadSFX();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            PlaySFX(SFX.Kick,1);
        }
    }
    void LoadSFX(){
        int count = System.Enum.GetValues(typeof(SFX)).Length;

        sfxChannelGroup = new FMOD.ChannelGroup();
        sfxChannels = new FMOD.Channel[count];
        sfxs = new FMOD.Sound[count];

        for(int i=0;i<count;i++){
            string sfxFileName = System.Enum.GetName(typeof(SFX),i);
            string audioType = "ogg";

            FMODUnity.RuntimeManager.CoreSystem.createSound(Path.Combine(Application.streamingAssetsPath, "SFXS",sfxFileName+"."+audioType),
            FMOD.MODE.CREATESAMPLE, out sfxs[i]);
        }

        for(int i=0;i<count;i++){
            sfxChannels[i].setChannelGroup(sfxChannelGroup);
        }
    }

    public void PlaySFX(SFX _sfx, float _volume = 1){
        sfxChannels[(int)_sfx].stop();

        FMODUnity.RuntimeManager.CoreSystem.playSound(sfxs[(int)_sfx],sfxChannelGroup,false,
        out sfxChannels[(int)_sfx]);
        sfxChannels[(int)_sfx].setPaused(true);
        sfxChannels[(int)_sfx].setVolume((_volume * sfxVolume) * masterVolume);
        sfxChannels[(int)_sfx].setPaused(false);
    }
}
