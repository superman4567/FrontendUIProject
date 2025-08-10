using DG.Tweening;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class BackgroundDarkeningController : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float tweenDuration = 0.5f;

    private Tweener darkeningTween;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
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
        darkeningTween.Kill();
    }

    private void OnShowPopup_Callback()
    {
        HandleDarkeningBackground(true);
    }

    private void OnClosePopup_Callback()
    {
        HandleDarkeningBackground(false);
    }

    private void HandleDarkeningBackground(bool enableEffect)
    {
        float targetValue = enableEffect ? 1f : 0f;
        
        darkeningTween = DOTween.To(
            () => canvasGroup.alpha,
            x => canvasGroup.alpha = x,
            targetValue,
            tweenDuration
        ).SetEase(Ease.OutCubic);
    }
}
