using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Notes
{
    public int bar;
    public int beat;
    public int direction;//0: left, 1: right, 2: up, 3: down
    public string sevent;
    public int type;// 0 : left, 1 : right
    public int cameraX;
    public int cameraY;
}

[System.Serializable]
public class NotesList
{
    public List<Notes> notes;
}