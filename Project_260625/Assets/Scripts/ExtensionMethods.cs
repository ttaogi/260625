using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class ExtensionMethod
{
    public static void SetOnClickEvent(this Button btn, UnityAction onClickEvent)
    {
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(onClickEvent);
    }
}
