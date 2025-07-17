using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ButtonSlot
{
    public Transform ButtonTransform;
    public GameObject UIButton;
    public float TextSize;
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
    public GameObject[] musicSelectUIButton = new GameObject[6];
    public ButtonSlot[] buttonSlots = new ButtonSlot[5];
    public int selectIndex = 0;
    [SerializeField]
    Transform hiddenParent;
    [SerializeField]
    Transform firstUIParent;
    [SerializeField]
    Transform musicSelectUIParent;

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
                    SlotInput();
                    break;
                case LobiState.MusicSelect:
                    SlotInput();
                    break;
                case LobiState.Option:
                    break;
            }
        }
    }

    void SlotInput()
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
            if (lobiState == LobiState.MusicSelect)
            {
                if (selectIndex < musicSelectUIButton.Length - 1)
                {
                    selectIndex++;
                    RefreshSlots();
                }
            }
            else
            {
                if (selectIndex < firstUIButton.Length - 1)
                {
                    selectIndex++;
                    RefreshSlots();
                }
            }
        }
        else if (Input.GetKeyDown(UIKey[4]))
        {
            GameObject selectedButton = buttonSlots[2].UIButton;
            selectedButton?.GetComponent<Button>()?.onClick.Invoke();
        }
        else if (Input.GetKeyDown(UIKey[5]))
        {
            if (lobiState == LobiState.MusicSelect)
            {
                FirstUISetting();
            }
        }
    }

    public void RefreshSlots()
    {
        int N = firstUIButton.Length;
        if (lobiState == LobiState.MusicSelect)
        {
            N = musicSelectUIButton.Length;
        }

        int S = buttonSlots.Length;
        int center = S / 2;
        GameObject[] desired = new GameObject[S];

        for (int slotIdx = 0; slotIdx < S; slotIdx++)
        {
            int dataIndex = selectIndex + (slotIdx - center);
            GameObject btn;
            if (lobiState == LobiState.MusicSelect)
                btn = (dataIndex >= 0 && dataIndex < N) ? musicSelectUIButton[dataIndex] : null;
            else
                btn = (dataIndex >= 0 && dataIndex < N) ? firstUIButton[dataIndex] : null;
            desired[slotIdx] = btn;
        }
        for (int i = 0; i < N; i++)
        {
            if (lobiState == LobiState.MusicSelect)
                HideButton(musicSelectUIButton[i]);
            else
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
                ShowButtonInSlot(newBtn, slot);
            }
        }
    }

    void ShowButtonInSlot(GameObject go, ButtonSlot slot)
    {
        if (!go.activeSelf) go.SetActive(true);
        TextMeshProUGUI text = go.GetComponentInChildren<TextMeshProUGUI>();
        text.fontSize = slot.TextSize;
        Transform parent = slot.ButtonTransform;
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
    public void MusicSelectSetting()
    {
        lobiState = LobiState.MusicSelect;
        selectIndex = 0;
        for (int i = 0; i < buttonSlots.Length; i++)
        {
            buttonSlots[i].UIButton = null;
        }
        for (int i = 0; i < firstUIButton.Length; i++)
        {
            firstUIButton[i].SetActive(false);
            firstUIButton[i].transform.SetParent(firstUIParent, false);
        }
        RefreshSlots();
    }
    public void FirstUISetting()
    {
        lobiState = LobiState.FirstUI;
        selectIndex = 0;
        for (int i = 0; i < buttonSlots.Length; i++)
        {
            buttonSlots[i].UIButton = null;
        }
        for (int i = 0; i < musicSelectUIButton.Length; i++)
        {
            musicSelectUIButton[i].SetActive(false);
            musicSelectUIButton[i].transform.SetParent(musicSelectUIParent, false);
        }
        RefreshSlots();
    }
}
