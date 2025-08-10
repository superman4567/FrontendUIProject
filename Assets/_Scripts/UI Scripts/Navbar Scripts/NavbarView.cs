using System;
using System.Linq;
using UnityEngine;

public sealed class NavbarView : MonoBehaviour
{
    public event Action<NavbarEnums> ContentActivated;
    public event Action Closed;

    [SerializeField] private NavbarButton[] buttonBehaviours;
    [SerializeField] private NavbarAnimator navbarAnimator;

    INavbarButton[] buttons;
    NavbarEnums? activeKey = null;

    void Awake()
    {
        buttons = buttonBehaviours.OfType<INavbarButton>().ToArray();
        foreach (var button in buttons)
        {
            button.ToggledOn += OnButtonOn;
            button.ToggledOff += OnButtonOff;
            button.LockedPressed += OnLockedPressed;
        }
    }

    async void OnButtonOn(INavbarButton button)
    {
        if (button.IsLocked) return;

        foreach (var otherButton in buttons)
        {
            if (!ReferenceEquals(otherButton, button) && otherButton.IsOn)
                otherButton.OnClick();
        }

        await navbarAnimator.AppearIfHiddenAsync();

        activeKey = button.ContentKey;
        await button.PlayToggleOnAsync();

        ContentActivated?.Invoke(activeKey.Value);
    }

    async void OnButtonOff(INavbarButton button)
    {
        await button.PlayToggleOffAsync();

        bool anyOn = buttons.Any(existingButton => existingButton.IsOn);
        if (!anyOn)
        {
            activeKey = null;
            Closed?.Invoke();
        }
    }

    void OnLockedPressed(INavbarButton button)
    {
        // Optional: show locked feedback here
    }
}
