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
            UIControlManager.Instance.GoWindowScene(targetScene, isHistory: true, onFinished: (result) =>
            {
                Utils.Log($"[Portal] GoWindowScene : {targetScene}, Result : {result}");
            });
        }
    }
}
