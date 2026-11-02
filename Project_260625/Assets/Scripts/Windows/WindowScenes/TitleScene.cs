using TMPro;
using UnityEngine;
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
            //LoginManager.Instance.Login(textLog, () =>
            //{
                UIControlManager.Instance.GoWindowScene(eScene.Home, isHistory: false, onFinished: (result) =>
                {
                    Utils.Log($"[TitleScene] GoWindowScene : Home, Result : {result}");
                });
            //});
        });
    }
}
