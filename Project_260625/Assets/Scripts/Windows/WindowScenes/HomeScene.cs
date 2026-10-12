using UnityEngine;

public class HomeScene : WindowScene
{
    #region Inspector
    public GameObject objChar;
    #endregion Inspector

    #region Open
    protected override void OpenProcess()
    {
        base.OpenProcess();

        CameraManager.Instance.SetTarget(objChar);
    }
    #endregion Open
}
