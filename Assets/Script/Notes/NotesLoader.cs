using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SongName
{
    SoHappy,
    Stay,
    HaiPhutHon,
    Matches,
    Tutorial,
    TocaToca,
}

public class NotesLoader : MonoBehaviour
{
    public static NotesLoader instance;
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
    public static NotesData LoadChart(SongName songName)
    {
        NotesData temp;
        string path = $"Json/{songName}";
        TextAsset jsonFile = Resources.Load<TextAsset>(path);
        if (jsonFile == null)
        {
            Debug.LogError("파일이 읎어요 이사람아." + path);
            return null;
        }
        temp = JsonUtility.FromJson<NotesData>(jsonFile.text);
        return temp;
    }
}