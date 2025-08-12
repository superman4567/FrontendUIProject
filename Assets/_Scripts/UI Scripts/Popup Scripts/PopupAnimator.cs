using System;
using DG.Tweening;
using UnityEngine;

public class PopupAnimator : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] RectTransform rect;

    [Header("Positions & Ease")]
    [SerializeField] float shownY = 0f;
    [SerializeField] float hiddenY = -512f;
    [SerializeField] Ease ease = Ease.OutCubic;

    [Header("Show (runs together)")]
    [SerializeField] float showDuration = 0.35f;
    [SerializeField] AnimationCurve showScaleCurve = null;

    [Header("Hide (scale+slide, then fade)")]
    [SerializeField] float hideScaleDuration = 0.25f;
    [SerializeField] float hideFadeDuration = 0.20f;
    [SerializeField] AnimationCurve hideScaleCurve = null;

    public event Action Shown;
    public event Action Hidden;

    Sequence showSeq;
    Sequence hideSeq;

    void Awake()
    {
        BuildSequences();
        SetHiddenInstant();
    }

    public void Show()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        hideSeq?.Pause();
        showSeq.Restart();
    }

    public void Hide()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        showSeq?.Pause();
        hideSeq.Restart();
    }

    public void InstantShow()
    {
        showSeq?.Kill();
        hideSeq?.Kill();
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, shownY);
        rect.localScale = Vector3.one;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        BuildSequences();
        Shown?.Invoke();
    }

    public void InstantHide()
    {
        showSeq?.Kill();
        hideSeq?.Kill();
        SetHiddenInstant();
        BuildSequences();
        Hidden?.Invoke();
    }

    void BuildSequences()
    {
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, shownY);
        rect.localScale = Vector3.one;
        canvasGroup.alpha = 1f;

        var fadeIn = DOTween.To(() => canvasGroup.alpha, a => canvasGroup.alpha = a, 1f, showDuration)
                            .From(0f).SetEase(ease);

        var slideIn = DOTween.To(() => rect.anchoredPosition.y, y =>
        { var p = rect.anchoredPosition; p.y = y; rect.anchoredPosition = p; },
                        shownY, showDuration)
                        .From(hiddenY).SetEase(ease);

        var scaleIn = DOTween.To(() => 0f, v =>
        { var s = Mathf.LerpUnclamped(0f, 1f, v); rect.localScale = new Vector3(s, s, s); },
                        1f, showDuration)
                        .SetEase(showScaleCurve ?? AnimationCurve.Linear(0, 0, 1, 1));

        showSeq = DOTween.Sequence()
            .SetAutoKill(false)
            .Pause()
            .Append(fadeIn)
            .Join(slideIn)
            .Join(scaleIn)
            .OnComplete(() => Shown?.Invoke());

        var scaleOut = DOTween.To(() => 0f, v =>
        { var s = Mathf.LerpUnclamped(1f, 0f, v); rect.localScale = new Vector3(s, s, s); },
                        1f, hideScaleDuration)
                        .SetEase(hideScaleCurve ?? AnimationCurve.Linear(0, 1, 1, 0));

        var slideOut = DOTween.To(() => rect.anchoredPosition.y, y =>
        { var p = rect.anchoredPosition; p.y = y; rect.anchoredPosition = p; },
                        hiddenY, hideScaleDuration)
                        .SetEase(ease);

        var fadeOut = DOTween.To(() => canvasGroup.alpha, a => canvasGroup.alpha = a, 0f, hideFadeDuration)
                             .SetEase(ease);

        hideSeq = DOTween.Sequence()
            .SetAutoKill(false)
            .Pause()
            .Append(scaleOut)
            .Join(slideOut)
            .Append(fadeOut)
            .OnComplete(() => Hidden?.Invoke());
    }

    void SetHiddenInstant()
    {
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, hiddenY);
        rect.localScale = Vector3.zero;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}