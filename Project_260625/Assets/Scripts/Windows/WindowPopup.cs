using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowPopup : Window
{
    #region Inspector
    public EventTrigger blockTouch;
    #endregion Inspector

    private Window _parent = null;

    public GameObject[] noChangeLayerObjects;

    /////////////////////////////////////////////

    protected override void Awake()
    {
        base.Awake();

        if (blockTouch != null)
            blockTouch.SetOnClickEvent(Close);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        // 여는 효과음 추가.
    }

    /////////////////////////////////////////////

    public void Open(Window parent, Action<bool> onFinished = null, Action onClose = null, WindowArgs args = null)
    {
        OpenCheck((result) =>
        {
            Utils.Log($"[WindowPopup] Open[{gameObject.name}] result : {result}");

            if (result)
                OpenWindow(parent, onClose);

            onFinished?.Invoke(result);
        }, args);
    }

    #region Open
    protected void OpenCheck(Action<bool> onCheck, WindowArgs args = null)
    {
        onCheck?.Invoke(true);
    }

    protected void OpenWindow(Window parent, Action onClose = null)
    {
        _parent = parent;

        if (_parent != null)
        {
            IsValid = true;

            if (onClose != null)
                OnClose += onClose;

            SortCanvasOrder();

            gameObject.SetActive(true);

            if (!parent.childrenWindows.Contains(this))
                parent.childrenWindows.Add(this);
        }
    }
    #endregion Open



    #region Close
    protected override void CloseProcess()
    {
        if (_parent != null)
        {
            UpdateCanvasOrder(0);

            _parent.childrenWindows.Remove(this);
            _parent.SetCanvasOrder();
        }

        base.CloseProcess();
    }
    #endregion Close



    #region BackPress
    protected override bool BackPressProcess()
    {
        Close();

        return true;
    }
    #endregion BackPress

}
