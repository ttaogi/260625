using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public abstract class WindowArgs { }

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(GraphicRaycaster))]
public class Window : MonoBehaviour
{
    public class CanvasInfo
    {
        public Canvas canvas;
        public int order;
    }

    /////////////////////////////////////////////

    #region Event
    public Action _onClose;
    private event Action OnClose
    {
        add
        {
            _onClose -= value;
            _onClose += value;
        }
        remove
        {
            _onClose -= value;
        }
    }
    #endregion Event

    #region Inspector
    public EventTrigger blockTouch;
    #endregion Inspector

    public Canvas Canvas { get; private set; } = null;
    public int baseOrder = 0;
    public int minOrder = 0;
    public int maxOrder = 0;

    public bool IsValid { get; set; } = false;

    public eScene scene = eScene.None;

    /////////////////////////////////////////////

    protected virtual void Awake()
    {
        Canvas = GetComponent<Canvas>();
    }

    protected virtual void OnEnable()
    {
        if (IsValid)
            OpenProcess();
    }

    protected virtual void OnDisable() { }

    /////////////////////////////////////////////

    public void Open(Action<bool> onFinished, Action onClosed = null, WindowArgs args = null)
    {
        OpenCheck((result) =>
        {
            Utils.Log($"[Window] Open[{gameObject.name}] result : {result}");

            if (result)
            {
                IsValid = true;

                if (onClosed != null)
                    OnClose += onClosed;
            }

            onFinished?.Invoke(result);
        }, args);
    }

    #region Open
    protected virtual void OpenCheck(Action<bool> onCheck, WindowArgs args = null)
    {
        onCheck?.Invoke(true);
    }

    protected virtual void OpenProcess() { }
    #endregion Open



    public void Close()
    {
        CloseProcess();
    }

    #region Close
    protected virtual void CloseProcess()
    {
        // order 정리 필요.

        gameObject.SetActive(false);

        IsValid = false;

        _onClose?.Invoke();
        _onClose = null;
    }
    #endregion Close



    public bool BackPress()
    {
        return BackPressProcess();
    }

    #region BackPress
    protected virtual bool BackPressProcess()
    {
        Close();

        return true;
    }
    #endregion BackPress
}
