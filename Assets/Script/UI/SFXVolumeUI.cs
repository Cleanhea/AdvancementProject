using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SFXVolumeUI : MonoBehaviour, IEndDragHandler
{
    Slider musicSlider;

    void Start()
    {
        musicSlider = GetComponent<Slider>();
        musicSlider.SetValueWithoutNotify(AudioManager.instance.SFXVolume);

        musicSlider.onValueChanged.AddListener(AudioManager.instance.SetSFXVolume);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        AudioManager.instance.PlayUISet();
    }
}
