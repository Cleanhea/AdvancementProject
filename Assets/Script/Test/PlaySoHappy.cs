using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoHappy : MonoBehaviour
{
    public int startTime = 0;
    public void MusicStart()
    {
        BeatEvent.instance.leftPoint = BeatEvent.instance.startLeftCirclePosition;
        BeatEvent.instance.rightPoint = BeatEvent.instance.startRightCirclePosition;
        AudioManager.instance.PlayMusic("event:/SoHappy", startTime);
        BeatManager.instance.BeatStart(SongName.SoHappy);
    }
}
