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

    void Update()
    {
        HandleInput(BeatEvent.instance.leftFootHoldQueue, leftKeys, 0);
        HandleInput(BeatEvent.instance.rightFootHoldQueue, rightKeys, 1);
    }

    //유저 인풋 처리
    void HandleInput(Queue<NoteCreate> queue, KeyCode[] keys, int type)
    {
        if (queue.Count == 0)
            return;
        int dir = queue.Peek().noteData.direction;
        Notes noteData = queue.Peek().noteData;
        if (Input.GetKeyDown(keys[dir]))
        {
            //성공시
            if (queue.Peek().noteState == NoteState.Available)
            {
                if (noteData.cameraPosition.y != 0 || noteData.cameraPosition.x != 0)
                {
                    BeatEvent.instance.MoveCamera(new Vector3(noteData.cameraPosition.x, noteData.cameraPosition.y, -10));
                }
                StartCoroutine(queue.Peek().SetPointCircle());
                if (noteData.cameraZoom != 0)
                {
                    BeatEvent.instance.SetCameraZoom(noteData.cameraZoom);
                }
                BeatEvent.instance.MoveCircle(dir, type);
                queue.Peek().noteState = NoteState.Clear;
                queue.Dequeue();
            }
            else if (queue.Peek().noteState == NoteState.Ready)
            {
                Debug.Log("over");
                GameManager.instance.RestartGame();
            }
        }
    }
}
