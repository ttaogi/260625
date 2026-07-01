using System;
using UnityEngine;

public class InputManager : SingletonBehaviour<InputManager>, IManager
{
    #region Event
    private Action _onBackPress;
    public event Action OnBackPress
    {
        add
        {
            _onBackPress -= value;
            _onBackPress += value;
        }
        remove
        {
            _onBackPress -= value;
        }
    }
    #endregion Event

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            _onBackPress?.Invoke();
    }
}
