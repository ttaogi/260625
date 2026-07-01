using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIControlManager : SingletonBehaviour<UIControlManager>
{
    #region Event
    private Action<WindowScene> _onChangeScene;
    public event Action<WindowScene> OnChangeScene
    {
        add
        {
            _onChangeScene -= value;
            _onChangeScene += value;
        }
        remove
        {
            _onChangeScene -= value;
        }
    }
    #endregion Event

    #region Inspector
    #endregion Inspector

    private List<eScene> _history = new();

    public WindowScene CurrentWindowScene { get; private set; }

    /////////////////////////////////////////////////

    public override void Init()
    {
        base.Init();

        InputManager.Instance.OnBackPress += null;
        SceneControlManager.Instance.OnChangeScenePre += Clear;
    }

    /////////////////////////////////////////////////

    private void Clear()
    {

    }




}
