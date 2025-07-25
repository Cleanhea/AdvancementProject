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
    Vector3 circleMoveOffset;
    [SerializeField]
    float circleMoveTime = 1.5f;
    [SerializeField]
    Vector3 slotUIMoveOffset;
    [SerializeField]
    float slotUIMoveTime = 1.5f;
    [SerializeField]
    Vector3 optionUIInOffset;
    [SerializeField]
    float optionUIMoveTime = 1.5f;
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
        Transform circleTransform = circleUI.transform;
        Transform slotTransform = slotUI.transform;
        Sequence seq = DOTween.Sequence();
        seq.Join(circleTransform.DOMove(circleTransform.position + circleMoveOffset, circleMoveTime).SetEase(Ease.InOutSine));
        seq.Join(slotTransform.DOMove(slotTransform.position + slotUIMoveOffset, slotUIMoveTime).SetEase(Ease.InOutSine));

        return seq;
    }
    public Sequence GoOptionAnimation()
    {
        EventSystem.current.SetSelectedGameObject(volumeSlider.gameObject);
        Sequence seq = DOTween.Sequence();
        Transform slotTransform = slotUI.transform;
        Transform optionTransform = optionPanel.transform;
        seq.Append(slotTransform.DOMove(slotTransform.position + slotUIMoveOffset, slotUIMoveTime).SetEase(Ease.InOutSine));
        seq.Append(optionTransform.DOMove(optionTransform.position - optionUIInOffset, optionUIMoveTime).SetEase(Ease.InOutSine));
        return seq;
    }
    public Sequence OutOptionAnimation()
    {
        EventSystem.current.SetSelectedGameObject(volumeSlider.gameObject);
        Sequence seq = DOTween.Sequence();
        Transform slotTransform = slotUI.transform;
        Transform optionTransform = optionPanel.transform;
        seq.Append(optionTransform.DOMove(optionTransform.position + optionUIInOffset, optionUIMoveTime).SetEase(Ease.InOutSine));
        seq.Append(slotTransform.DOMove(slotTransform.position - slotUIMoveOffset, slotUIMoveTime).SetEase(Ease.InOutSine));
        return seq;
    }

    public Sequence OutButtonSelect()
    {
        Sequence seq = DOTween.Sequence();
        Transform slotTransform = slotUI.transform;
        Transform logoTransform = circleUI.transform.GetChild(0).GetComponent<Transform>();
        Transform musicInformationTransform = circleUI.transform.GetChild(1).GetComponent<Transform>();
        seq.Join(slotTransform.DOMove(slotTransform.position + slotUIMoveOffset, slotUIMoveTime).SetEase(Ease.InOutSine));
        if (isLogo)
        {
            seq.Join(logoTransform.DOMove(logoTransform.position + circleMoveOffset, circleMoveTime).SetEase(Ease.InOutSine));
        }
        else
        {
            seq.Join(musicInformationTransform.DOMove(logoTransform.position + circleMoveOffset, circleMoveTime).SetEase(Ease.InOutSine));
        }
        return seq;
    }

    public Sequence InButtonSelect()
    {
        Sequence seq = DOTween.Sequence();
        Transform slotTransform = slotUI.transform;
        Transform logoTransform = circleUI.transform.GetChild(0).GetComponent<Transform>();
        Transform musicInformationTransform = circleUI.transform.GetChild(1).GetComponent<Transform>();
        seq.Join(slotTransform.DOMove(slotTransform.position - slotUIMoveOffset, slotUIMoveTime).SetEase(Ease.InOutSine));
        if (!isLogo)
        {
            seq.Join(logoTransform.DOMove(logoTransform.position - circleMoveOffset, circleMoveTime).SetEase(Ease.InOutSine));
        }
        else
        {
            seq.Join(musicInformationTransform.DOMove(logoTransform.position - circleMoveOffset, circleMoveTime).SetEase(Ease.InOutSine));
        }
        return seq;
    }
}
