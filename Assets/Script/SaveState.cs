using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveState
{
    public int musicTime = 0;
    public int currentBar = 0;
    public int currentBeat = 0;
    public CameraPos cameraPosition;
    public float CameraZoom;
    public Vector3 leftCirclePosition;
    public Vector3 rightCirclePosition;
    public bool isInversion = false;
    public Color globalLightColor;
    public List<Notes> remainingNotes = new List<Notes>();
}