using UnityEngine;

public class PopupShowTrigger : MonoBehaviour, IPopupTrigger
{
    [SerializeField] private PopupEnums target;
    [SerializeField] private bool clearHistoryFirst = false;

    public void Trigger()
    {
        if (clearHistoryFirst) PopupManager.Instance.CloseAll();
        PopupManager.Instance.Show(target);
    }
}
