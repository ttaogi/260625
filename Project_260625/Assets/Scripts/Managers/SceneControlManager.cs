using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class SceneControlManager : SingletonBehaviour<SceneControlManager>, IManager
{
    private eScene _curScene = eScene.None;
    public eScene CurScene
    {
        get { return _curScene; }
    }

    private Queue<eScene> _scenes = new();
    private bool _isLoading = false;

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
        }
    }

    /////////////////////////////////////////////////////

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }

    private void OnSceneUnloaded(Scene scene)
    {

    }

    public void LoadScene(eScene scene)
    {
        if (_isLoading)
            return;

        StartCoroutine(CoLoadScene(scene, LoadSceneMode.Single));
    }

    #region LoadScene
    private IEnumerator CoLoadScene(eScene scene, LoadSceneMode mode)
    {
        yield return Addressables.LoadSceneAsync(scene.ToString(), mode);

        yield break;
    }
    #endregion LoadScene
}
