using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SystemUIManager : SingletonBehaviour<SystemUIManager>, IManager
{
    #region Inspector
    public Image imgFade;
    public GameObject objIndicator;
    #endregion Inspector



    public override void Init()
    {
        base.Init();

        IsFadeOut = false;
    }



    #region FadeIn/Out
    private Coroutine _coFadeIn = null;
    private Coroutine _coFadeOut = null;
    private const float FadeTime = 0.3f;

    public bool IsFadeOut { get; private set; }

    public void FadeIn(Action onFinished = null)
    {
        if (IsFadeOut == false)
        {
            onFinished?.Invoke();
            return;
        }

        IsFadeOut = false;

        if (_coFadeIn != null)
            StopCoroutine(_coFadeIn);
        if (_coFadeOut != null)
            StopCoroutine(_coFadeOut);

        _coFadeIn = StartCoroutine(CoFadeIn(onFinished));
    }

    private IEnumerator CoFadeIn(Action onFinished = null)
    {
        float time = 0f;
        Color color = imgFade.color;

        imgFade.gameObject.SetActive(true);

        while (true)
        {
            time += Time.deltaTime;

            if (time > FadeTime)
            {
                color.a = 0f;
                imgFade.color = color;

                break;
            }

            color.a = Mathf.Max(FadeTime - time, 0f) / FadeTime;
            imgFade.color = color;
        }

        imgFade.gameObject.SetActive(false);
        onFinished?.Invoke();

        _coFadeIn = null;

        yield return null;
    }

    public void FadeOut(Action onFinished = null)
    {
        if (IsFadeOut == true)
        {
            onFinished?.Invoke();
            return;
        }

        IsFadeOut = true;
        // 터막.

        if (_coFadeIn != null)
            StopCoroutine(_coFadeIn);
        if (_coFadeOut != null)
            StopCoroutine(_coFadeOut);

        _coFadeOut = StartCoroutine(CoFadeOut(onFinished));
    }

    private IEnumerator CoFadeOut(Action onFinished = null)
    {
        float time = 0f;
        Color color = imgFade.color;

        imgFade.gameObject.SetActive(true);

        while(true)
        {
            time += Time.deltaTime;

            if (time > FadeTime)
            {
                color.a = 1f;
                imgFade.color = color;

                break;
            }

            color.a = time / FadeTime;
            imgFade.color = color;
        }

        imgFade.gameObject.SetActive(false);
        onFinished?.Invoke();

        _coFadeOut = null;

        yield return null;
    }
    #endregion FadeIn/Out



    #region Indicator
    private int _indicatorCount = 0;
    public bool IsIndicatorOn => objIndicator.activeInHierarchy;

    public void IndicatorOn()
    {
        objIndicator.SetActive(true);

        ++_indicatorCount;

        Utils.Log($"[SystemUIManager] IndicatorOn : {_indicatorCount}.");
    }

    public void IndicatorOff()
    {
        objIndicator.SetActive(false);

        --_indicatorCount;

        Utils.Log($"[SystemUIManager] IndicatorOff : {_indicatorCount}");
    }
    #endregion Indicator
}
