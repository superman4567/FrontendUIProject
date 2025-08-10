using UnityEngine;
using UnityEngine.UI;

public class PopupShowButtonTrigger : MonoBehaviour, IPopupTrigger
{
    [SerializeField] private PopupEnums target;
    [SerializeField] private bool clearHistoryFirst = false;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    void OnEnable()
    {
        button.onClick.AddListener(Trigger);
    }

    void OnDisable()
    {
        button.onClick.RemoveListener(Trigger);
    }

    public void Trigger()
    {
        if (clearHistoryFirst) PopupManager.Instance.CloseAll();
        PopupManager.Instance.Show(target);
    }
}

