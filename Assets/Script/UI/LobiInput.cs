using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LobiInput : MonoBehaviour
{
    [SerializeField]
    // W, A, S, D, J(클릭), K(취소) 순
    KeyCode[] UIKey = new KeyCode[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.J, KeyCode.K };
    public static LobiInput instance;
    public bool isOK=true;
    public LobiState lobiState;
    //Start, Quit, Setting
    public GameObject[] firstUIButton = new GameObject[3];

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        lobiState = LobiState.FirstUI;
    }

    void Update()
    {
        if (isOK == true)
        {
            switch (lobiState)
            {
                case LobiState.FirstUI:
                    FirstUIInput();
                    break;
                case LobiState.MusicSelect:
                    break;
                case LobiState.Option:
                    break;
            }
        }
    }

    void FirstUIInput()
    {
        
    }
}
