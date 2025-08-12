using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("TweenUtils/Graphic Color Tween Util")]
public class GraphicColorTweenUtil : TweenBehaviourBase
{
    public Graphic target;
    public Color startColor = Color.white;
    public Color targetColor = Color.yellow;
    Color _captured;

    protected override void CaptureStart()
    {
        if (target == null) target = GetComponent<Graphic>();
        _captured = captureStartFromCurrent && target != null ? target.color : startColor;
        ApplyEvaluated(0f);
    }

    protected override void ApplyEvaluated(float t01)
    {
        if (target == null) return;
        target.color = Color.LerpUnclamped(_captured, targetColor, Mathf.Clamp01(t01));
    }

    protected override void ApplyRaw(float raw01) { ApplyEvaluated(raw01); }

    protected override void ResetToStart()
    {
        if (target == null) return;
        target.color = _captured;
    }
}