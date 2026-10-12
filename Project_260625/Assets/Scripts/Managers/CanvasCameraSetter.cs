using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasCameraSetter : MonoBehaviour
{
    private Canvas _canvas = null;
    public Camera Camera { get; }

    //////////////////////////////////////

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }

    private void OnEnable()
    {
        SetCanvasCameraLate();
        UIControlManager.Instance.OnChangeScene += SetCanvasCamera;
        SceneControlManager.Instance.OnChangeScene += SetCanvasCameraLate;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UIControlManager.Instance.OnChangeScene -= SetCanvasCamera;
        SceneControlManager.Instance.OnChangeScene -= SetCanvasCameraLate;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #region
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        SearchAndSetCamera();
    }

    private void SetCanvasCamera(WindowScene windowScene)
    {
        SearchAndSetCamera();
    }

    private void SetCanvasCameraLate()
    {
        StartCoroutine(CoSet());
        IEnumerator CoSet()
        {
            yield return null;
            SearchAndSetCamera();
        }
    }

    private void SearchAndSetCamera()
    {
        var camera = Utils.GetCamera(eLayer.UI);
        _canvas.worldCamera = camera;
    }
    #endregion
}
