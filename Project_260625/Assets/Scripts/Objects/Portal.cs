using UnityEngine;

public class Portal : MonoBehaviour
{
    #region Inspector
    public eScene targetScene;
    #endregion Inspector

    private const string HERO = "Hero";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains(HERO) && targetScene != eScene.None)
        {
            eScene startScene = SceneControlManager.Instance.CurScene;

            SystemUIManager.Instance.FadeOut(() =>
            {
                SceneControlManager.Instance.UnloadScene(startScene, () =>
                {
                    UIControlManager.Instance.GoWindowScene(targetScene, isHistory: true, isShowLoading: true, onFinished: (result) =>
                    {
                        Utils.Log($"[{startScene}Scene] GoWindowScene : {targetScene}, Result : {result}");
                        SystemUIManager.Instance.FadeIn();
                    });
                });
            });
        }
    }
}
