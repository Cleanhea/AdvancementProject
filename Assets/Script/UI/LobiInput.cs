using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

    public TextMeshProUGUI songName;
    public TextMeshProUGUI composer;
    public TextMeshProUGUI bpm;
    public TextMeshProUGUI level;
    public GameObject clearPanel;
    public TextMeshProUGUI death;
    public Image coverImage;

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
        
    }
    void Start()
    {
        LobiUI.instance.InCircle();
        if (!GameManager.instance.startGame)
        {
            StartCoroutine(FirstUISetting());
            GameManager.instance.startGame = true;
        }
        else
        {
            StartCoroutine(MusicSelectSetting());
        }
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
                    OptionInput();
                    break;
            }
        }
    }
    void OptionInput()
    {
        if (Input.GetKeyDown(UIKey[5]))
        {
            if (lobiState == LobiState.Option)
            {
                AudioManager.instance.PlaySFX(AudioManager.instance.uiCancel);
                LobiUI.instance.StartCoroutine(LobiUI.instance.OutOptionReady());
                lobiState = LobiState.FirstUI;
            }
        }
    }
    void SlotInput()
    {
        if (Input.GetKeyDown(UIKey[0]))
        {
            if (selectIndex > 0)
            {
                AudioManager.instance.PlaySFX(AudioManager.instance.uiSet);
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
                    AudioManager.instance.PlaySFX(AudioManager.instance.uiSet);
                    selectIndex++;
                    RefreshSlots();
                }
            }
            else
            {
                if (selectIndex < firstUIButton.Length - 1)
                {
                    AudioManager.instance.PlaySFX(AudioManager.instance.uiSet);
                    selectIndex++;
                    RefreshSlots();
                }
            }
        }
        else if (Input.GetKeyDown(UIKey[4]))
        {
            if(lobiState == LobiState.FirstUI)
                AudioManager.instance.PlaySFX(AudioManager.instance.uiClick);
            GameObject selectedButton = buttonSlots[2].UIButton;
            selectedButton?.GetComponent<Button>()?.onClick.Invoke();
        }
        else if (Input.GetKeyDown(UIKey[5]))
        {
            if (lobiState == LobiState.MusicSelect)
            {
                AudioManager.instance.PlaySFX(AudioManager.instance.uiCancel);
                StartCoroutine(FirstUISetting());
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
        if (lobiState == LobiState.MusicSelect)
        {
            RefreshSongInformation();
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
        go.SetActive(false);
    }
    public IEnumerator MusicSelectSetting()
    {
        isOK = false;
        LobiUI.instance.isLogo = true;
        Sequence anim = LobiUI.instance.OutButtonSelect();
        yield return anim.WaitForCompletion();
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
        anim = LobiUI.instance.InButtonSelect();
        yield return anim.WaitForCompletion();
        isOK = true;
    }
    public IEnumerator FirstUISetting()
    {
        isOK = false;
        LobiUI.instance.isLogo = false;
        Sequence anim = LobiUI.instance.OutButtonSelect();
        yield return anim.WaitForCompletion();
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
        anim = LobiUI.instance.InButtonSelect();
        yield return anim.WaitForCompletion();
        isOK = true;
    }
    public void RefreshSongInformation()
    {
        SongInformation songInformation = buttonSlots[2].UIButton.GetComponent<SongInformation>();
        if (PlayerPrefs.GetInt(songInformation.songIDName+"Clear") == 1)
        {
            songInformation.clear = true;
            songInformation.death = PlayerPrefs.GetInt(songInformation.songIDName + "Death");
        }
        songName.text = songInformation.songName;
        bpm.text = songInformation.Bpm.ToString();
        level.text = songInformation.level.ToString();
        composer.text = songInformation.composer;
        coverImage.sprite = songInformation.coverImage;
        if (songInformation.clear)
        {
            clearPanel.SetActive(true);
            death.text = songInformation.death.ToString();
        }
        else
        {
            clearPanel.SetActive(false);
        }
    }
}
