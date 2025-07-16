using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class BigCircleDecoration : MonoBehaviour
{
    [SerializeField]
    float rotateTime = 7f;
    void Start()
    {
        RandomDirection();
    }
    void  RandomDirection()
    {
        int direction = Random.value > 0.5f ? 1 : -1;
        transform.DORotate(new Vector3(0, 0, 180f * direction), rotateTime, RotateMode.FastBeyond360)
            .SetRelative().SetEase(Ease.InOutSine).OnComplete(RandomDirection);
    }
}
