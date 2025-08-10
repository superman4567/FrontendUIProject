using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PopupController : MonoBehaviour
{
    [SerializeField] PopupEnums key;
    [SerializeField] bool startHidden = true;

    CanvasGroup popupCanvasGroup;

    public PopupEnums Key => key;

    void Awake()
    {
        popupCanvasGroup = GetComponent<CanvasGroup>();
        if (startHidden) InstantHide();
        else InstantShow();
    }

    void OnEnable() => PopupManager.Instance?.Register(this);
    void OnDisable() => PopupManager.Instance?.Unregister(this);

    public void InstantShow()
    {
        gameObject.SetActive(true);
        popupCanvasGroup.alpha = 1f;
        popupCanvasGroup.interactable = true;
        popupCanvasGroup.blocksRaycasts = true;
    }

    public void InstantHide()
    {
        popupCanvasGroup.alpha = 0f;
        popupCanvasGroup.interactable = false;
        popupCanvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false);
    }
}