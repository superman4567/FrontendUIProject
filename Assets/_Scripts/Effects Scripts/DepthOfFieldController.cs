using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;
using DG.Tweening;

public class DepthOfFieldController : MonoBehaviour
{
    public static Action<bool> OnDepthOfFieldToggle;

    [SerializeField] private Volume volume;
    [SerializeField] private float tweenDuration = 0.5f;

    private DepthOfField dof;
    private Tweener focalLengthTween;

    private void Awake()
    {
        volume.profile.TryGet(out DepthOfField depthOfField);
        dof = depthOfField;
    }

    private void OnEnable()
    {
        OnDepthOfFieldToggle += HandleDepthOfFieldToggle;
    }

    private void OnDisable()
    {
        OnDepthOfFieldToggle -= HandleDepthOfFieldToggle;
        focalLengthTween.Kill();
    }

    private void HandleDepthOfFieldToggle(bool enableEffect)
    {
        float targetValue = enableEffect ? 300f : 0f;
        focalLengthTween.Kill();
        focalLengthTween = DOTween.To(
            () => dof.focalLength.value,
            x => dof.focalLength.value = x,
            targetValue,
            tweenDuration
        ).SetEase(Ease.OutCubic);
    }
}
