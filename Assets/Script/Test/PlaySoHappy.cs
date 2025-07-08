using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoHappy : MonoBehaviour
{
    public int startTime = 0;
    public void MusicStart()
    {
        AudioManager.instance.PlayMusic("event:/SoHappy", startTime);
        BeatManager.instance.BeatStart(SongName.SoHappy);
    }
}
