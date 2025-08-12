using UnityEngine;

public class LevelCompleteManager : MonoBehaviour
{
    [SerializeField] private GameObject levelCompletePanel;

    private void Start()
    {
        levelCompletePanel.SetActive(false);
    }
}
