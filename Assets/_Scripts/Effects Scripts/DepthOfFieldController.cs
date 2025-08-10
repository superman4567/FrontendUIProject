using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;
using DG.Tweening;

public class DepthOfFieldController : MonoBehaviour
{
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
        PopupManager.OnShowPopup += OnShowPopup_Callback;
        PopupManager.OnClosePopup += OnClosePopup_Callback;
    }

    private void OnDisable()
    {
        PopupManager.OnShowPopup -= OnShowPopup_Callback;
        PopupManager.OnClosePopup -= OnClosePopup_Callback;
        focalLengthTween.Kill();
    }

    private void OnShowPopup_Callback()
    {
        HandleDepthOfFieldToggle(true);
    }

    private void OnClosePopup_Callback()
    {
        HandleDepthOfFieldToggle(false);
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
