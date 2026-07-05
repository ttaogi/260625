using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    #region Inspector
    public Button btnEnter;
    #endregion Inspector

    /////////////////////////////////////////////////////

    protected void Awake()
    {
        btnEnter.SetOnClickEvent(() =>
        {
            UIControlManager.Instance.GoWindowScene(eScene.Home, isHistory: false, onFinished: (result) =>
            {
                Utils.Log($"GoWindowScene : Home, Result : {result}");
            });
        });
    }
}
