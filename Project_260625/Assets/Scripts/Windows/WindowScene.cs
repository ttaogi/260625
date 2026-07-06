using System;
using System.Collections.Generic;
using UnityEngine;

public class WindowScene : Window
{
    #region Inspector

    [Header("씬과 함께 On/Off.")]
    public List<GameObject> objOnOffGroup;
    #endregion Inspector

    /////////////////////////////////////////////

    protected override void Awake()
    {
        base.Awake();

        SetOnOffGroup(false);
    }

    /////////////////////////////////////////////

    public void Open(Action<bool> onFinished, Action onClose = null, WindowArgs args = null)
    {
        OpenCheck((result) =>
        {
            Utils.Log($"[WindowScene] Open[{gameObject.name}] result : {result}");

            if (result)
                OpenWindow(onClose);

            onFinished?.Invoke(result);
        }, args);
    }

    #region Open
    protected void OpenCheck(Action<bool> onCheck, WindowArgs args = null)
    {
        onCheck?.Invoke(true);
    }

    protected void OpenWindow(Action onClose = null)
    {
        IsValid = true;

        if (onClose != null)
            OnClose += onClose;
    }

    protected override void OpenProcess()
    {
        base.OpenProcess();

        SetOnOffGroup(true);
    }
    #endregion Open



    #region Close
    protected override void CloseProcess()
    {
        SystemUIManager.Instance.IndicatorOn();

        // 팝업 종료.
        while (childrenWindows.Count > 0)
            childrenWindows[^1].Close();

        SetOnOffGroup(false);

        base.CloseProcess();

        SystemUIManager.Instance.IndicatorOff();
    }
    #endregion Close



    #region BackPress
    protected override bool BackPressProcess()
    {
        if (IsCanBackPress())
        {
            childrenWindows[^1].BackPress();
            return false;
        }
        else
            return true;
    }

    public bool IsCanBackPress()
    {
        if (childrenWindows.Count > 0)
            return false;
        else
            return true;
    }
    #endregion BackPress

    /////////////////////////////////////////////

    public void SetOnOffGroup(bool isActive)
    {
        if (objOnOffGroup != null)
            for (int i = 0; i < objOnOffGroup.Count; ++i)
                if (objOnOffGroup[i] != null)
                    objOnOffGroup[i].SetActive(isActive);
    }
}
