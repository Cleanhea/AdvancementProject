using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SongName
{
    SoHappy,
}

public class NotesLoader : MonoBehaviour
{
    public static NotesLoader instance;
    public float bpm;
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
            Debug.LogError($"파일을 찾을 수 없습니다: Resources/{path}.json");
            return null;
        }
        temp = JsonUtility.FromJson<NotesData>(jsonFile.text);
        return temp;
    }
}