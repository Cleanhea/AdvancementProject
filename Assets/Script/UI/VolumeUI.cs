using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeUI : MonoBehaviour
{
    Slider musicSlider;

    void Start()
    {
        musicSlider = GetComponent<Slider>();
        musicSlider.SetValueWithoutNotify(AudioManager.instance.MusicVolume);

        musicSlider.onValueChanged.AddListener(AudioManager.instance.SetMusicVolume);
    }
}
