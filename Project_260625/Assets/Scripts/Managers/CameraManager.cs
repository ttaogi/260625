using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : SingletonBehaviour<CameraManager>, IManager
{
    /// <summary> 카메라가 추적하는 대상. </summary>
    private GameObject _target;
    private Vector3 _targetPos;

    private Camera _camera;

    private float _moveSpeed = 3.0f;

    //////////////////////////////////////////

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
        _camera = Utils.GetMainCamera();
    }
    #endregion

    //////////////////////////////////////////

    private void Update()
    {
        if (_target == null || _camera == null) return;

        _targetPos.Set(_target.transform.position.x, _target.transform.position.y, _camera.transform.position.z);
        _camera.transform.position = Vector3.Lerp(_camera.transform.position, _targetPos, Mathf.Clamp01(_moveSpeed * Utils.DeltaTime));
    }

    public void SetTarget(GameObject target)
    {
        _target = target;
    }
}
