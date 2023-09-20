using System;
using UnityEngine;

public class PlayerSaveManager : SaveManager
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private ItemSelectionController itemSelectionController;
    [SerializeField] private HealthController playerHealthController;

    public override SaveManagerState GetState()
    {
        Vector2 playerPosition = playerController.transform.position;
        int playerDirection = (int)playerController.Direction;
        int selectedItemIndex = itemSelectionController.SelectedItemIndex;
        int health = playerHealthController.CurrentHealth;

        var saveManagerState = new PlayerSaveManagerState
        {
            SaveGuid = saveGuid,
            PlayerPosition = playerPosition,
            PlayerDirection = playerDirection,
            SelectedItemIndex = selectedItemIndex,
            Health = health
        };

        return saveManagerState;
    }

    public override void UpdateFromState(SaveManagerState saveManagerState)
    {
        PlayerSaveManagerState playerState = (PlayerSaveManagerState)saveManagerState;

        playerController.transform.position = playerState.PlayerPosition;
        playerController.Direction = (CharacterDirection)playerState.PlayerDirection;
        itemSelectionController.SelectedItemIndex = playerState.SelectedItemIndex;
        playerHealthController.CurrentHealth = playerState.Health;
    }

    [Serializable]
    public class PlayerSaveManagerState : SaveManagerState
    {
        public Vector2 PlayerPosition;
        public int PlayerDirection;
        public int SelectedItemIndex;
        public int Health;
    }
}
