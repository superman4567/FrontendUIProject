using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using TMPro;

public class SettingNext : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private TextMeshProUGUI nextButtonText;
    private readonly bool wrapAround = true;

    private void Awake()
    {
        nextButton.onClick.AddListener(() => SetToggleState());
    }

    private void SetToggleState()
    {
        UpdateVisual();
        HandleEffect();
    }

    private void UpdateVisual()
    {
        //To be implemented
    }

    private void HandleEffect()
    {
        var locales = LocalizationSettings.AvailableLocales?.Locales;
        if (locales == null || locales.Count == 0) return;

        var current = LocalizationSettings.SelectedLocale;
        int idx = Mathf.Max(0, locales.IndexOf(current));
        int next = idx + 1;

        if (next >= locales.Count)
        {
            if (!wrapAround) return;
            next = 0;
        }

        var target = locales[next];
        if (target == null || target == current) return;

        LocalizationSettings.SelectedLocale = target;
        nextButtonText.text = target.LocaleName;
    }
}
