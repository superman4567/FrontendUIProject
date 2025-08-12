using UnityEngine;

[AddComponentMenu("TweenUtils/Position Tween Util (UI Offset)")]
public class PositionTweenUtil : TweenBehaviourBase
{
    [Header("Position Settings")]
    public bool isUIRect = true; 
    public bool relative = true;
    public Vector3 startOffset = Vector3.zero;
    public Vector3 targetOffset = new Vector3(8f, -8f, 0f);
    public Vector3 axisMask = new Vector3(1, 1, 0);

    RectTransform _rt;
    Vector3 _capturedStartWorld;
    Vector2 _capturedStartAnchored;

    protected override void CaptureStart()
    {
        if (target == null) target = transform;
        _rt = target as RectTransform;

        if (isUIRect && _rt != null)
        {
            _capturedStartAnchored = _rt.anchoredPosition;
        }
        else
        {
            _capturedStartWorld = target.localPosition; // local for consistency
        }
        ApplyEvaluated(0f);
    }

    protected override void ApplyEvaluated(float t01)
    {
        float raw = Mathf.Clamp01(t01);
        Vector3 off = Vector3.LerpUnclamped(startOffset, targetOffset, raw);

        if (relative)
        {
            if (isUIRect && _rt != null)
            {
                Vector2 desired = _capturedStartAnchored + new Vector2(off.x * axisMask.x, off.y * axisMask.y);
                _rt.anchoredPosition = desired;
            }
            else
            {
                Vector3 desired = _capturedStartWorld + Vector3.Scale(off, axisMask);
                target.localPosition = desired;
            }
        }
        else
        {
            if (isUIRect && _rt != null)
            {
                Vector2 desired = Vector2.LerpUnclamped(_capturedStartAnchored, new Vector2(targetOffset.x, targetOffset.y), raw);
                _rt.anchoredPosition = new Vector2(
                    axisMask.x > 0 ? desired.x : _capturedStartAnchored.x,
                    axisMask.y > 0 ? desired.y : _capturedStartAnchored.y
                );
            }
            else
            {
                Vector3 desired = Vector3.LerpUnclamped(_capturedStartWorld, targetOffset, raw);
                target.localPosition = new Vector3(
                    axisMask.x > 0 ? desired.x : _capturedStartWorld.x,
                    axisMask.y > 0 ? desired.y : _capturedStartWorld.y,
                    axisMask.z > 0 ? desired.z : _capturedStartWorld.z
                );
            }
        }
    }

    protected override void ApplyRaw(float raw01)
    {
        ApplyEvaluated(raw01);
    }

    protected override void ResetToStart()
    {
        if (isUIRect && _rt != null)
        {
            _rt.anchoredPosition = _capturedStartAnchored;
        }
        else
        {
            target.localPosition = _capturedStartWorld;
        }
    }
}
