using System.Collections;

public interface IPopup
{
    PopupEnums Key { get; }
    bool IsShown { get; }
    void Show();
    void Hide();
}
