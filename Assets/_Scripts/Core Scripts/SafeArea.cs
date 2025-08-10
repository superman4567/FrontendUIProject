using UnityEngine;

public class SafeArea : MonoBehaviour
{
    [SerializeField] private RectTransform content;

    private void Start()
    {
        ApplySafeArea();
    }

    void ApplySafeArea()
    {
        Rect safe = Screen.safeArea;

        float w = Mathf.Max(1f, Screen.width);
        float h = Mathf.Max(1f, Screen.height);

        Vector2 anchorMin = new Vector2(safe.xMin / w, safe.yMin / h);
        Vector2 anchorMax = new Vector2(safe.xMax / w, safe.yMax / h);

        content.anchorMin = anchorMin;
        content.anchorMax = anchorMax;
        content.offsetMin = Vector2.zero;
        content.offsetMax = Vector2.zero;
    }
}
