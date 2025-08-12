using System.Threading.Tasks;
using UnityEngine;

public class CheatMenu : MonoBehaviour
{
    [Header("Refs")]
    public NavbarView navbarView;
    public NavbarAnimator navbarAnimator;
    public NavbarButton[] navbarButtons;
    public GameObject levelCompletePanel;

    private void Start()
    {
        DeactivateAll();
    }

    public async void ShowNavbar()
    {
        await navbarAnimator.AppearIfHiddenAsync();
    }

    public async void HideNavbar()
    {
        await navbarAnimator.DisappearAsync();
    }

    public void ClickButton(int buttonIndex)
    {
        navbarButtons[buttonIndex].OnClick();
    }

    public void SetLocked(int buttonIndex, bool locked)
    {
        navbarButtons[buttonIndex].SetLocked(locked);
    }

    public async void ActivateOnly(int buttonIndex)
    {
        for (int i = 0; i < navbarButtons.Length; i++)
        {
            if (i == buttonIndex) continue;
            if (navbarButtons[i].IsOn)
                navbarButtons[i].OnClick();
        }

        await navbarAnimator.AppearIfHiddenAsync();

        if (!navbarButtons[buttonIndex].IsOn)
            navbarButtons[buttonIndex].OnClick();
    }

    public void DeactivateAll()
    {
        for (int i = 0; i < navbarButtons.Length; i++)
        {
            if (navbarButtons[i].IsOn)
                navbarButtons[i].OnClick();
        }
    }

    public void ClickButtonByKey(NavbarEnums key)
    {
        int buttonIndex = FindButtonIndexByKey(key);
        if (buttonIndex >= 0)
            navbarButtons[buttonIndex].OnClick();
    }

    public void SetLockedByKey(NavbarEnums key, bool locked)
    {
        int buttonIndex = FindButtonIndexByKey(key);
        if (buttonIndex >= 0)
            navbarButtons[buttonIndex].SetLocked(locked);
    }

    public async void ActivateOnlyByKey(NavbarEnums key)
    {
        int targetIndex = FindButtonIndexByKey(key);
        if (targetIndex < 0) return;

        for (int i = 0; i < navbarButtons.Length; i++)
        {
            if (i == targetIndex) continue;
            if (navbarButtons[i].IsOn)
                navbarButtons[i].OnClick();
        }

        await navbarAnimator.AppearIfHiddenAsync();

        if (!navbarButtons[targetIndex].IsOn)
            navbarButtons[targetIndex].OnClick();
    }

    int FindButtonIndexByKey(NavbarEnums key)
    {
        for (int i = 0; i < navbarButtons.Length; i++)
        {
            if (navbarButtons[i].ContentKey == key)
                return i;
        }
        return -1;
    }

    public void ToggleLevelComplete()
    {
        bool isActive = levelCompletePanel.activeSelf;
        levelCompletePanel.SetActive(!isActive); 
    }
}
