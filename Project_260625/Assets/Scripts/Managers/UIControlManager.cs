using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInfo
{
    public readonly eScene scene;
    public readonly Scene unityScene;
    public readonly WindowScene windowScene;

    public SceneInfo(eScene scene, Scene unityScene)
    {
        this.scene = scene;
        this.unityScene = unityScene;

        foreach (GameObject root in unityScene.GetRootGameObjects())
        {
            windowScene = root.GetComponentInChildren<WindowScene>(true);

            if (windowScene != null)
                break;
        }
    }

    public bool IsValid() => scene != eScene.None && unityScene.IsValid() && windowScene != null;
}

public class UIControlManager : SingletonBehaviour<UIControlManager>, IManager
{
    #region Event
    private Action<WindowScene> _onChangeScene;
    public event Action<WindowScene> OnChangeScene
    {
        add
        {
            _onChangeScene -= value;
            _onChangeScene += value;
        }
        remove
        {
            _onChangeScene -= value;
        }
    }
    #endregion Event

    private readonly List<eScene> _history = new();

    private bool _isSwitching = false;

    public SceneInfo CurrentSceneInfo { get; private set; } = null;

    /////////////////////////////////////////////////

    protected override void OnDestroy()
    {
        if (InputManager.Instance)
            InputManager.Instance.OnBackPress -= BackPress;

        base.OnDestroy();
    }

    public override void Init()
    {
        base.Init();

        InputManager.Instance.OnBackPress += BackPress;
    }

    /////////////////////////////////////////////////

    #region History
    public void PushHistory(eScene scene)
    {
        if (scene == eScene.None)
            return;

        _history.Remove(scene);
        _history.Add(scene);
    }

    public eScene PopHistory()
    {
        if (_history.Count == 0)
            return eScene.None;

        eScene scene = _history[^1];
        _history.RemoveAt(_history.Count - 1);
        return scene;
    }

    public void ClearHistory() => _history.Clear();
    #endregion History



    #region BackPress
    private bool BackPress()
    {
        // 인디케이터 / 전환 중이면 무시, 팝업이 있으면 팝업이 먼저 처리.
        if (SystemUIManager.Instance.IsIndicatorOn || _isSwitching || !PopupManager.Instance.BackPress())
            return true;

        if (CurrentSceneInfo != null && CurrentSceneInfo.windowScene.BackPress())
        {
            eScene scene = PopHistory();

            GoWindowScene(scene != eScene.None ? scene : eScene.Home, isHistory: false);
        }

        return true;
    }
    #endregion BackPress



    public void GoWindowScene(eScene scene, bool isHistory, bool isShowLoading = true, Action<bool> onFinished = null, Action onClose = null, WindowArgs args = null)
    {
        if (CurrentSceneInfo != null && CurrentSceneInfo.scene == scene)
        {
            onFinished?.Invoke(true);
            return;
        }

        if (_isSwitching)
        {
            onFinished?.Invoke(false);
            return;
        }

        _isSwitching = true;

        SystemUIManager.Instance.IndicatorOn(isIconOn: false);
        SystemUIManager.Instance.FadeOut(() =>
        {
            CloseCurrentScene(isHistory);

            SceneControlManager.Instance.LoadScene(scene, isShowLoading, (loadResult, sceneInstance) =>
            {
                SceneInfo sceneInfo = loadResult ? new SceneInfo(scene, sceneInstance.Scene) : null;

                if (sceneInfo == null || !sceneInfo.IsValid())
                {
                    Finish(false);
                    return;
                }

                OpenScene(sceneInfo, onClose, args, Finish);
            });
        });

        void Finish(bool result)
        {
            _isSwitching = false;

            onFinished?.Invoke(result);

            SystemUIManager.Instance.FadeIn(SystemUIManager.Instance.IndicatorOff);
        }
    }

    #region GoWindow
    private void CloseCurrentScene(bool isHistory)
    {
        if (CurrentSceneInfo == null)
            return;

        if (isHistory)
            PushHistory(CurrentSceneInfo.scene);

        if (CurrentSceneInfo.windowScene != null)
            CurrentSceneInfo.windowScene.Close();

        CurrentSceneInfo = null;
    }

    private void OpenScene(SceneInfo sceneInfo, Action onClose, WindowArgs args, Action<bool> onFinished)
    {
        WindowScene windowScene = sceneInfo.windowScene;

        // Open 후 다시 활성화될 때 OnEnable -> OpenProcess 가 실행되도록 꺼둔다.
        windowScene.gameObject.SetActive(false);

        windowScene.Open((result) =>
        {
            if (result)
            {
                CurrentSceneInfo = sceneInfo;

                windowScene.gameObject.SetActive(true);

                _onChangeScene?.Invoke(windowScene);
            }
            else
                windowScene.SetOnOffGroup(false);

            onFinished?.Invoke(result);
        }, onClose, args);
    }
    #endregion GoWindow
}
