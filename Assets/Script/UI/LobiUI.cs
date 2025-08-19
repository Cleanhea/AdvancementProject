using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum LobiState
{
    FirstUI,
    MusicSelect,
    Option,
}

public class LobiUI : MonoBehaviour
{
    public static LobiUI instance;
    public GameObject circleUI;
    public GameObject slotUI;
    public GameObject optionPanel;
    Slider volumeSlider;
    [SerializeField]
    float circleMoveTime = 1.5f;
    [SerializeField]
    float slotUIMoveTime = 1.5f;
    [SerializeField]
    float optionUIMoveTime = 1.5f;
    [SerializeField] RectTransform informationPosition;
    [SerializeField] RectTransform outPosition;
    [SerializeField] RectTransform outButtonPosition;
    [SerializeField] RectTransform circlePosition;
    [SerializeField] RectTransform buttonPosition;
    public bool isLogo = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        volumeSlider = optionPanel.transform.GetChild(0).GetComponent<Slider>();
    }
    public IEnumerator GoStageReady()
    {
        Sequence anim = GoStageAnimation();
        AudioManager.instance.PlaySFX(AudioManager.instance.goStage);
        yield return anim.WaitForCompletion();
        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.LoadScene("GameScene");
    }
    public IEnumerator GoOptionReady()
    {
        LobiInput.instance.isOK = false;
        optionPanel.SetActive(true);
        Sequence anim = GoOptionAnimation();
        yield return anim.WaitForCompletion();
        LobiInput.instance.isOK = true;
    }
    public IEnumerator OutOptionReady()
    {
        LobiInput.instance.isOK = false;
        Sequence anim = OutOptionAnimation();
        yield return anim.WaitForCompletion();
        optionPanel.SetActive(false);
        LobiInput.instance.isOK = true;
    }

    public Sequence GoStageAnimation()
    {
        RectTransform circleTransform = circleUI.GetComponent<RectTransform>();
        RectTransform slotTransform = slotUI.GetComponent<RectTransform>();
        Sequence seq = DOTween.Sequence();
        seq.Join(circleTransform.DOAnchorPos(outPosition.anchoredPosition, circleMoveTime).SetEase(Ease.InOutSine));
        seq.Join(slotTransform.DOAnchorPos(outButtonPosition.anchoredPosition, slotUIMoveTime).SetEase(Ease.InOutSine));
        return seq;
    }
    public Sequence GoOptionAnimation()
    {
        EventSystem.current.SetSelectedGameObject(volumeSlider.gameObject);
        Sequence seq = DOTween.Sequence();
        RectTransform slotTransform = slotUI.GetComponent<RectTransform>();
        RectTransform optionTransform = optionPanel.GetComponent<RectTransform>();
        seq.Append(slotTransform.DOAnchorPos(outButtonPosition.anchoredPosition, slotUIMoveTime).SetEase(Ease.InOutSine));
        seq.Append(optionTransform.DOAnchorPos(buttonPosition.anchoredPosition, optionUIMoveTime).SetEase(Ease.InOutSine));
        return seq;
    }
    public Sequence OutOptionAnimation()
    {
        EventSystem.current.SetSelectedGameObject(volumeSlider.gameObject);
        Sequence seq = DOTween.Sequence();
        RectTransform slotTransform = slotUI.GetComponent<RectTransform>();
        RectTransform optionTransform = optionPanel.GetComponent<RectTransform>();
        seq.Append(optionTransform.DOAnchorPos(outButtonPosition.anchoredPosition, optionUIMoveTime).SetEase(Ease.InOutSine));
        seq.Append(slotTransform.DOAnchorPos(buttonPosition.anchoredPosition, slotUIMoveTime).SetEase(Ease.InOutSine));
        return seq;
    }

    public Sequence OutButtonSelect()
    {
        Sequence seq = DOTween.Sequence();
        RectTransform slotTransform = slotUI.GetComponent<RectTransform>();
        RectTransform logoTransform = circleUI.transform.GetChild(0).GetComponent<RectTransform>();
        RectTransform musicInformationTransform = circleUI.transform.GetChild(1).GetComponent<RectTransform>();
        seq.Join(slotTransform.DOAnchorPos(outButtonPosition.anchoredPosition, slotUIMoveTime).SetEase(Ease.InOutSine));
        if (isLogo)
        {
            seq.Join(logoTransform.DOAnchorPos(outPosition.anchoredPosition, circleMoveTime).SetEase(Ease.InOutSine));
        }
        else
        {
            seq.Join(musicInformationTransform.DOAnchorPos(outPosition.anchoredPosition, circleMoveTime).SetEase(Ease.InOutSine));
        }
        return seq;
    }
    public Sequence InCircle()
    {
        Sequence seq = DOTween.Sequence();
        RectTransform circleTransform = circleUI.transform.GetChild(2).GetComponent<RectTransform>();
        seq.Join(circleTransform.DOAnchorPos(circlePosition.anchoredPosition, circleMoveTime).SetEase(Ease.InOutSine));
        return seq;
    }
    public Sequence InButtonSelect()
    {
        Sequence seq = DOTween.Sequence();
        RectTransform slotTransform = slotUI.GetComponent<RectTransform>();
        RectTransform logoTransform = circleUI.transform.GetChild(0).GetComponent<RectTransform>();
        RectTransform musicInformationTransform = circleUI.transform.GetChild(1).GetComponent<RectTransform>();
        seq.Join(slotTransform.DOAnchorPos(buttonPosition.anchoredPosition, slotUIMoveTime).SetEase(Ease.InOutSine));
        if (!isLogo)
        {
            seq.Join(logoTransform.DOAnchorPos(informationPosition.anchoredPosition, circleMoveTime).SetEase(Ease.InOutSine));
        }
        else
        {
            seq.Join(musicInformationTransform.DOAnchorPos(informationPosition.anchoredPosition, circleMoveTime).SetEase(Ease.InOutSine));
        }
        return seq;
    }
    
}
