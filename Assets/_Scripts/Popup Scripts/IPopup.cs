public interface IPopup
{
    PopupEnums Key { get; }
    bool IsShown { get; }
    void InstantShow();
    void InstantHide();
    void Show();
    void Hide();
}
