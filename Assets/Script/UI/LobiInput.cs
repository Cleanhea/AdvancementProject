using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ButtonSlot
{
    public Transform ButtonTransform;
    public GameObject UIButton;
}

public class LobiInput : MonoBehaviour
{
    [SerializeField]
    // W, A, S, D, J(클릭), K(취소) 순
    KeyCode[] UIKey = new KeyCode[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.J, KeyCode.K };
    public static LobiInput instance;
    public bool isOK = true;
    public LobiState lobiState;
    //Start, Quit, Setting
    public GameObject[] firstUIButton = new GameObject[3];
    public ButtonSlot[] buttonSlots = new ButtonSlot[5];
    public int selectIndex = 0;
    [SerializeField]
    Transform hiddenParent;

    void Awake()
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
        RefreshSlots();
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
        if (Input.GetKeyDown(UIKey[0]))
        {
            if (selectIndex > 0)
            {
                selectIndex--;
                RefreshSlots();
            }
        }
        else if (Input.GetKeyDown(UIKey[2]))
        {
            if (selectIndex < firstUIButton.Length - 1)
            {
                selectIndex++;
                RefreshSlots();
            }
        }
    }
    void RefreshSlots()
    {
        int N = firstUIButton.Length;
        int S = buttonSlots.Length;
        int center = S / 2;
        GameObject[] desired = new GameObject[S];

        for (int slotIdx = 0; slotIdx < S; slotIdx++)
        {
            int dataIndex = selectIndex + (slotIdx - center);
            GameObject btn = (dataIndex >= 0 && dataIndex < N) ? firstUIButton[dataIndex] : null;
            desired[slotIdx] = btn;
        }
        for (int i = 0; i < N; i++)
        {
            HideButton(firstUIButton[i]);
        }
        for (int slotIdx = 0; slotIdx < S; slotIdx++)
        {
            var slot = buttonSlots[slotIdx];
            if (slot == null || slot.ButtonTransform == null) continue;

            var newBtn = desired[slotIdx];
            slot.UIButton = newBtn;

            if (newBtn != null)
            {
                ShowButtonInSlot(newBtn, slot.ButtonTransform);
            }
        }
    }

    void ShowButtonInSlot(GameObject go, Transform parent)
    {
        if (!go.activeSelf) go.SetActive(true);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
    }
    void HideButton(GameObject go)
    {
        if (hiddenParent != null)
        {
            go.transform.SetParent(hiddenParent, false);
        }
        // 표시되지 않게
        go.SetActive(false);
    }
}
