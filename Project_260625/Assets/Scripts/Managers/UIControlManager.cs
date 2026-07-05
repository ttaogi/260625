using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInfo
{
    public eScene scene;
    public Scene unityScene;
    public WindowScene windowScene;

    public SceneInfo(Scene unityScene)
    {
        // scene.
        if (Enum.TryParse(unityScene.name, true, out eScene parsedScene))
            this.scene = parsedScene;

        // unity scene.
        this.unityScene = unityScene;

        // windowScene.
        GameObject[] getRootGameObjects = unityScene.GetRootGameObjects();

        if (getRootGameObjects != null)
            for (int i = 0; i < getRootGameObjects.Length; i++)
            {
                WindowScene windowScene = getRootGameObjects[i].GetComponentInChildren<WindowScene>(true);

                if (windowScene != null)
                {
                    this.windowScene = windowScene;
                    break;
                }
            }
    }

    public bool IsValid() => scene != eScene.None && unityScene != null && windowScene != null;
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
    private readonly List<SceneInfo> _sceneInfos = new();

    private WindowScene _target = null;
    private Action<bool> _onFinishedSwitch = null;
    private bool _isSwitching = false;

    //private bool _isLoading = false;


    public SceneInfo CurrentSceneInfo { get; private set; } = null;



    /////////////////////////////////////////////////

    protected override void OnDestroy()
    {
        if (SceneControlManager.Instance)
            SceneControlManager.Instance.OnChangeScenePre -= Clear;
        if (InputManager.Instance)
            InputManager.Instance.OnBackPress -= BackPress;

        base.OnDestroy();
    }

    public override void Init()
    {
        base.Init();

        InputManager.Instance.OnBackPress += BackPress;
        SceneControlManager.Instance.OnChangeScenePre += Clear;
    }

    /////////////////////////////////////////////////

    private void Clear()
    {
        ClearHistory();

        _isSwitching = false;

        CurrentSceneInfo = null;
    }



    #region History
    public void PushHistory(eScene scene)
    {
        for (int i = 0; i < _history.Count; ++i)
            if (_history[i] == scene)
            {
                _history.RemoveAt(i);
                break;
            }

        _history.Add(scene);
    }

    public eScene PopHistory()
    {
        if (_history.Count > 0)
        {
            eScene scene = _history[^1];
            _history.RemoveAt(_history.Count - 1);
            return scene;
        }
        else
            return eScene.None;
    }

    public void ClearHistory()
    {
        _history.Clear();
    }
    #endregion History



    public bool BackPress()
    {
        if (IsBackPressValid())
            BackPressProcess();

        return true;
    }

    #region BackPress
    public bool IsBackPressValid()
    {
        //터막 체크.

        if (_isSwitching)
            return false;

        //팝업 체크.
        if (!PopupManager.Instance.BackPress())
            return false;

        return true;
    }

    private void BackPressProcess()
    {
        if (CurrentSceneInfo == null)
            return;

        WindowScene windowScene = CurrentSceneInfo.windowScene;

        if (windowScene == null)
            return;

        if (windowScene.BackPress())
        {
            eScene scene = PopHistory();

            if (scene != eScene.None)
                GoWindowScene(scene, false);
            else
                GoWindowScene(eScene.Home, false);
        }
    }
    #endregion BackPress



    public void GoWindowScene(eScene scene, bool isHistory, Action<bool> onFinished = null, Action onClose = null, WindowArgs args = null)
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

        SystemUIManager.Instance.FadeOut();
        //터막.

        LoadWindowScene(scene, (loadResult, sceneInfo) =>
        {
            if (loadResult && sceneInfo != null && sceneInfo.IsValid())
            {
                SwitchWindow(sceneInfo.windowScene, (switchResult) =>
                {
                    if (switchResult)
                    {
                        if (isHistory)
                            PushHistory(scene);

                        CurrentSceneInfo = sceneInfo;

                        SceneManager.SetActiveScene(CurrentSceneInfo.unityScene);
                    }

                    _isSwitching = false;

                    onFinished?.Invoke(switchResult);

                    //터막 해제.
                    SystemUIManager.Instance.FadeIn();
                }, onClose, args);
            }
            else
            {
                _isSwitching = false;

                onFinished?.Invoke(false);

                //터막 해제.
                SystemUIManager.Instance.FadeIn();
            }
        });
    }

    #region GoWindow
    private void LoadWindowScene(eScene scene, Action<bool, SceneInfo> onFinished = null)
    {
        SceneInfo sceneInfo = _sceneInfos.Find(x => x.scene == scene);

        if (sceneInfo != null)
        {
            onFinished?.Invoke(true, sceneInfo);
            return;
        }

        SceneControlManager.Instance.LoadScene(scene, LoadSceneMode.Single, (result, sceneInstance) =>
        {
            if (result)
            {
                sceneInfo = new(sceneInstance.Scene);

                if (sceneInfo.windowScene != null)
                    sceneInfo.windowScene.gameObject.SetActive(false);

                if (sceneInfo.IsValid())
                {
                    _sceneInfos.Add(sceneInfo);
                    onFinished?.Invoke(true, sceneInfo);
                }
                else
                    onFinished?.Invoke(false, null);
            }
            else
                onFinished?.Invoke(false, null);
        });
    }

    private void SwitchWindow(WindowScene target, Action<bool> onFinishedSwitch = null, Action onClose = null, WindowArgs args = null)
    {
        _target = target;
        _onFinishedSwitch = onFinishedSwitch;

        target.Open(OnFinishedOpen, onClose, args);
    }

    private void OnFinishedOpen(bool result)
    {
        StartCoroutine(CoOnFinishedOpen(result));
    }

    private IEnumerator CoOnFinishedOpen(bool result)
    {
        if (result)
        {
            if (CurrentSceneInfo != null)
            {
                CurrentSceneInfo.windowScene.OnClose += () =>
                {
                    _target.gameObject.SetActive(true);
                    _onChangeScene?.Invoke(_target);
                    CurrentSceneInfo = null;
                };

                CurrentSceneInfo.windowScene.Close();

                yield return new WaitUntil(() => _target.gameObject.activeSelf);
            }
            else
            {
                _target.gameObject.SetActive(true);
                _onChangeScene?.Invoke(_target);
                CurrentSceneInfo = null;
            }
        }
        else
        {
            _target.SetOnOffGroup(false);
        }

        _onFinishedSwitch?.Invoke(result);
    }
    #endregion GoWindow

}
