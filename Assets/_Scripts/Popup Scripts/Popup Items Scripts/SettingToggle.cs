using UnityEngine;
using UnityEngine.UI;

public class SettingToggle : MonoBehaviour
{
    [SerializeField] private Button toggleOn;
    [SerializeField] private Button toggleOff;

    private void Awake()
    {
        toggleOn.onClick.AddListener(() => SetToggleState(true));
        toggleOff.onClick.AddListener(() => SetToggleState(false));
    }

    private void SetToggleState(bool value)
    {
        UpdateVisual(value);
        HandleEffect();
    }

    private void UpdateVisual(bool value)
    {
        if (value)
        {
            toggleOn.gameObject.SetActive(true);
            toggleOff.gameObject.SetActive(false);
        }
        else
        {
            toggleOn.gameObject.SetActive(false);
            toggleOff.gameObject.SetActive(true);
        }
    }

    private void HandleEffect()
    {
        //To be implemented
    }
}
