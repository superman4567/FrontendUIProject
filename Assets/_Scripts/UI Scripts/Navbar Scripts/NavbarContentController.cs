using System;
using System.Collections.Generic;
using UnityEngine;

public class NavbarContentController : MonoBehaviour
{
    [Serializable]
    public class Entry
    {
        public NavbarEnums key;       
        public CanvasGroup contentCanvasGroup;  
    }

    [Header("Wiring")]
    [SerializeField] private NavbarView navbarView;
    [SerializeField] private List<Entry> entries = new();

    [Header("Behavior")]
    [SerializeField] private bool hideOthersOnActivate = true;
    [SerializeField] private bool hideAllOnClosed = true;
    [SerializeField] private NavbarEnums defaultKey = NavbarEnums.House; // optional fallback

    // quick lookup
    Dictionary<NavbarEnums, Entry> map;

    void Awake()
    {
        map = new Dictionary<NavbarEnums, Entry>(entries.Count);
        
        foreach (var e in entries)
        {
            if (e.contentCanvasGroup) e.contentCanvasGroup.alpha = 0f;
            map[e.key] = e;
        }
    }

    void OnEnable()
    {
        navbarView.ContentActivated += OnContentActivated;
        navbarView.Closed += OnClosed;
    }

    void OnDisable()
    {
        navbarView.ContentActivated -= OnContentActivated;
        navbarView.Closed -= OnClosed;
    }

    void OnContentActivated(NavbarEnums key)
    {
        if (!map.TryGetValue(key, out var onEntry)) return;

        if (hideOthersOnActivate)
        {
            foreach (var e in entries)
            {
                bool show = e == onEntry;
                SetVisible(e, show);
            }
        }
        else
        {
            SetVisible(onEntry, true);
        }
    }

    void OnClosed()
    {
        if (hideAllOnClosed)
        {
            foreach (var e in entries) SetVisible(e, false);
        }
        else
        {
            // fallback: show a default/home panel instead of empty state
            if (map.TryGetValue(defaultKey, out var def))
            {
                foreach (var e in entries) SetVisible(e, e == def);
            }
        }
    }

    void SetVisible(Entry e, bool visible)
    {
        if (visible)
            e.contentCanvasGroup.alpha = 1f;
        else
            e.contentCanvasGroup.alpha = 0f;
    }
}
