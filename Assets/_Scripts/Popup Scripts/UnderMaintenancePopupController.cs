using UnityEngine;

public class UnderMaintenancePopupController : PopupControllerBase
{
    protected override void OnOpened()
    {
        // refresh inventory UI, play open SFX, etc.
    }

    protected override bool CanClose()
    {
        // return false to block close (e.g., while confirming an action)
        return true;
    }
}

