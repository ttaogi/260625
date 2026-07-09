using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraSetter : MonoBehaviour
{
    private Canvas _canvas = null;

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

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        SetCanvasCameraLate();
    }

    public void SetCanvasCamera(WindowScene windowScene)
    {
        var camera = Utils.GetCamera(eLayer.UI);
        _canvas.worldCamera = camera;
    }

    public void SetCanvasCameraLate()
    {
        StartCoroutine(CoSet());
        IEnumerator CoSet()
        {
            yield return null;
            var camera = Utils.GetCamera(eLayer.UI);
            _canvas.worldCamera = camera;
        }
    }
}
