using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoticeViewer : MonoBehaviour
{
    #region Inspector
    public RectTransform rectNotice;
    public TMP_Text textNotice;
    #endregion Inspector

    private const float MOVE_TIME = 0.2f;
    private const float VIEW_TIME = 1.0f;

    private readonly Queue<string> _queueMsg = new();
    private Coroutine _coShowNotice = null;
    private Sequence _seqNoticeOn = null;
    private Sequence _seqNoticeOff = null;

    /////////////////////////////////////////////////

    public void Init()
    {
        rectNotice.anchoredPosition = new(0, rectNotice.rect.height / 2);
        textNotice.text = string.Empty;

        _queueMsg.Clear();

        _coShowNotice = null;
        _seqNoticeOn = null;
        _seqNoticeOff = null;
    }

    public void ShowNotice(string msg)
    {
        lock(_queueMsg)
        {
            _queueMsg.Enqueue(msg);
        }

        if (_coShowNotice == null)
            _coShowNotice = StartCoroutine(CoShowNotice());
    }

    private IEnumerator CoShowNotice()
    {
        float heightHalf = rectNotice.rect.height / 2;

        while (_queueMsg.Count > 0)
        {
            string msg = string.Empty;
            
            lock(_queueMsg)
            {
                msg = _queueMsg.Dequeue();
            }

            if (string.IsNullOrEmpty(msg))
                continue;

            textNotice.text = msg;

            // sequence.
            _seqNoticeOn?.Kill();
            _seqNoticeOff?.Kill();

            _seqNoticeOn = DOTween.Sequence();
            _seqNoticeOn.timeScale = 1f;
            _seqNoticeOn.Append(rectNotice.DOAnchorPosY(-heightHalf, MOVE_TIME)).SetEase(Ease.Linear).SetUpdate(true);
            _seqNoticeOn.OnComplete(() =>
            {
                _seqNoticeOff = DOTween.Sequence();
                _seqNoticeOff.timeScale = 1f;
                _seqNoticeOff.SetDelay(VIEW_TIME);
                _seqNoticeOff.Append(rectNotice.DOAnchorPosY(heightHalf, MOVE_TIME).SetEase(Ease.Linear)).SetUpdate(true);
                _seqNoticeOff.OnComplete(() =>
                {
                    textNotice.text = string.Empty;
                });
            });
        }

        _coShowNotice = null;

        yield break;
    }
}
