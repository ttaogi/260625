using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class ExtensionMethod
{
    public static void SetOnClickEvent(this Button btn, UnityAction onClickEvent)
    {
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(onClickEvent);
    }

    public static void SetOnClickEvent(this EventTrigger trigger, Action onClickEvent)
    {
        trigger.triggers.Clear();

        EventTrigger.Entry entry = new();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) =>
        {
            onClickEvent?.Invoke();
        });

        trigger.triggers.Add(entry);
    }
}
