using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SystemUIManager : SingletonBehaviour<SystemUIManager>, IManager
{
    #region Inspector
    [Header("Tooltip.")]
    public TooltipViewer tooltipViewer;

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

    /////////////////////////////////////////////////

    public override void Init()
    {
        base.Init();

        // Tooltip.
        tooltipViewer.Init();

        // Fade In/Out.

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
        objIndicator.SetActive(false);
    }



    #region Tooltip
    public void ShowTooltip(Transform target, string title, string desc)
    {
        tooltipViewer.ShowTooltip(target, title, desc);
    }

    public void HideTooltip()
    {
        tooltipViewer.HideTooltip();
    }


    public GameObject objTest;
    public string title;
    public string desc;
    [ContextMenu("TEST")]
    public void Test()
    {
        ShowTooltip(objTest.transform, title, desc);
    }

    [ContextMenu("Test convert")]
    public void TT()
    {
        tooltipViewer.TT(objTest.transform);
    }
    #endregion Tooltip



    #region Fade In/Out
    private Coroutine _coFadeIn = null;
    private Coroutine _coFadeOut = null;
    private const float FadeTime = 0.3f;

    public bool IsFadeOut { get; private set; } = false;

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
        Utils.Log($"[SystemUIManager] SetLoading : {ration}");
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

    private Coroutine _coShowToastMessage = null;
    Sequence _seqToastMessageOn = null;
    Sequence _seqToastMessageOff = null;

    public void ShowToastMessage(string msg)
    {
        if (_coShowToastMessage != null)
        {
            StopCoroutine(_coShowToastMessage);
            _coShowToastMessage = null;
        }

        _coShowToastMessage = StartCoroutine(CoShowToastMessage(msg));
    }

    private IEnumerator CoShowToastMessage(string msg)
    {
        canvasGroupToastMessage.alpha = 0f;
        objToastMessage.SetActive(true);
        textToastMessage.text = msg;

        yield return null;

        rectToastMessageBackground.sizeDelta = new(textToastMessage.rectTransform.rect.width,
                                                   textToastMessage.rectTransform.rect.height);

        _seqToastMessageOn?.Kill();
        _seqToastMessageOff?.Kill();

        _seqToastMessageOn = DOTween.Sequence();
        _seqToastMessageOn.timeScale = 1f;
        _seqToastMessageOn.Append(canvasGroupToastMessage.DOFade(1, FADE_TOAST_MESSAGE_TIME).SetEase(Ease.OutQuart)).SetUpdate(true);
        _seqToastMessageOn.OnComplete(() =>
        {
            _seqToastMessageOff = DOTween.Sequence();
            _seqToastMessageOff.timeScale = 1f;
            _seqToastMessageOff.SetDelay(SHOW_TOAST_MESSAGE_TIME);
            _seqToastMessageOff.Append(canvasGroupToastMessage.DOFade(0, FADE_TOAST_MESSAGE_TIME).SetEase(Ease.InQuart)).SetUpdate(true);
            _seqToastMessageOff.OnComplete(() =>
            {
                objToastMessage.SetActive(false);
                textToastMessage.text = string.Empty;

                _coShowToastMessage = null;
            });
        });
    }
    #endregion Toast Message



    #region Notice
    public void ShowNotice(string msg)
    {
        noticeViewer.ShowNotice(msg);
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

        Utils.Log($"[SystemUIManager] IndicatorOn : {_indicatorCount}");
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
