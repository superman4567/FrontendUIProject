using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-10000)]
public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    public static Action OnShowPopup;
    public static Action OnClosePopup;

    readonly Dictionary<PopupEnums, PopupControllerBase> byKey = new();
    readonly Stack<PopupControllerBase> history = new();

    PopupControllerBase current;

    public PopupEnums? CurrentKey => current ? current.Key : (PopupEnums?)null;
    public PopupEnums? PreviousKey => history.Count > 0 ? history.Peek().Key : (PopupEnums?)null;

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Register(PopupControllerBase p)
    {
        if (!byKey.ContainsKey(p.Key)) byKey.Add(p.Key, p);
    }

    public void Unregister(PopupControllerBase p)
    {
        if (byKey.TryGetValue(p.Key, out var refp) && refp == p) byKey.Remove(p.Key);
    }

    public bool Show(PopupEnums key)
    {
        if (!byKey.TryGetValue(key, out var next)) return false;
        if (current == next) return false;

        if (current) { history.Push(current); current.Hide(); }

        current = next;
        current.Show();
        OnShowPopup?.Invoke();
        return true;
    }

    public bool Back()
    {
        if (current == null || history.Count == 0) return false;

        current.Hide();
        current = history.Pop();
        current.Show();
        return true;
    }

    public bool CloseCurrent()
    {
        if (current == null) return false;

        current.Hide();
        current = null;
        OnClosePopup?.Invoke();

        if (history.Count > 0)
        {
            current = history.Pop();
            current.Show();
        }
        return true;
    }

    public void CloseAll()
    {
        if (current) current.Hide();
        current = null;
        while (history.Count > 0) history.Pop().Hide();
    }

    public bool IsOpen(PopupEnums key) => current && current.Key.Equals(key);
}