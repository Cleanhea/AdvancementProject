using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnLogic : MonoBehaviour
{
    public void GoMusicSelect()
    {
        Debug.Log("GoMusicSelect");
        LobiInput.instance.MusicSelectSetting();
    }

    public void GoOption()
    {
        Debug.Log("GoOption");
        LobiInput.instance.lobiState = LobiState.Option;
    }

    public void QuitGame()
    {
        Debug.Log("QuitGame");
        Application.Quit();
    }

    public void GoStage1()
    {
        DOTween.KillAll();
        StageLoadContext.Set(SongName.SoHappy);
        StartCoroutine(GoStageReady());
    }
    public void GoStage2()
    {
        DOTween.KillAll();
        StageLoadContext.Set(SongName.Stay);
        StartCoroutine(GoStageReady());
    }


    public IEnumerator GoStageReady()
    {
        Sequence anim = LobiUI.instance.GoStageAnimation();
        yield return anim.WaitForCompletion();
        SceneManager.LoadScene("GameScene");
    }
}
