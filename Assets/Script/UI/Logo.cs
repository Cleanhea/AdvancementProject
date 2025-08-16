using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Logo : MonoBehaviour
{
    [SerializeField] Image logo;
    [SerializeField] float logoDuration = 2f;
    [SerializeField] Image logoText;
    [SerializeField] float logoTextDuration = 2f;
    void Start()
    {
        StartCoroutine(StartLogo());
    }
    IEnumerator StartLogo()
    {
        yield return new WaitForSeconds(1f);
        logo.fillAmount = 0f;
        logo.DOFillAmount(1f, logoDuration).SetEase(Ease.OutCubic);
        AudioManager.instance.PlaySFX(AudioManager.instance.logo);
        yield return new WaitForSeconds(logoDuration);
        logoText.DOFade(1f, 1f);
        yield return new WaitForSeconds(logoTextDuration);
        Sequence seq = DOTween.Sequence();
        seq.Append(logo.DOFade(0f, 1f));
        seq.Join(logoText.DOFade(0f, 1f));
        yield return seq.WaitForCompletion();
        SceneManager.LoadScene("Lobby");
    }
}
