using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;
    public bool inversion = false;
    [SerializeField]
    Camera mainCamera;
    public Color defaultColor;
    public Color inversionColor;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void InversionMode()
    {
        StartCoroutine(InversionStart());
    }
    public void DefaultMode()
    {
        StartCoroutine(DefaultStart());
    }

    IEnumerator InversionStart()
    {
        inversion = true;
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm * 4f);
        mainCamera.backgroundColor = inversionColor;
    }
    IEnumerator DefaultStart()
    {
        inversion = false;
        yield return new WaitForSeconds(60f / BeatManager.instance.notes.bpm * 4f);
        mainCamera.backgroundColor = defaultColor;
    }
}
