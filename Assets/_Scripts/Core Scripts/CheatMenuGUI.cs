using UnityEngine;

public class CheatMenuUGUI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private CheatMenu cheatMenu;

    [Header("Layout")]
    [SerializeField] private Vector2 anchor = new Vector2(10, 10);
    [SerializeField] private Vector2 panelSize = new Vector2(300, 400);
    [SerializeField] private float headerHeight = 24f;

    private bool isOpen = false;
    private Vector2 scroll;
    private NavbarEnums selectedKey = default;

    private bool mouseDownOnHeader = false;
    private bool didDrag = false;
    private Vector2 dragOffset;
    private Vector2 mouseDownPos;

    void OnGUI()
    {
        Rect headerRect = new Rect(anchor.x, anchor.y, panelSize.x, headerHeight);
        Event e = Event.current;

        if (e.type == EventType.MouseDown && headerRect.Contains(e.mousePosition))
        {
            mouseDownOnHeader = true;
            didDrag = false;
            mouseDownPos = e.mousePosition;
            dragOffset = e.mousePosition - anchor;
            e.Use();
        }
        else if (e.type == EventType.MouseDrag && mouseDownOnHeader)
        {
            anchor = e.mousePosition - dragOffset;
            if ((e.mousePosition - mouseDownPos).sqrMagnitude > 1f) didDrag = true;
            e.Use();
        }
        else if (e.type == EventType.MouseUp && mouseDownOnHeader)
        {
            if (!didDrag) isOpen = !isOpen;
            mouseDownOnHeader = false;
            didDrag = false;
            e.Use();
        }

        string headerLabel = isOpen ? "Cheats ▲" : "Cheats ▼";
        GUI.Box(headerRect, headerLabel);

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.F1)
        {
            isOpen = !isOpen;
            e.Use();
        }

        if (!isOpen || cheatMenu == null) return;

        Rect panelRect = new Rect(anchor.x, anchor.y + headerHeight + 2f, panelSize.x, panelSize.y);
        GUILayout.BeginArea(panelRect, GUI.skin.box);
        GUILayout.Label("<b>Cheat Menu Debug</b>", new GUIStyle(GUI.skin.label) { richText = true });

        scroll = GUILayout.BeginScrollView(scroll);

        GUILayout.Space(5);
        GUILayout.Label("Global Controls", EditorLabel());
        if (GUILayout.Button("Show Navbar")) cheatMenu.ShowNavbar();
        if (GUILayout.Button("Hide Navbar")) cheatMenu.HideNavbar();
        if (GUILayout.Button("Deactivate All")) cheatMenu.DeactivateAll();

        GUILayout.Space(10);

        GUILayout.Label("Per-Button Controls", EditorLabel());
        for (int i = 0; i < cheatMenu.navbarButtons.Length; i++)
        {
            var btn = cheatMenu.navbarButtons[i];
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label($"Button {i} ({btn.ContentKey})");

            if (GUILayout.Button("Click")) cheatMenu.ClickButton(i);
            if (GUILayout.Button("Activate Only")) cheatMenu.ActivateOnly(i);

            if (GUILayout.Button("Turn Off"))
            {
                if (btn.IsOn) btn.OnClick();
            }

            bool locked = btn.IsLocked;
            bool newLocked = GUILayout.Toggle(locked, "Locked");
            if (newLocked != locked) cheatMenu.SetLocked(i, newLocked);

            GUILayout.EndVertical();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Toggle Level Completer")) cheatMenu.ToggleLevelComplete();

        GUILayout.Space(10);

        GUILayout.Label("By Key Controls", EditorLabel());
        GUILayout.BeginHorizontal();
        GUILayout.Label("Content Key:", GUILayout.Width(90));
#if UNITY_EDITOR
        selectedKey = (NavbarEnums)UnityEditor.EditorGUILayout.EnumPopup(selectedKey, GUILayout.MinWidth(100));
#else
        GUILayout.Label(selectedKey.ToString(), GUILayout.MinWidth(100));
#endif
        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    GUIStyle EditorLabel()
    {
        return new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
    }
}
