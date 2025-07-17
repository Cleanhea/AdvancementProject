using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StageLoadContext
{
    public static SongName songName;
    public static void Set(SongName name)
    {
        songName = name;
    }
}
