using UnityEngine;

[AddComponentMenu("TweenUtils/Material Float Tween Util")]
public class MaterialFloatTweenUtil : TweenBehaviourBase
{
    public Renderer materTarget;
    public int materialIndex = 0;
    public string propertyName = "_Intensity";
    public float startValue = 0f;
    public float targetValue = 1f;

    float _captured;
    Material _mat;

    protected override void CaptureStart()
    {
        if (materTarget == null) materTarget = GetComponent<Renderer>();
        _mat = null;

        if (materTarget != null)
        {
            var mats = materTarget.materials;
            if (mats != null && mats.Length > 0)
            {
                materialIndex = Mathf.Clamp(materialIndex, 0, mats.Length - 1);
                _mat = mats[materialIndex];
            }
        }
        _captured = captureStartFromCurrent && _mat != null && _mat.HasProperty(propertyName) ? _mat.GetFloat(propertyName) : startValue;
        ApplyEvaluated(0f);
    }

    protected override void ApplyEvaluated(float t01)
    {
        if (_mat == null || !_mat.HasProperty(propertyName)) return;
        float v = Mathf.LerpUnclamped(_captured, targetValue, Mathf.Clamp01(t01));
        _mat.SetFloat(propertyName, v);
    }

    protected override void ApplyRaw(float raw01) { ApplyEvaluated(raw01); }

    protected override void ResetToStart()
    {
        if (_mat == null || !_mat.HasProperty(propertyName)) return;
        _mat.SetFloat(propertyName, _captured);
    }
}