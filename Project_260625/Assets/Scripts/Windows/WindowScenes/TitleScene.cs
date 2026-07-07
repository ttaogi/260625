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
            SystemUIManager.Instance.FadeOut(() =>
            {
                SceneControlManager.Instance.UnloadScene(eScene.Title, () =>
                {
                    UIControlManager.Instance.GoWindowScene(eScene.Home, isHistory: false, isShowLoading: false, onFinished: (result) =>
                    {
                        Utils.Log($"[TitleScene] GoWindowScene : Home, Result : {result}");
                        SystemUIManager.Instance.FadeIn();
                    });
                });
            });
        });
    }
}
