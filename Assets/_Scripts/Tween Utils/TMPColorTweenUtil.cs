using TMPro;
using UnityEngine;

[AddComponentMenu("TweenUtils/TMP Color Tween Util")]
public class TMPColorTweenUtil : TweenBehaviourBase
{
    public TMP_Text textTarget;
    public Color startColor = Color.white;
    public Color targetColor = Color.yellow;
    Color _captured;

    protected override void CaptureStart()
    {
        if (textTarget == null) textTarget = GetComponent<TMP_Text>();
        _captured = captureStartFromCurrent && textTarget != null ? textTarget.color : startColor;
        ApplyEvaluated(0f);
    }

    protected override void ApplyEvaluated(float t01)
    {
        if (textTarget == null) return;
        textTarget.color = Color.LerpUnclamped(_captured, targetColor, Mathf.Clamp01(t01));
    }

    protected override void ApplyRaw(float raw01)
    {
        ApplyEvaluated(raw01);
    }

    protected override void ResetToStart()
    {
        if (textTarget == null) return;
        textTarget.color = _captured;
    }
}

