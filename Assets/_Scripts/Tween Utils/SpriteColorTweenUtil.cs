using UnityEngine;

[AddComponentMenu("TweenUtils/Sprite Color Tween Util")]
public class SpriteColorTweenUtil : TweenBehaviourBase
{
    public SpriteRenderer spriteTarget;
    public Color startColor = Color.white;
    public Color targetColor = Color.yellow;
    Color _captured;

    protected override void CaptureStart()
    {
        if (spriteTarget == null) spriteTarget = GetComponent<SpriteRenderer>();
        _captured = captureStartFromCurrent && spriteTarget != null ? spriteTarget.color : startColor;
        ApplyEvaluated(0f);
    }

    protected override void ApplyEvaluated(float t01)
    {
        if (spriteTarget == null) return;
        spriteTarget.color = Color.LerpUnclamped(_captured, targetColor, Mathf.Clamp01(t01));
    }

    protected override void ApplyRaw(float raw01) { ApplyEvaluated(raw01); }

    protected override void ResetToStart()
    {
        if (spriteTarget == null) return;
        spriteTarget.color = _captured;
    }
}