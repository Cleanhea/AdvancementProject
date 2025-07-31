using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StageEvent
{
    public static void Sevent(string a)
    {
        switch (a)
        {
            case ("saveOK"):
                {
                    GameManager.instance.MarkSaveOK();
                    break;
                }
            case ("leftLightOn"):
                {
                    BeatEvent.instance.LightOn("left");
                    break;
            }
            case ("rightLightOn"):
                {
                    BeatEvent.instance.LightOn("right");
                    break;
            }
            case ("allLightOn"):
                {
                    BeatEvent.instance.LightOn("all");
                    break;
            }
            default:
                {
                    Debug.LogWarning("오류나씀");
                    break;
            }
        }
    }
}
