using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SystemUIManager : SingletonBehaviour<SystemUIManager>, IManager
{
    #region Inspector
    public Image imgFade;
    #endregion Inspector

    #region FadeIn/Out
    private Coroutine _coFadeIn = null;
    private Coroutine _coFadeOut = null;
    private const float FadeTime = 0.3f;

    public void FadeIn()
    {
        if (_coFadeIn != null)
            StopCoroutine(_coFadeIn);

        _coFadeIn = StartCoroutine(CoFadeIn());
    }

    private IEnumerator CoFadeIn()
    {
        float time = 0f;
        Color color = imgFade.color;

        imgFade.gameObject.SetActive(true);

        while (true)
        {
            time += Time.deltaTime;

            if (time > FadeTime)
                break;

            color.a = Mathf.Max(FadeTime - time, 0f) / FadeTime;
            imgFade.color = color;
        }

        _coFadeOut = null;

        yield return null;
    }

    public void FadeOut()
    {
        if (_coFadeOut != null)
            StopCoroutine(_coFadeOut);

        _coFadeOut = StartCoroutine(CoFadeOut());
    }

    private IEnumerator CoFadeOut()
    {
        float time = 0f;
        Color color = imgFade.color;

        imgFade.gameObject.SetActive(true);

        while(true)
        {
            time += Time.deltaTime;

            if (time > FadeTime)
                break;

            color.a = time / FadeTime;
            imgFade.color = color;
        }

        _coFadeOut = null;

        yield return null;
    }

    #endregion FadeIn/Out
}
