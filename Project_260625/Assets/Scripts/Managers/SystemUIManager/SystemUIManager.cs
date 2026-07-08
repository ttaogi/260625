using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SystemUIManager : SingletonBehaviour<SystemUIManager>, IManager
{
    #region Inspector
    [Header("Fade In/Out.")]
    public Image imgFade;

    [Header("Loading.")]
    public GameObject objLoading;
    public Image imgLoading;

    [Header("Toast Message.")]
    public CanvasGroup canvasGroupToastMessage;
    public GameObject objToastMessage;
    public RectTransform rectToastMessageBackground;
    public TMP_Text textToastMessage;

    [Header("Notice.")]
    public NoticeViewer noticeViewer;

    [Header("Indicator.")]
    public GameObject objIndicator;
    public GameObject objIndicatorIcon;
    #endregion Inspector



    public override void Init()
    {
        base.Init();

        // Fade In/Out.
        _coFadeIn = null;
        _coFadeOut = null;
        IsFadeOut = false;

        // Loading.
        objLoading.SetActive(false);
        imgLoading.fillAmount = 0f;

        // Toast Message.
        canvasGroupToastMessage.alpha = 0f;
        objToastMessage.SetActive(false);
        rectToastMessageBackground.sizeDelta = new(textToastMessage.rectTransform.rect.width,
                                                   textToastMessage.rectTransform.rect.height);
        textToastMessage.text = string.Empty;

        // Notice.
        noticeViewer.Init();

        // Indicator.
        _indicatorCount = 0;
        objIndicator.SetActive(false);
    }



    #region Fade In/Out
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
        Utils.Log($"[SystemUIManager] FadeIn.");

        float time = 0f;
        Color color = imgFade.color;

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
        Utils.Log($"[SystemUIManager] FadeOut.");

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

        onFinished?.Invoke();

        _coFadeOut = null;

        yield return null;
    }
    #endregion Fade In/Out



    #region Loading
    public void LoadingOn()
    {
        objLoading.SetActive(true);
        imgLoading.fillAmount = 0f;
    }

    /// <summary> ration : 0 ~ 1. </summary>
    public void SetLoading(float ration)
    {
        imgLoading.fillAmount = Mathf.Clamp01(ration);
    }

    public void LoadingOff()
    {
        objLoading.SetActive(false);
        imgLoading.fillAmount = 0f;
    }
    #endregion Loading



    #region Toast Message
    private const float FADE_TOAST_MESSAGE_TIME = 0.1f;
    private const float SHOW_TOAST_MESSAGE_TIME = 1.0f;
    private Coroutine _coShowMessage = null;

    public void ShowMessage(string msg)
    {
        if (_coShowMessage != null)
        {
            StopCoroutine(_coShowMessage);
            _coShowMessage = null;
        }

        _coShowMessage = StartCoroutine(CoShowMessage(msg));
    }

    private IEnumerator CoShowMessage(string msg)
    {
        canvasGroupToastMessage.alpha = 0f;
        objToastMessage.SetActive(true);
        textToastMessage.text = msg;

        yield return null;

        rectToastMessageBackground.sizeDelta = new(textToastMessage.rectTransform.rect.width,
                                                   textToastMessage.rectTransform.rect.height);


        /*
         * Sequence _seqMessageOn;
         * Sequence _seqMessageOff;
         * 
         * _seqMessageOn?.Kill();
         * _seqMessageOff?.kill();
         * 
         * _seqMessageOn = DOTween.Sequence();
         * _seqMessageOn.timeScale = 1f;
         * _seqMessageOn.Append(canvasGroupToastMessage.DOFade(1, FADE_TOAST_MESSAGE_TIME).SetEase(Ease.봐서 결정)).SetUpdate(true);
         * _seqMessageOn.OnComplete(() =>
         * {
         *  _seqMessageOff = DOTween.Sequence();
         *  _seqMessageOff.timeScale = 1f;
         *  _seqMessageOff.SetDelay(SHOW_TOAST_MESSAGE_TIME);
         *  _seqMessageOff.Append(canvasGroupToastMessage.DOFade(0, FADE_TOAST_MESSAGE_TIME).SetEase(상동)).SetUpdate(true);
         *  _seqMessageOff.OnComplete(() =>
         *  {
         *   objToastMessage.SetActive(false);
         *   textToastMessage.text = string.Empty;
         *  });
         * });
         */


        yield return new WaitForSecondsRealtime(SHOW_TOAST_MESSAGE_TIME);

        objToastMessage.SetActive(false);
        textToastMessage.text = string.Empty;
    }
    #endregion Toast Message



    #region Notice
    public void Show(string msg)
    {
        noticeViewer.Show(msg);
    }
    #endregion Notice



    #region Indicator
    private int _indicatorCount = 0;
    public bool IsIndicatorOn => objIndicator.activeInHierarchy;

    public void IndicatorOn(bool isIconOn)
    {
        objIndicator.SetActive(true);
        objIndicatorIcon.SetActive(isIconOn);

        ++_indicatorCount;

        Utils.Log($"[SystemUIManager] IndicatorOn : {_indicatorCount}.");
    }

    public void IndicatorOff()
    {
        objIndicator.SetActive(false);
        objIndicatorIcon.SetActive(false);

        --_indicatorCount;

        Utils.Log($"[SystemUIManager] IndicatorOff : {_indicatorCount}");
    }
    #endregion Indicator
}
