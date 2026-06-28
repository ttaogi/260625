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

    public override void Init()
    {
        base.Init();
    }
}
