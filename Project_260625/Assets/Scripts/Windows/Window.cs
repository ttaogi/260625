using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public abstract class WindowArgs { }

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(GraphicRaycaster))]
public abstract class Window : MonoBehaviour
{
    #region Event
    protected Action _onClose;
    public event Action OnClose
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
    #endregion Inspector

    private readonly List<Canvas> _canvases = new();

    public Canvas Canvas { get; private set; } = null;
    public int baseOrder = 0;
    public int minOrder = 0;
    public int maxOrder = 0;

    public bool IsValid { get; protected set; } = false;

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

    #region Open
    protected virtual void OpenProcess() { }
    #endregion Open



    public void Close()
    {
        CloseProcess();
    }

    #region Close
    protected virtual void CloseProcess()
    {
        UpdateCanvasOrder(0);

        gameObject.SetActive(false);

        IsValid = false;

        _onClose?.Invoke();
        _onClose = null;
    }
    #endregion Close



    #region Order
    public void SetCanvasOrder()
    {
        Canvas[] canvases = GetComponentsInChildren<Canvas>(true);

        _canvases.Clear();
        _canvases.AddRange(canvases);

        if (_canvases.Count > 0)
        {
            minOrder = _canvases[0].sortingOrder;
            maxOrder = _canvases[0].sortingOrder;

            for (int i = 0; i < _canvases.Count; ++i)
            {
                if (minOrder > _canvases[i].sortingOrder)
                    minOrder = _canvases[i].sortingOrder;

                if (maxOrder < _canvases[i].sortingOrder)
                    maxOrder = _canvases[i].sortingOrder;
            }
        }
    }

    public void SortCanvasOrder()
    {
        SetCanvasOrder();

        Canvas parentCanvas = null;

        if (gameObject.transform.parent != null)
            parentCanvas = gameObject.transform.parent.GetComponentInParent<Canvas>();

        if (parentCanvas != null)
            baseOrder = parentCanvas.sortingOrder + 1;
        else
            baseOrder = 0;

        for (int i = 0; i < _canvases.Count; ++i)
            _canvases[i].sortingOrder = baseOrder + i;

        SetCanvasOrder();
    }

    public void UpdateCanvasOrder(int newBaseOrder)
    {
        if (_canvases.Count > 0 && newBaseOrder >= 0)
        {
            baseOrder = _canvases[0].sortingOrder;

            int diffOrder = newBaseOrder - baseOrder;

            for (int i = 0; i < _canvases.Count; ++i)
                _canvases[i].sortingOrder += diffOrder;
        }

        SetCanvasOrder();
    }
    #endregion Order



    public bool BackPress()
    {
        return BackPressProcess();
    }

    #region BackPress
    protected abstract bool BackPressProcess();
    #endregion BackPress
}
