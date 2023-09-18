using System;
using UnityEngine;

public class PlayerSaveManager : SaveManager
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private ItemSelectionController itemSelectionController;
    [SerializeField] private HealthController playerHealthController;

    public override SavableState GetSavableState()
    {
        Vector2 playerPosition = playerController.transform.position;
        int playerDirection = (int)playerController.Direction;
        int selectedItemIndex = itemSelectionController.SelectedItemIndex;
        int health = playerHealthController.CurrentHealth;

        var savableState = new SavablePlayerState
        {
            SaveGuid = saveGuid,
            PlayerPosition = playerPosition,
            PlayerDirection = playerDirection,
            SelectedItemIndex = selectedItemIndex,
            Health = health
        };

        return savableState;
    }

    public override void SetPropertiesFromSavableState(SavableState savableState)
    {
        SavablePlayerState playerState = (SavablePlayerState)savableState;

        playerController.transform.position = playerState.PlayerPosition;
        playerController.Direction = (CharacterDirection)playerState.PlayerDirection;
        itemSelectionController.SelectedItemIndex = playerState.SelectedItemIndex;
        playerHealthController.CurrentHealth = playerState.Health;
    }

    [Serializable]
    public class SavablePlayerState : SavableState
    {
        public Vector2 PlayerPosition;
        public int PlayerDirection;
        public int SelectedItemIndex;
        public int Health;

        public override Type GetSavableClassType() =>
            typeof(PlayerSaveManager);
    }
}
