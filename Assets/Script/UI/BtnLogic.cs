using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BtnLogic : MonoBehaviour
{
    public void GoMusicSelect()
    {
        Debug.Log("GoMusicSelect");
        LobiInput.instance.StartCoroutine(LobiInput.instance.MusicSelectSetting());
    }

    public void GoOption()
    {
        Debug.Log("GoOption");
        LobiInput.instance.lobiState = LobiState.Option;
        LobiUI.instance.StartCoroutine(LobiUI.instance.GoOptionReady());
    }

    public void QuitGame()
    {
        Debug.Log("QuitGame");
        Application.Quit();
    }

    public void GoHaiPhutHon()
    {
        DOTween.KillAll();
        StageLoadContext.Set(SongName.HaiPhutHon);
        LobiUI.instance.StartCoroutine(LobiUI.instance.GoStageReady());
    }
    public void GoSoHappy()
    {
        DOTween.KillAll();
        StageLoadContext.Set(SongName.SoHappy);
        LobiUI.instance.StartCoroutine(LobiUI.instance.GoStageReady());
    }
    public void GoStay()
    {
        DOTween.KillAll();
        StageLoadContext.Set(SongName.Stay);
        LobiUI.instance.StartCoroutine(LobiUI.instance.GoStageReady());
    }
    public void GoMatches()
    {
        DOTween.KillAll();
        StageLoadContext.Set(SongName.Matches);
        LobiUI.instance.StartCoroutine(LobiUI.instance.GoStageReady());
    }
}
