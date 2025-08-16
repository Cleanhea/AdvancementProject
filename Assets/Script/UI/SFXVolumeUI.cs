using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFXVolumeUI: MonoBehaviour
{
    Slider musicSlider;

    void Start()
    {
        musicSlider = GetComponent<Slider>();
        musicSlider.SetValueWithoutNotify(AudioManager.instance.SFXVolume);

        musicSlider.onValueChanged.AddListener(AudioManager.instance.SetSFXVolume);
    }
}
