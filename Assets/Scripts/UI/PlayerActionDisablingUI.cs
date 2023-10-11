using UnityEngine;

public class PlayerActionDisablingUI : MonoBehaviour
{
    [SerializeField] private PlayerActionDisablingUIManager manager;

    private void OnEnable() => manager.ActionDisablingUIOpen = true;

    private void OnDisable() => manager.ActionDisablingUIOpen = false;
}
