using UnityEngine;
using TMPro;

[AddComponentMenu("TweenUtils/TMP Number Tween Util")]
public class TMPNumberTweenUtil : TweenBehaviourBase
{
    public TextMeshProUGUI textTarget;
    public float startValue = 0f;
    public float targetValue = 100f;
    public bool integer = true;
    public string format = "N0";
    float _capturedStart;

    protected override void CaptureStart()
    {
        if (textTarget == null) textTarget = GetComponent<TextMeshProUGUI>();
        _capturedStart = captureStartFromCurrent ? ParseCurrent() : startValue;
        ApplyEvaluated(0f);
    }

    float ParseCurrent()
    {
        if (textTarget == null) return startValue;
        if (float.TryParse(textTarget.text, out float v)) return v; return startValue;
    }

    protected override void ApplyEvaluated(float t01)
    {
        if (textTarget == null) return;
        float v = Mathf.LerpUnclamped(_capturedStart, targetValue, Mathf.Clamp01(t01));
        if (integer) textTarget.text = Mathf.RoundToInt(v).ToString(format);
        else textTarget.text = v.ToString(format);
    }

    protected override void ApplyRaw(float raw01) { ApplyEvaluated(raw01); }

    protected override void ResetToStart()
    {
        if (textTarget == null) return;
        if (integer) textTarget.text = Mathf.RoundToInt(_capturedStart).ToString(format);
        else textTarget.text = _capturedStart.ToString(format);
    }
}