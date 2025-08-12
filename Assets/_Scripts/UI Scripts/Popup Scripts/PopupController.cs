using UnityEngine;
using UnityEngine.UI;

public abstract class PopupControllerBase : MonoBehaviour, IPopup
{
    [SerializeField] private PopupEnums key;
    [SerializeField] private Button closeButton;

    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected PopupAnimator popupAnimator;

    public PopupEnums Key => key;
    public bool IsShown { get; private set; }

    protected virtual void Awake()
    {
        PopupManager.Instance.Register(this);
        closeButton.onClick.AddListener(HandleCloseClicked);

        popupAnimator.Shown += HandleShown;
        popupAnimator.Hidden += HandleHidden;
    }

    protected virtual void OnDestroy()
    {
        closeButton.onClick.RemoveListener(HandleCloseClicked);
        PopupManager.Instance.Unregister(this);

        popupAnimator.Shown -= HandleShown;
        popupAnimator.Hidden -= HandleHidden;
    }

    protected virtual void Start()
    {
        transform.parent.gameObject.SetActive(false);
        popupAnimator.InstantHide();
    }

    public void Show()
    {
        transform.parent.gameObject.SetActive(true);
        popupAnimator.Show();
    }

    public void Hide()
    {
        popupAnimator.Hide();
    }

    void HandleShown()
    {
        IsShown = true;
        OnOpened();
    }

    void HandleHidden()
    {
        IsShown = false;
        OnClosed();
        transform.parent.gameObject.SetActive(false);
    }

    void HandleCloseClicked()
    {
        if (!CanClose()) return;
        OnBeforeClose();
        PopupManager.Instance.CloseCurrent();
    }

    protected virtual void OnOpened() { }
    protected virtual void OnClosed() { }
    protected virtual void OnBeforeClose() { }
    protected virtual bool CanClose() => true;
}