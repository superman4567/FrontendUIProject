using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("TweenUtils/Image Fill Tween Util")]
public class ImageFillTweenUtil : TweenBehaviourBase
{
    public Image imagetarget;
    public float startFill = 0f;
    public float targetFill = 1f;
    float _captured;

    protected override void CaptureStart()
    {
        if (imagetarget == null) imagetarget = GetComponent<Image>();
        _captured = captureStartFromCurrent && imagetarget != null ? imagetarget.fillAmount : startFill;
        ApplyEvaluated(0f);
    }

    protected override void ApplyEvaluated(float t01)
    {
        if (imagetarget == null) return;
        imagetarget.fillAmount = Mathf.LerpUnclamped(_captured, targetFill, Mathf.Clamp01(t01));
    }

    protected override void ApplyRaw(float raw01) { ApplyEvaluated(raw01); }

    protected override void ResetToStart()
    {
        if (imagetarget == null) return;
        imagetarget.fillAmount = _captured;
    }
}