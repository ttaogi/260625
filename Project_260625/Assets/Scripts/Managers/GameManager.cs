using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>, IManager
{
    #region Inspector
    #endregion Inspector

    protected override void Awake()
    {
        base.Awake();

        if (IsMyInstance())
            DontDestroyOnLoad(gameObject);
    }

    protected void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        InputManager.Instance.Init();
        SceneControlManager.Instance.Init();
        SystemUIManager.Instance.Init();
        UIControlManager.Instance.Init();
    }
}
