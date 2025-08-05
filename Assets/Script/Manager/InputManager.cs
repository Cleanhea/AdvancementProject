using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    //0 : left, 1: right, 2: up, 3: down
    [SerializeField]
    KeyCode[] leftKeys = new KeyCode[4];
    [SerializeField]
    KeyCode[] rightKeys = new KeyCode[4];
    [SerializeField]
    KeyCode ESC = KeyCode.Escape;

    void Update()
    {
        if (Input.GetKeyDown(ESC) && GameManager.instance.gameState == GameState.inGame)
        {
            GameManager.OnPauseRequest?.Invoke();
            return;
        }
        if (GameManager.instance.isInputEnabled == false)
        {
            return;
        }
        if (GameManager.instance.gameState == GameState.inGame)
        {
            HandleInput(BeatEvent.instance.leftFootHoldQueue, leftKeys, 0);
            HandleInput(BeatEvent.instance.rightFootHoldQueue, rightKeys, 1);
        }
    }

    //유저 인풋 처리
    void HandleInput(Queue<NoteCreate> queue, KeyCode[] keys, int type)
    {
        if (queue.Count == 0)
            return;
        int dir = queue.Peek().noteData.direction;
        Notes noteData = queue.Peek().noteData;
        for (int i = 0; i < keys.Length; i++)
        {
            if (queue.Peek().noteState == NoteState.Ready)
            {
                if (Input.GetKeyDown(keys[i]) && i != dir)
                {
                    Debug.Log("wrong key");
                    GameManager.instance.RestartGame();
                    return;
                }
            }
        }
        if (Input.GetKeyDown(keys[dir]))
        {
            //성공시
            if (queue.Peek().noteState == NoteState.Available)
            {
                if (noteData.cameraPosition.y != 0 || noteData.cameraPosition.x != 0)
                {
                    Debug.Log("move camera");
                    BeatEvent.instance.MoveCamera(new Vector3(noteData.cameraPosition.x, noteData.cameraPosition.y, 0));
                }
                BeatEvent.instance.ClearNote(noteData.type);
                StartCoroutine(queue.Peek().SetPointCircle());
                if (noteData.cameraZoom != 0)
                {
                    BeatEvent.instance.SetCameraZoom(noteData.cameraZoom,noteData.zoomSpeed);
                }
                BeatEvent.instance.MoveCircle(dir, type);
                if (noteData.input != null)
                {
                    StageEvent.Sevent(noteData.input);
                }
                queue.Peek().noteState = NoteState.Clear;
                queue.Peek().StartCoroutine(queue.Peek().DestroyNote());
                queue.Dequeue();
            }
            else
            {
                if (BeatManager.instance.Playname != SongName.Tutorial)
                {
                    Debug.Log("over");
                    GameManager.instance.RestartGame();
                }
            }
        }
    }
}
