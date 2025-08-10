using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public sealed class NavbarAnimator : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private RectTransform navbarTransform;
    [SerializeField] private CanvasGroup navbarCanvasGroup;
    [SerializeField] private RectTransform navbarBackgroundTransform;
    [SerializeField] private CanvasGroup navbarBackgroundCanvasGroup;

    [Header("Timing")]
    [SerializeField] private float duration = 0.25f;
    [SerializeField] private AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private bool ignoreTimeScale = true;

    [Header("Position")]
    [SerializeField] private float hiddenYOffset = -238f;
    [SerializeField] private float backgroundHiddenYOffset = -238f;

    public bool IsVisible { get; private set; }

    float navbarVisibleY;
    float backgroundVisibleY;
    Sequence animationSequence;

    void Awake()
    {
        animationSequence = DOTween.Sequence();
        navbarVisibleY = navbarTransform.anchoredPosition.y;
        backgroundVisibleY = navbarBackgroundTransform.anchoredPosition.y;
    }

    void OnDisable()
    {
        KillSequence();
    }

    public async Task AppearIfHiddenAsync()
    {
        if (IsVisible) return;

        KillSequence();
        PrepareForAppear();

        var taskCompletionSource = new TaskCompletionSource<bool>();

        animationSequence = DOTween.Sequence().SetEase(ease).SetUpdate(ignoreTimeScale);
        animationSequence.Join(navbarTransform.DOAnchorPosY(navbarVisibleY, duration));
        animationSequence.Join(navbarCanvasGroup.DOFade(1f, duration));
        animationSequence.Join(navbarBackgroundTransform.DOAnchorPosY(backgroundVisibleY, duration));
        animationSequence.Join(navbarBackgroundCanvasGroup.DOFade(1f, duration));
        animationSequence.OnComplete(() =>
        {
            IsVisible = true;
            navbarCanvasGroup.interactable = true;
            taskCompletionSource.TrySetResult(true);
        });

        await taskCompletionSource.Task;
    }

    public async Task DisappearAsync()
    {
        if (!IsVisible) return;

        KillSequence();
        PrepareForDisappear();

        var taskCompletionSource = new TaskCompletionSource<bool>();

        animationSequence = DOTween.Sequence().SetEase(ease).SetUpdate(ignoreTimeScale);
        animationSequence.Join(navbarTransform.DOAnchorPosY(navbarVisibleY + hiddenYOffset, duration));
        animationSequence.Join(navbarCanvasGroup.DOFade(0f, duration));
        animationSequence.Join(navbarBackgroundTransform.DOAnchorPosY(backgroundVisibleY + backgroundHiddenYOffset, duration));
        animationSequence.Join(navbarBackgroundCanvasGroup.DOFade(0f, duration));
        animationSequence.OnComplete(() =>
        {
            IsVisible = false;
            taskCompletionSource.TrySetResult(true);
        });

        await taskCompletionSource.Task;
    }

    void PrepareForAppear()
    {
        navbarCanvasGroup.blocksRaycasts = true;
        navbarCanvasGroup.interactable = false;

        Vector2 navbarPosition = navbarTransform.anchoredPosition;
        navbarTransform.anchoredPosition = new Vector2(navbarPosition.x, navbarVisibleY + hiddenYOffset);
        navbarCanvasGroup.alpha = 0f;

        Vector2 backgroundPosition = navbarBackgroundTransform.anchoredPosition;
        navbarBackgroundTransform.anchoredPosition = new Vector2(backgroundPosition.x, backgroundVisibleY + backgroundHiddenYOffset);
        navbarBackgroundCanvasGroup.alpha = 0f;
    }

    void PrepareForDisappear()
    {
        navbarCanvasGroup.interactable = false;
        navbarCanvasGroup.blocksRaycasts = false;
    }

    void SetHiddenImmediate()
    {
        KillSequence();

        Vector2 navbarPosition = navbarTransform.anchoredPosition;
        navbarTransform.anchoredPosition = new Vector2(navbarPosition.x, navbarVisibleY + hiddenYOffset);
        navbarCanvasGroup.alpha = 0f;
        navbarCanvasGroup.interactable = false;
        navbarCanvasGroup.blocksRaycasts = false;

        Vector2 backgroundPosition = navbarBackgroundTransform.anchoredPosition;
        navbarBackgroundTransform.anchoredPosition = new Vector2(backgroundPosition.x, backgroundVisibleY + backgroundHiddenYOffset);
        navbarBackgroundCanvasGroup.alpha = 0f;

        IsVisible = false;
    }

    void KillSequence()
    {
        animationSequence.Kill();
        animationSequence = DOTween.Sequence();
    }
}
