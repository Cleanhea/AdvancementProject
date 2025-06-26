using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoHappy : MonoBehaviour
{
    public void MusicStart()
    {
        AudioManager.instance.PlayMusic("event:/SoHappy");
        BeatManager.instance.BeatStart();
    }
}
