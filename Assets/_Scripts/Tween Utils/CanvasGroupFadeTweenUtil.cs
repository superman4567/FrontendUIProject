using UnityEngine;

[AddComponentMenu("TweenUtils/CanvasGroup Fade Tween Util")]
public class CanvasGroupFadeTweenUtil : TweenBehaviourBase
{
    public CanvasGroup canvasGroupTarget;
    public float startAlpha = 0f;
    public float targetAlpha = 1f;
    float _captured;

    protected override void CaptureStart()
    {
        if (canvasGroupTarget == null) canvasGroupTarget = GetComponent<CanvasGroup>();
        _captured = captureStartFromCurrent && canvasGroupTarget != null ? canvasGroupTarget.alpha : startAlpha;
        ApplyEvaluated(0f);
    }

    protected override void ApplyEvaluated(float t01)
    {
        if (canvasGroupTarget == null) return;
        canvasGroupTarget.alpha = Mathf.LerpUnclamped(_captured, targetAlpha, Mathf.Clamp01(t01));
    }

    protected override void ApplyRaw(float raw01) { ApplyEvaluated(raw01); }

    protected override void ResetToStart()
    {
        if (canvasGroupTarget == null) return;
        canvasGroupTarget.alpha = _captured;
    }
}