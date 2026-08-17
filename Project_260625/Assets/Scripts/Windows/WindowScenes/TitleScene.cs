using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    #region Inspector
    public Button btnEnter;
    public TMP_Text textLog;
    #endregion Inspector

    /////////////////////////////////////////////////////

    protected void Awake()
    {
        textLog.text = string.Empty;

        btnEnter.SetOnClickEvent(() =>
        {
            //SystemUIManager.Instance.FadeOut(() =>
            //{
            //    SceneControlManager.Instance.UnloadScene(eScene.Title, () =>
            //    {
            //        UIControlManager.Instance.GoWindowScene(eScene.Home, isHistory: false, isShowLoading: true, onFinished: (result) =>
            //        {
            //            Utils.Log($"[TitleScene] GoWindowScene : Home, Result : {result}");
            //            SystemUIManager.Instance.FadeIn();
            //        });
            //    });
            //});

            LoginManager.Instance.Login(textLog, () =>
            {
                SystemUIManager.Instance.FadeOut(() =>
                {
                    SceneControlManager.Instance.UnloadScene(eScene.Title, () =>
                    {
                        UIControlManager.Instance.GoWindowScene(eScene.Home, isHistory: false, isShowLoading: true, onFinished: (result) =>
                        {
                            Utils.Log($"[TitleScene] GoWindowScene : Home, Result : {result}");
                            SystemUIManager.Instance.FadeIn();
                        });
                    });
                });
            });
        });
    }
}
