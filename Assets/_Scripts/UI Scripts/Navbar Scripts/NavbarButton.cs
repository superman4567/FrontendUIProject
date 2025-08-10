using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using UnityEngine.Localization;

public class NavbarButton : MonoBehaviour, INavbarButton
{
    [Header("Button Type")]
    [SerializeField] private NavbarEnums buttonType;
    [SerializeField] private LocalizedString stringreference;
    public static event Action<NavbarButton, RectTransform> ButtonSelected;

    public event Action<INavbarButton> ToggledOn;
    public event Action<INavbarButton> ToggledOff;
    public event Action<INavbarButton> LockedPressed;

    [Header("Icon")]
    [SerializeField] private RectTransform icon;

    [Header("Text")]
    [SerializeField] private CanvasGroup textCanvasGroup;

    [Header("UI")]
    [SerializeField] private Button button;
    [SerializeField] private LayoutElement layoutElement;
    [SerializeField] private RectTransform bodyTransform;

    [Header("Tween Settings")]
    [SerializeField] private float iconYIconOnly = 24f;
    [SerializeField] private float iconYWithText = 86f;
    [SerializeField] private float layoutWidthIconOnly = 1f;
    [SerializeField] private float layoutWidthWithText = 1.6f;
    [SerializeField] private float duration = 0.25f;
    [SerializeField] private AnimationCurve iconMoveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve textFadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve layoutWidthCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Sequence animationSequence;

    public NavbarEnums ContentKey => buttonType;
    public bool IsOn { get; private set; }
    public bool IsLocked { get; private set; }

    void Awake()
    {
        animationSequence = DOTween.Sequence();
        button.onClick.AddListener(OnClick);
    }

    void OnDisable()
    {
        KillTween();
    }

    public void OnClick()
    {
        if (IsLocked)
        {
            _ = PlayLockedAnimAsync();
            LockedPressed?.Invoke(this);
            return;
        }

        if (!IsOn)
        {
            IsOn = true;
            ToggledOn?.Invoke(this);
            ButtonSelected?.Invoke(this, bodyTransform);
        }
        else
        {
            IsOn = false;
            ToggledOff?.Invoke(this);
        }
    }

    public void SetLocked(bool locked)
    {
        IsLocked = locked;
    }

    public void SetOff()
    {
        if (!IsOn) return;
        IsOn = false;
        _ = PlayToggleOffAsync();
    }

    public Task PlayToggleOnAsync()
    {
        var tcs = new TaskCompletionSource<bool>();

        KillTween();
        animationSequence = DOTween.Sequence();
        animationSequence.Join(icon.DOAnchorPosY(iconYWithText, duration).SetEase(iconMoveCurve));
        animationSequence.Join(textCanvasGroup.DOFade(1f, duration).SetEase(textFadeCurve));
        animationSequence.Join(DOTween.To(() => layoutElement.flexibleWidth, v => layoutElement.flexibleWidth = v, layoutWidthWithText, duration).SetEase(layoutWidthCurve));
        animationSequence.OnComplete(() =>
        {
            tcs.TrySetResult(true);
        });

        return tcs.Task;
    }

    public Task PlayToggleOffAsync()
    {
        var tcs = new TaskCompletionSource<bool>();

        KillTween();
        animationSequence = DOTween.Sequence();
        animationSequence.Join(icon.DOAnchorPosY(iconYIconOnly, duration).SetEase(iconMoveCurve));
        animationSequence.Join(textCanvasGroup.DOFade(0f, duration).SetEase(textFadeCurve));
        animationSequence.Join(DOTween.To(() => layoutElement.flexibleWidth, v => layoutElement.flexibleWidth = v, layoutWidthIconOnly, duration).SetEase(layoutWidthCurve));
        animationSequence.OnComplete(() =>
        {
            tcs.TrySetResult(true);
        });

        return tcs.Task;
    }

    public Task PlayLockedAnimAsync()
    {
        // Could add a shake or highlight animation here for feedback
        return Task.CompletedTask;
    }

    void KillTween()
    {
        animationSequence.Kill();
    }
}
