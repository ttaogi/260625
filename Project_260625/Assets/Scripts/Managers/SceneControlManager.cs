using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneControlManager : SingletonBehaviour<SceneControlManager>, IManager
{
    #region Event
    private Action _onChangeScene;
    public event Action OnChangeScene
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

    private readonly Queue<eScene> _scenes = new();
    private readonly List<AsyncOperationHandle<SceneInstance>> _handles = new();

    public eScene PreScene { get; private set; } = eScene.None;
    public eScene CurScene { get; private set; } = eScene.None;
    public eScene CurLoadingScene { get; private set; } = eScene.None;
    public bool IsLoading { get; private set; } = false;
    public float LoadingRation { get; private set; } = 0f;

    /////////////////////////////////////////////////////

    protected override void OnDestroy()
    {
        if (IsMyInstance())
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        base.OnDestroy();
    }

    public override void Init()
    {
        base.Init();

        if (IsMyInstance())
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            CurScene = eScene.Title;
        }
    }

    #region
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Utils.Log($"[SceneControlManager] OnSceneLoaded : {scene.name}, {mode}");
    }

    private void OnSceneUnloaded(Scene scene)
    {
        Utils.Log($"[SceneControlManager] OnSceneUnloaded : {scene.name}");
    }
    #endregion

    /////////////////////////////////////////////////////

    public void LoadScene(eScene scene, LoadSceneMode loadSceneMode, Action<bool, SceneInstance> onFinished = null)
    {
        if (IsLoading)
            return;

        IsLoading = true;

        StartCoroutine(CoLoadScene(scene, loadSceneMode, onFinished));
    }

    #region LoadScene
    private IEnumerator CoLoadScene(eScene scene, LoadSceneMode loadSceneMode, Action<bool, SceneInstance> onFinished = null)
    {
        string sceneName = scene.ToString();

        // 전.
        if (loadSceneMode == LoadSceneMode.Single)
            yield return StartCoroutine(ReleaseHandles());

        CurLoadingScene = scene;

        // 로딩.
        AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync($"Assets/Scenes/{sceneName}.unity", loadSceneMode);

        while (!handle.IsDone)
        {
            LoadingRation = handle.PercentComplete;
            yield return null;
        }

        yield return new WaitUntil(() => handle.Status != AsyncOperationStatus.None);

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            lock (_handles)
            {
                _handles.Add(handle);
            }
        }

        yield return new WaitForEndOfFrame();

        // 후.
        CurLoadingScene = eScene.None;
        IsLoading = false;

        bool result = handle.Status == AsyncOperationStatus.Succeeded;

        if (result)
        {
            PreScene = CurScene;
            CurScene = scene;
        }

        _onChangeScene?.Invoke();
        onFinished?.Invoke(result, handle.Result);

        yield break;
    }

    private IEnumerator ReleaseHandles()
    {
        lock (_handles)
        {
            for (int i = _handles.Count - 1; i >= 0; --i)
            {
                AsyncOperationHandle<SceneInstance> handle = _handles[i];

                if (handle.IsValid())
                {
                    AsyncOperationHandle<SceneInstance> unloadHandle = Addressables.UnloadSceneAsync(handle);
                    yield return unloadHandle;
                }

                _handles.RemoveAt(i);
            }
        }
    }
    #endregion LoadScene



    public void UnloadScene(eScene scene, Action onFinished = null)
    {
        StartCoroutine(CoUnloadScene(scene, onFinished));
    }

    #region UnloadScene
    private IEnumerator CoUnloadScene(eScene scene, Action onFinished = null)
    {
        lock (_handles)
        {
            AsyncOperationHandle<SceneInstance> handle = _handles.Find(x =>
            {
                if (x.IsValid())
                    return x.Result.Scene.name == scene.ToString();
                else
                    return false;
            });

            if (handle.IsValid())
            {
                AsyncOperationHandle<SceneInstance> unloadHandle = Addressables.UnloadSceneAsync(handle, UnloadSceneOptions.None);

                yield return unloadHandle;

                if (unloadHandle.IsValid())
                    Addressables.Release(unloadHandle);
            }

            _handles.Remove(handle);

            onFinished?.Invoke();
        }
    }
    #endregion UnloadScene



    public bool IsLoadedScene(eScene scene) => IsLoadedScene(scene.ToString());

    public bool IsLoadedScene(string sceneName)
    {
        lock (_handles)
        {
            if (_handles.Exists(x => x.IsValid() && x.Result.Scene.name == sceneName))
                return true;
            else if (CurLoadingScene != eScene.None)
                return CurLoadingScene.ToString() == sceneName;
        }

        return false;
    }
}


