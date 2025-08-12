using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class NavbarIndicatorAnimator : MonoBehaviour
{
    [Header("Indicator")]
    [SerializeField] private RectTransform indicator;
    [SerializeField] private CanvasGroup indicatorCanvasGroup;

    [Header("Timing")]
    [SerializeField] private float followDuration = 0.25f;
    [SerializeField] private AnimationCurve followEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Close Animation")]
    [SerializeField] private float resizeDuration = 0.18f;
    [SerializeField] private float hideDuration = 0.18f;
    [SerializeField] private float dropDistance = 20f;
    [SerializeField] private AnimationCurve closeEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Buttons (optional)")]
    [SerializeField] private NavbarButton[] navbarButtons;

    private NavbarButton lastSelectedButton;
    private RectTransform lastSelectedRect;
    private Tween activeTween;

    void Awake()
    {
        HideImmediately();   // ensure hidden on scene load
    }

    void OnEnable()
    {
        NavbarButton.ButtonSelected += HandleSelected;
       
        foreach (NavbarButton navbarButton in navbarButtons.Where(button => button != null))
        {
            navbarButton.ToggledOff += HandleToggledOff;
        }
    }

    void OnDisable()
    {
        NavbarButton.ButtonSelected -= HandleSelected;

        foreach (NavbarButton navbarButton in navbarButtons.Where(button => button != null)) 
        {
            navbarButton.ToggledOff -= HandleToggledOff;
        }

        activeTween.Kill();
    }

    public void HideImmediately()
    {
        if (indicatorCanvasGroup != null) indicatorCanvasGroup.alpha = 0f;

        // optional: collapse width so it doesn’t flash in layout reflows
        var parentRect = (RectTransform)indicator.parent;
        indicator.anchorMin = new Vector2(0.5f, 0f);
        indicator.anchorMax = new Vector2(0.5f, 1f);
        indicator.pivot = new Vector2(0.5f, 0.5f);
        indicator.offsetMin = new Vector2(0f, indicator.offsetMin.y);
        indicator.offsetMax = new Vector2(0f, indicator.offsetMax.y);

        lastSelectedButton = null;
    }

    void HandleSelected(NavbarButton selectedButton, RectTransform targetRect)
    {
        if (selectedButton == lastSelectedButton) return;

        lastSelectedButton = selectedButton;
        lastSelectedRect = targetRect;
        FollowTarget(targetRect);
    }

    void FollowTarget(RectTransform targetRect)
    {
        RectTransform parentRect = (RectTransform)indicator.parent;

        indicator.anchorMin = new Vector2(0f, 0f);
        indicator.anchorMax = new Vector2(1f, 1f);
        indicator.pivot = new Vector2(0.5f, 0.5f);

        float startMinY = indicator.offsetMin.y;
        float startMaxY = indicator.offsetMax.y;

        float parentLeft = -parentRect.rect.width * parentRect.pivot.x;
        float parentRight = parentRect.rect.width * (1f - parentRect.pivot.x);

        float startLeftInset = indicator.offsetMin.x;
        float startRightInset = -indicator.offsetMax.x;
        float startLeftEdge = parentLeft + startLeftInset;
        float startRightEdge = parentRight - startRightInset;
        float startCenter = 0.5f * (startLeftEdge + startRightEdge);
        float startHalfWidth = 0.5f * (startRightEdge - startLeftEdge);

        activeTween.Kill();
        activeTween = DOVirtual.Float(0f, 1f, followDuration, progress =>
        {
            float easedProgress = followEase.Evaluate(progress);

            RectTransform layoutRootRect = targetRect.parent as RectTransform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRootRect);
            Canvas.ForceUpdateCanvases();

            Bounds targetBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(parentRect, targetRect);
            float targetLeft = targetBounds.min.x;
            float targetRight = targetBounds.max.x;
            float targetCenter = 0.5f * (targetLeft + targetRight);
            float targetHalfWidth = 0.5f * (targetRight - targetLeft);

            float currentCenter = Mathf.LerpUnclamped(startCenter, targetCenter, easedProgress);
            float currentHalfWidth = Mathf.LerpUnclamped(startHalfWidth, targetHalfWidth, easedProgress);

            float leftEdge = currentCenter - currentHalfWidth;
            float rightEdge = currentCenter + currentHalfWidth;
            float leftInset = leftEdge - parentLeft;
            float rightInset = parentRight - rightEdge;

            indicator.offsetMin = new Vector2(leftInset, startMinY);
            indicator.offsetMax = new Vector2(-rightInset, startMaxY);

            if (indicatorCanvasGroup != null) indicatorCanvasGroup.alpha = 1f;
        })
        .SetUpdate(true);
    }

    void HandleToggledOff(INavbarButton toggledButton)
    {
        if (lastSelectedButton == null || !ReferenceEquals(lastSelectedButton, toggledButton)) return;
        AnimateClose(lastSelectedButton.GetComponent<RectTransform>());
    }

    void AnimateClose(RectTransform targetRect)
    {
        if (indicator == null || targetRect == null) return;

        RectTransform parentRect = (RectTransform)indicator.parent;

        float parentLeft = -parentRect.rect.width * parentRect.pivot.x;
        float parentRight = parentRect.rect.width * (1f - parentRect.pivot.x);

        float startMinY = indicator.offsetMin.y;
        float startMaxY = indicator.offsetMax.y;
        float startAlpha = indicatorCanvasGroup != null ? indicatorCanvasGroup.alpha : 1f;
        float startY = indicator.anchoredPosition.y;

        activeTween.Kill();

        Tween resizeTween = DOVirtual.Float(0f, 1f, resizeDuration, _ =>
        {
            RectTransform layoutRootRect = targetRect.parent as RectTransform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutRootRect);
            Canvas.ForceUpdateCanvases();

            Bounds currentBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(parentRect, targetRect);
            float leftInset = currentBounds.min.x - parentLeft;
            float rightInset = parentRight - currentBounds.max.x;

            indicator.offsetMin = new Vector2(leftInset, startMinY);
            indicator.offsetMax = new Vector2(-rightInset, startMaxY);
        })
        .SetEase(closeEase)
        .SetUpdate(true);

        Tween fadeAndDropTween = DOTween.Sequence()
            .Join(indicator.DOAnchorPosY(startY - dropDistance, hideDuration).SetEase(closeEase))
            .Join(DOVirtual.Float(startAlpha, 0f, hideDuration, alphaValue =>
            {
                if (indicatorCanvasGroup != null) indicatorCanvasGroup.alpha = alphaValue;
            }).SetUpdate(true))
            .SetUpdate(true);

        activeTween = DOTween.Sequence()
            .Append(resizeTween)
            .Append(fadeAndDropTween)
            .OnComplete(() =>
            {
                indicator.anchoredPosition = new Vector2(indicator.anchoredPosition.x, startY);
                lastSelectedButton = null;
            });
    }
}
