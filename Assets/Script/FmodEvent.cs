using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
public class FmodEvent : MonoBehaviour
{
    [field:Header("LoopSFX")]
    [field: SerializeField] public EventReference LoopSFX { get; private set; }
    [field: Header("KickSFX")]
    [field: SerializeField] public EventReference KickSFX { get; private set; }
    public static FmodEvent instance;
    private void Awake()
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
}
