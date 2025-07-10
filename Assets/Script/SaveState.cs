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
    public Vector3 leftCirclePosition;
    public Vector3 rightCirclePosition;
    public Queue<NoteCreate> leftFootHoldQueue = new Queue<NoteCreate>();
    public Queue<NoteCreate> rightFootHoldQueue = new Queue<NoteCreate>();

}