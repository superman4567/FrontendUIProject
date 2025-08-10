using UnityEngine;
using UnityEngine.UI;

public class SettingNext : MonoBehaviour
{
    [SerializeField] private Button nextButton;

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
        //To be implemented
    }
}
