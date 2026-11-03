using UnityEngine;
using System;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

/// <summary> 씬 로드 전담. 항상 Single 모드로 로드한다. (이전 씬은 Addressables가 자동으로 언로드/해제) </summary>
public class SceneControlManager : SingletonBehaviour<SceneControlManager>, IManager
{
    private const string SCENE_PATH_FORMAT = "Assets/Scenes/{0}.unity";

    public bool IsLoading { get; private set; } = false;
    public eScene CurScene { get; private set; }

    /////////////////////////////////////////////////////

    public override void Init()
    {
        base.Init();

        IsLoading = false;
        CurScene = eScene.Title;
    }

    /////////////////////////////////////////////////////

    public void LoadScene(eScene scene, bool isShowLoading = true, Action<bool, SceneInstance> onFinished = null)
    {
        Utils.Log($"[SceneControlManager] LoadScene : {scene}");

        if (IsLoading)
        {
            Utils.Log($"[SceneControlManager] LoadScene : Already Loading.");
            onFinished?.Invoke(false, default);
            return;
        }

        StartCoroutine(CoLoadScene(scene, isShowLoading, onFinished));
    }

    private IEnumerator CoLoadScene(eScene scene, bool isShowLoading, Action<bool, SceneInstance> onFinished)
    {
        IsLoading = true;

        PopupManager.Instance.CloseAll();

        if (isShowLoading)
            SystemUIManager.Instance.LoadingOn();

        AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(string.Format(SCENE_PATH_FORMAT, scene), LoadSceneMode.Single);

        while (!handle.IsDone)
        {
            if (isShowLoading)
                SystemUIManager.Instance.SetLoading(handle.PercentComplete);

            yield return null;
        }

        if (isShowLoading)
            SystemUIManager.Instance.LoadingOff();

        IsLoading = false;
        CurScene = scene;

        bool result = handle.Status == AsyncOperationStatus.Succeeded;

        onFinished?.Invoke(result, result ? handle.Result : default);
    }
}
