using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

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
    [SerializeField]
    Vector3 circleMoveOffset;
    [SerializeField]
    float circleMoveTime = 1.5f;
    [SerializeField]
    Vector3 slotUIMoveOffset;
    [SerializeField]
    float slotUIMoveTime = 1.5f;
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
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
}
