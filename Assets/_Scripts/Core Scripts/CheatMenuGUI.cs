using UnityEngine;

public class CheatMenuUGUI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private CheatMenu cheatMenu;

    [Header("Settings")]
    [SerializeField] private bool showUI = true;
    [SerializeField] private Vector2 scrollSize = new Vector2(300, 400);

    Vector2 scroll;
    NavbarEnums selectedKey = default;

    void OnGUI()
    {
        if (!showUI || cheatMenu == null) return;

        GUILayout.BeginArea(new Rect(10, 10, scrollSize.x, scrollSize.y), GUI.skin.box);
        GUILayout.Label("<b>Cheat Menu Debug</b>", new GUIStyle(GUI.skin.label) { richText = true });

        scroll = GUILayout.BeginScrollView(scroll);

        // Global controls
        GUILayout.Space(5);
        GUILayout.Label("Global Controls", EditorLabel());
        if (GUILayout.Button("Show Navbar")) cheatMenu.ShowNavbar();
        if (GUILayout.Button("Hide Navbar")) cheatMenu.HideNavbar();
        if (GUILayout.Button("Deactivate All")) cheatMenu.DeactivateAll();

        GUILayout.Space(10);

        // Per-button controls
        GUILayout.Label("Per-Button Controls", EditorLabel());
        for (int i = 0; i < cheatMenu.navbarButtons.Length; i++)
        {
            var btn = cheatMenu.navbarButtons[i];
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label($"Button {i} ({btn.ContentKey})");

            if (GUILayout.Button("Click")) cheatMenu.ClickButton(i);
            if (GUILayout.Button("Activate Only")) cheatMenu.ActivateOnly(i);

            // Turn Off - replaces TurnOffImmediate / Animated
            if (GUILayout.Button("Turn Off"))
            {
                if (btn.IsOn)
                    btn.OnClick();
            }

            bool locked = btn.IsLocked;
            bool newLocked = GUILayout.Toggle(locked, "Locked");
            if (newLocked != locked)
                cheatMenu.SetLocked(i, newLocked);

            GUILayout.EndVertical();
        }

        GUILayout.Space(10);

        // By key controls (enum dropdown instead of text field)
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
