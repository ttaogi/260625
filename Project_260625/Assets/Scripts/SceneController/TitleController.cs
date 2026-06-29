using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleController : MonoBehaviour
{
    #region Inspector
    public Button btnEnter;
    #endregion Inspector

    /////////////////////////////////////////////////////
    ///

    protected void Awake()
    {
        btnEnter.SetOnClickEvent(() =>
        {
            SceneControlManager.Instance.LoadScene(eScene.Home, LoadSceneMode.Single, onFinished: (result, sceneInstance) =>
            {
                Utils.Log("load home.");
            });
        });
    }
}
