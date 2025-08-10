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
    [SerializeField] private bool ignoreTimeScale = true;

    [Header("Animation Curves")]
    [SerializeField] private AnimationCurve heightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve opacityCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Height Settings")]
    [SerializeField] private float visibleHeight = 242f;
    [SerializeField] private float hiddenHeight = 0f;

    public bool IsVisible { get; private set; }

    private float backgroundVisibleHeight;
    private float backgroundHiddenHeight;
    private Sequence animationSequence;

    void Awake()
    {
        animationSequence = DOTween.Sequence();

        backgroundVisibleHeight = navbarBackgroundTransform.sizeDelta.y;
        backgroundHiddenHeight = hiddenHeight;
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

        var tcs = new TaskCompletionSource<bool>();

        animationSequence = DOTween.Sequence().SetUpdate(ignoreTimeScale);

        animationSequence.Join(DOVirtual.Float(hiddenHeight, visibleHeight, duration, h =>
        {
            Vector2 size = navbarTransform.sizeDelta;
            size.y = h;
            navbarTransform.sizeDelta = size;
        }).SetEase(heightCurve));

        animationSequence.Join(navbarCanvasGroup
            .DOFade(1f, duration)
            .SetEase(opacityCurve));

        animationSequence.Join(DOVirtual.Float(backgroundHiddenHeight, backgroundVisibleHeight, duration, h =>
        {
            Vector2 size = navbarBackgroundTransform.sizeDelta;
            size.y = h;
            navbarBackgroundTransform.sizeDelta = size;
        }).SetEase(heightCurve));

        animationSequence.Join(navbarBackgroundCanvasGroup
            .DOFade(1f, duration)
            .SetEase(opacityCurve));

        animationSequence.OnComplete(() =>
        {
            IsVisible = true;
            navbarCanvasGroup.interactable = true;
            tcs.TrySetResult(true);
        });

        await tcs.Task;
    }

    public async Task DisappearAsync()
    {
        if (!IsVisible) return;

        KillSequence();
        PrepareForDisappear();

        var tcs = new TaskCompletionSource<bool>();

        animationSequence = DOTween.Sequence().SetUpdate(ignoreTimeScale);

        animationSequence.Join(DOVirtual.Float(visibleHeight, hiddenHeight, duration, h =>
        {
            Vector2 size = navbarTransform.sizeDelta;
            size.y = h;
            navbarTransform.sizeDelta = size;
        }).SetEase(heightCurve));

        animationSequence.Join(navbarCanvasGroup
            .DOFade(0f, duration)
            .SetEase(opacityCurve));

        animationSequence.Join(DOVirtual.Float(backgroundVisibleHeight, backgroundHiddenHeight, duration, h =>
        {
            Vector2 size = navbarBackgroundTransform.sizeDelta;
            size.y = h;
            navbarBackgroundTransform.sizeDelta = size;
        }).SetEase(heightCurve));

        animationSequence.Join(navbarBackgroundCanvasGroup
            .DOFade(0f, duration)
            .SetEase(opacityCurve));

        animationSequence.OnComplete(() =>
        {
            IsVisible = false;
            tcs.TrySetResult(true);
        });

        await tcs.Task;
    }

    void PrepareForAppear()
    {
        navbarCanvasGroup.blocksRaycasts = true;
        navbarCanvasGroup.interactable = false;

        navbarTransform.sizeDelta = new Vector2(navbarTransform.sizeDelta.x, hiddenHeight);
        navbarCanvasGroup.alpha = 0f;

        navbarBackgroundTransform.sizeDelta = new Vector2(navbarBackgroundTransform.sizeDelta.x, backgroundHiddenHeight);
        navbarBackgroundCanvasGroup.alpha = 0f;
    }

    void PrepareForDisappear()
    {
        navbarCanvasGroup.interactable = false;
        navbarCanvasGroup.blocksRaycasts = false;
    }

    void KillSequence()
    {
        animationSequence.Kill();
        animationSequence = DOTween.Sequence();
    }
}