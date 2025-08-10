using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    readonly Dictionary<PopupEnums, PopupController> byKey = new Dictionary<PopupEnums, PopupController>();
    readonly Stack<PopupController> history = new Stack<PopupController>();
    readonly HashSet<PopupEnums> everShown = new HashSet<PopupEnums>();

    PopupController current;

    public PopupEnums? CurrentKey => current ? current.Key : (PopupEnums?)null;
    public PopupEnums? PreviousKey => history.Count > 0 ? history.Peek().Key : (PopupEnums?)null;

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Register(PopupController p)
    {
        if (!byKey.ContainsKey(p.Key)) byKey.Add(p.Key, p);
    }

    public void Unregister(PopupController p)
    {
        if (byKey.TryGetValue(p.Key, out var refp) && refp == p) byKey.Remove(p.Key);
    }

    public bool Show(PopupEnums key)
    {
        if (!byKey.TryGetValue(key, out var next)) return false;
        if (current == next) return true;

        if (current)
        {
            history.Push(current);
            current.InstantHide();
        }

        current = next;
        current.InstantShow();
        everShown.Add(key);
        return true;
    }

    public bool Back()
    {
        if (current == null || history.Count == 0) return false;

        current.InstantHide();
        current = history.Pop();
        current.InstantShow();
        return true;
    }

    public bool CloseCurrent()
    {
        if (current == null) return false;

        current.InstantHide();
        current = null;

        if (history.Count > 0)
        {
            current = history.Pop();
            current.InstantShow();
        }
        return true;
    }

    public void CloseAll()
    {
        if (current) current.InstantHide();
        current = null;
        while (history.Count > 0) history.Pop().InstantHide();
    }

    public bool IsOpen(PopupEnums key) => current && current.Key.Equals(key);
    public bool WasEverShown(PopupEnums key) => everShown.Contains(key);
}
