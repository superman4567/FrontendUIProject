using UnityEngine;
using UnityEngine.UI;

public abstract class PopupControllerBase : MonoBehaviour, IPopup
{
    [SerializeField] private PopupEnums key;
    [SerializeField] private Button closeButton;

    protected CanvasGroup canvasGroup;
    public PopupEnums Key => key;
    public bool IsShown { get; private set; }

    protected virtual void Awake()
    {
        canvasGroup = GetComponentInParent<CanvasGroup>();

        PopupManager.Instance.Register(this);
        closeButton.onClick.AddListener(HandleCloseClicked);
    }

    protected virtual void OnDestroy()
    {
        closeButton.onClick.RemoveListener(HandleCloseClicked);
        PopupManager.Instance.Unregister(this);
    }

    protected virtual void Start()
    {
        InstantHide();
    }

    public virtual void InstantShow()
    {
        var parentGO = transform.parent.gameObject;
        parentGO.SetActive(true);
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        IsShown = true;
        OnOpened();
    }

    public virtual void InstantHide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        var parentGO = transform.parent.gameObject;
        parentGO.SetActive(false);
        IsShown = false;
        OnClosed();
    }

    private void HandleCloseClicked()
    {
        if (!CanClose()) return;
        OnBeforeClose();
        PopupManager.Instance.CloseCurrent();
    }

    public void Show() => InstantShow();
    public void Hide() => InstantHide();

    protected virtual void OnOpened() { }
    protected virtual void OnClosed() { }
    protected virtual void OnBeforeClose() { }
    protected virtual bool CanClose() => true;
}
