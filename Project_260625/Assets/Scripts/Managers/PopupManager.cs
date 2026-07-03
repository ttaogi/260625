using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PopupInfo
{
    private static int COUNT = 10;
    private int _count;

    public ePopup popup;
    public WindowPopup windowPopup;
    public AsyncOperationHandle<GameObject> handle;

    public int MaxOrder => windowPopup ? windowPopup.maxOrder : 0;
    public bool IsOpen => windowPopup ? windowPopup.gameObject.activeInHierarchy : false;

    public PopupInfo(ePopup popup, AsyncOperationHandle<GameObject> handle)
    {
        this.popup = popup;
        this.handle = handle;
        windowPopup = handle.Result.GetComponent<WindowPopup>();
        _count = COUNT;
    }

    public void CountDown()
    {
        --_count;
    }

    public bool IsValidCount()
    {
        return _count > 0;
    }

    public void ResetCount()
    {
        _count = COUNT;
    }
}

public class PopupManager : SingletonBehaviour<PopupManager>, IManager
{
    #region Event
    private Action _onOpen;
    public event Action OnOpen
    {
        add
        {
            _onOpen -= value;
            _onOpen += value;
        }
        remove
        {
            _onOpen -= value;
        }
    }

    private Action _onClose;
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
    public PopupManagerPopup popupManagerPopup;
    public Transform tranInactivePopupParent;
    #endregion Inspector

    private readonly List<PopupInfo> _popupInfos = new();
    private readonly List<WindowPopup> _openPopups = new();

    public bool IsOpenPopup => _openPopups.Count > 0;
    public int MaxOrder
    {
        get
        {
            int maxOrder = popupManagerPopup.maxOrder;

            for (int i = 0; i < _openPopups.Count; ++i)
                if (maxOrder < _openPopups[i].maxOrder)
                    maxOrder = _openPopups[i].maxOrder;

            return maxOrder;
        }
    }

    ////////////////////////////////////////////

    public T OpenPopup<T>(ePopup popup, Action<bool> onOpen = null, Action onClose = null,
                            eLayer layer = eLayer.UI, WindowArgs args = null)
                            where T : WindowPopup
    {
        // 터막.

        WindowPopup windowPopup = null;
        PopupInfo popupInfo = _popupInfos.Find(x => x.popup == popup && !x.IsOpen);

        if (popupInfo != null)
        {
            windowPopup = popupInfo.windowPopup;
            popupInfo.ResetCount();
        }
        else
            windowPopup = LoadPopup(popup);

        TrimPopupInfos();

        if (windowPopup != null)
        {
            if (windowPopup.gameObject.layer != (int)layer)
                Utils.ChangeLayer(windowPopup.gameObject, (int)layer, windowPopup.noChangeLayerObjects);

            if (windowPopup.TryGetComponent(out RectTransform rectPopup))
            {
                rectPopup.anchorMin = new Vector2(0, 0);
                rectPopup.anchorMax = new Vector2(1, 1);
                rectPopup.sizeDelta = Vector2.zero;
            }

            windowPopup.Open(popupManagerPopup,
                onFinished: (result) =>
                {
                    if (result)
                    {
                        int order = MaxOrder;

                        if (UIControlManager.IsLive() && UIControlManager.Instance.CurrentSceneInfo.windowScene != null)
                            order += UIControlManager.Instance.CurrentSceneInfo.windowScene.maxOrder;

                        ++order;

                        if (windowPopup.Canvas == null)
                            windowPopup.SetCanvas();

                        windowPopup.Canvas.overrideSorting = true;
                        windowPopup.SortCanvasOrder(order);

                        _openPopups.Add(windowPopup);
                        _onOpen?.Invoke();
                        onOpen?.Invoke(true);

                        // 터막 해제.
                    }
                    else
                    {
                        onOpen?.Invoke(false);

                        // 터막 해제.
                    }
                },
                onClose: () =>
                {
                    _openPopups.Remove(windowPopup);

                    _onClose?.Invoke();
                    onClose?.Invoke();
                },
                args: args
            );

            return windowPopup as T;
        }
        else
        {
            onOpen?.Invoke(false);

            // 터막 해제.

            return null;
        }
    }

    private WindowPopup LoadPopup(ePopup popup)
    {
        string path = $"Assets/Popups/{popup}.prefab";

        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(path);
        handle.WaitForCompletion();

        GameObject go = handle.Result;

        if (go != null)
        {
            WindowPopup windowPopup = go.GetComponent<WindowPopup>();
            windowPopup.gameObject.SetActive(false);
            windowPopup.transform.SetParent(popupManagerPopup.transform);
            windowPopup.transform.localPosition = Vector3.zero;
            windowPopup.transform.localScale = Vector3.one;

            _popupInfos.Add(new(popup, handle));

            return windowPopup;
        }
        else
            return null;
    }

    private void TrimPopupInfos()
    {
        for (int i = _popupInfos.Count - 1; i >= 0; --i)
        {
            _popupInfos[i].CountDown();

            if (_popupInfos[i].IsValidCount() == false)
            {
                if (_popupInfos[i].handle.IsValid())
                    Addressables.ReleaseInstance(_popupInfos[i].handle);

                _popupInfos.RemoveAt(i);
            }
        }
    }

    ////////////////////////////////////////////

    public bool BackPress()
    {
        WindowPopup windowPopup = null;

        if (_openPopups.Count > 0)
            windowPopup = _openPopups[^1];

        if (windowPopup != null)
        {
            windowPopup.BackPress();
            return false;
        }
        else
            return true;
    }

    ////////////////////////////////////////////

    public T GetPopup<T>() where T : WindowPopup
    {
        PopupInfo info = _popupInfos.Find(x => x.windowPopup is T);
        T windowPopup = info != null ? info.windowPopup as T : null;

        return windowPopup;
    }

    public WindowPopup GetPopup(ePopup popup)
    {
        PopupInfo info = _popupInfos.Find(x => x.popup == popup);
        WindowPopup windowPopup = info != null ? info.windowPopup : null;

        return windowPopup;
    }

    public void ClosePopup<T>(Action onClose = null) where T : WindowPopup
    {
        T windowPopup = GetPopup<T>();

        if (windowPopup != null)
        {
            if (onClose != null)
                windowPopup.OnClose += onClose;

            windowPopup.Close();
        }
    }

    public void ClosePopup(ePopup popup, Action onClose = null)
    {
        WindowPopup windowPopup = GetPopup(popup);

        if (windowPopup != null)
        {
            if (onClose != null)
                windowPopup.OnClose += onClose;

            windowPopup.Close();
        }
    }

    public void CloseAll()
    {
        List<WindowPopup> openPopups = new(_openPopups);

        for (int i = 0; i < openPopups.Count; ++i)
            openPopups[i].Close();

        for (int i = _popupInfos.Count - 1; i >= 0; --i)
        {
            if (_popupInfos[i].handle.IsValid())
                Addressables.ReleaseInstance(_popupInfos[i].handle);

            _popupInfos.RemoveAt(i);
        }

        _popupInfos.Clear();
        _openPopups.Clear();
    }

    ////////////////////////////////////////////

}
