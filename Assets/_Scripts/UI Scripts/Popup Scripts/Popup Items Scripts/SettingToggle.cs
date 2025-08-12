using UnityEngine;
using UnityEngine.UI;

public class SettingToggle : MonoBehaviour
{
    [SerializeField] private Button toggleButton;
    [SerializeField] private Image toggleOn;
    [SerializeField] private Image toggleOff;
    [SerializeField] private bool startOn = false;

    public bool IsOn { get; private set; }

    void Awake()
    {
        IsOn = startOn;
        ApplyVisual();
        toggleButton.onClick.AddListener(Toggle);
    }

    void OnDestroy()
    {
        toggleButton.onClick.RemoveListener(Toggle);
    }

    void Toggle()
    {
        IsOn = !IsOn;
        ApplyVisual();
        HandleEffect();
    }

    void ApplyVisual()
    {
        toggleOn.gameObject.SetActive(IsOn);
        toggleOff.gameObject.SetActive(!IsOn);
    }

    void HandleEffect()
    {
        // Example: DepthOfFieldController.OnDepthOfFieldToggle?.Invoke(IsOn);
    }
}
