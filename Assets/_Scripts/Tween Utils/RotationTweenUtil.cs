using UnityEngine;

[AddComponentMenu("TweenUtils/Rotation Tween Util")]
public class RotationTweenUtil : TweenBehaviourBase
{
    [Header("Rotation Settings")]
    public Vector3 startEuler = Vector3.zero;
    public Vector3 targetEuler = new Vector3(0f, 0f, 6f);
    public bool relative = true;
    public Vector3 axisMask = new Vector3(0, 0, 1);

    Quaternion _capturedStartRot;
    Vector3 _capturedStartEuler;

    protected override void CaptureStart()
    {
        if (target == null) target = transform;
        _capturedStartRot = target.localRotation;
        _capturedStartEuler = target.localEulerAngles;
        ApplyEvaluated(0f);
    }

    protected override void ApplyEvaluated(float t01)
    {
        Vector3 e;
        if (relative)
        {
            Vector3 delta = Vector3.LerpUnclamped(startEuler, targetEuler, Mathf.Clamp01(t01));
            e = _capturedStartEuler + Vector3.Scale(delta, axisMask);
        }
        else
        {
            Vector3 absolute = Vector3.LerpUnclamped(startEuler, targetEuler, Mathf.Clamp01(t01));
            e = new Vector3(
                axisMask.x > 0 ? absolute.x : _capturedStartEuler.x,
                axisMask.y > 0 ? absolute.y : _capturedStartEuler.y,
                axisMask.z > 0 ? absolute.z : _capturedStartEuler.z
            );
        }
        target.localRotation = Quaternion.Euler(e);
    }

    protected override void ApplyRaw(float raw01)
    {
        ApplyEvaluated(raw01);
    }

    protected override void ResetToStart()
    {
        target.localRotation = _capturedStartRot;
    }
}

