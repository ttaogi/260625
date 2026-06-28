using System;
using UnityEngine;

public class InputManager : SingletonBehaviour<InputManager>, IManager
{
    #region Event
    private Action _onBack;
    public event Action OnBack
    {
        add
        {
            _onBack -= value;
            _onBack += value;
        }
        remove
        {
            _onBack -= value;
        }
    }
    #endregion Event

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            _onBack?.Invoke();
    }
}
