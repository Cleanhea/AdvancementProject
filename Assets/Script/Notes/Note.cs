using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraPos
{
    public float x;
    public float y;
}


[System.Serializable]
public class Notes
{
    public int bar;
    public int beat;
    public int direction;//0: left, 1: right, 2: up, 3: down
    public string sevent;
    public int type;// 0 : left, 1 : right
    public CameraPos cameraPosition;
    public float cameraZoom;
    public float zoomSpeed;
    public float guideCircleSpeed;
    public string image;
    public string input;
    public Vector3 moveCirclePosition;
    public int type2;
}

[System.Serializable]
public class NotesData
{
    public string name;
    public float bpm;
    public int clearBeat;
    public float guideCircleSpeed;
    public List<Notes> notes;
}