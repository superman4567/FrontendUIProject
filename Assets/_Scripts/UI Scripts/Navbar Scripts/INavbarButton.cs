using System;
using System.Threading.Tasks;

public interface INavbarButton
{
    NavbarEnums ContentKey { get; }
    bool IsOn { get; }
    bool IsLocked { get; }

    event Action<INavbarButton> ToggledOn;
    event Action<INavbarButton> ToggledOff;
    event Action<INavbarButton> LockedPressed;

    void OnClick();
    void SetLocked(bool locked);

    Task PlayToggleOnAsync();
    Task PlayToggleOffAsync();
    Task PlayLockedAnimAsync();
}
