using UnityEngine;

public class ScaleTweenUtil : TweenBehaviourBase
{
    [Header("Scale Settings")]
    public Vector3 startScale = Vector3.one;
    public Vector3 targetScale = new Vector3(1.05f, 1.05f, 1.05f);
    public Vector3 axisMask = new Vector3(1, 1, 1);

    Vector3 _capturedStart;

    protected override void CaptureStart()
    {
        if (target == null) target = transform;
        _capturedStart = captureStartFromCurrent ? target.localScale : startScale;
        // Ensure initial applied state reflects captured start
        ApplyEvaluated(0f);
    }

    protected override void ApplyEvaluated(float t01)
    {
        if (target == null) return;
        Vector3 desired = Vector3.LerpUnclamped(_capturedStart, targetScale, t01);
        Vector3 current = target.localScale;
        Vector3 masked = new Vector3(
            Mathf.LerpUnclamped(current.x, desired.x, axisMask.x > 0 ? 1f : 0f),
            Mathf.LerpUnclamped(current.y, desired.y, axisMask.y > 0 ? 1f : 0f),
            Mathf.LerpUnclamped(current.z, desired.z, axisMask.z > 0 ? 1f : 0f)
        );
        // Directly set desired values on masked axes; keep others unchanged
        target.localScale = new Vector3(
            axisMask.x > 0 ? desired.x : _capturedStart.x,
            axisMask.y > 0 ? desired.y : _capturedStart.y,
            axisMask.z > 0 ? desired.z : _capturedStart.z
        );
    }

    protected override void ApplyRaw(float raw01)
    {
        if (target == null) return;
        Vector3 desired = Vector3.LerpUnclamped(_capturedStart, targetScale, Mathf.Clamp01(raw01));
        target.localScale = new Vector3(
            axisMask.x > 0 ? desired.x : _capturedStart.x,
            axisMask.y > 0 ? desired.y : _capturedStart.y,
            axisMask.z > 0 ? desired.z : _capturedStart.z
        );
    }

    protected override void ResetToStart()
    {
        if (target == null) return;
        target.localScale = _capturedStart;
    }
}
