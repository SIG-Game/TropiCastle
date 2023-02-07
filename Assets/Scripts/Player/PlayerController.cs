using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool isAttacking;
    [SerializeField] private Sprite front, back, left, right;
    [SerializeField] private FishingMinigame fishingMinigame;

    public PlayerDirection LastDirection
    {
        get => lastDirection;
        set
        {
            lastDirection = value;

            spriteRenderer.sprite = lastDirection switch
            {
                PlayerDirection.Up => back,
                PlayerDirection.Down => front,
                PlayerDirection.Left => left,
                PlayerDirection.Right => right,
                _ => throw new ArgumentOutOfRangeException(nameof(lastDirection))
            };
        }
    }

    public static event Action OnPlayerDied = delegate { };

    private Animator animator;
    private BoxCollider2D boxCollider;
    private HealthController healthController;
    private Inventory inventory;
    private ItemSelectionController itemSelectionController;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer weaponSpriteRenderer;
    private WeaponController weaponController;
    private PlayerDirection lastDirection;
    private LayerMask interactableMask;
    private LayerMask waterMask;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        healthController = GetComponent<HealthController>();
        inventory = GetComponent<Inventory>();
        itemSelectionController = GetComponent<ItemSelectionController>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        weaponSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        weaponController = transform.GetChild(0).GetComponent<WeaponController>();

        interactableMask = LayerMask.GetMask("Interactable");
        waterMask = LayerMask.GetMask("Water");

        isAttacking = false;

        LastDirection = PlayerDirection.Down;

        healthController.OnHealthChanged += HealthController_OnHealthChanged;
    }

    private void Update()
    {
        if (PauseController.Instance.GamePaused)
        {
            return;
        }

        if (InputManager.Instance.GetLeftClickDownIfUnusedThisFrame())
        {
            UseItem(GetSelectedItem());

            // Prevent doing other actions on the frame an attack starts
            if (isAttacking)
            {
                return;
            }
        }

        if (InputManager.Instance.GetInteractButtonDownIfUnusedThisFrame())
        {
            RaycastHit2D hit = InteractionCast(interactableMask, 0.15f);

            if (hit.collider != null)
            {
                hit.transform.gameObject.GetComponent<Interactable>().Interact(this);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Bullet"))
        {
            healthController.DecreaseHealth(10);
        }
    }

    private void OnDestroy()
    {
        healthController.OnHealthChanged -= HealthController_OnHealthChanged;

        OnPlayerDied = delegate { };
    }

    private Vector2 GetInteractionDirection()
    {
        Vector2 interactionDirection = lastDirection switch
        {
            PlayerDirection.Up => Vector2.up,
            PlayerDirection.Down => Vector2.down,
            PlayerDirection.Left => Vector2.left,
            PlayerDirection.Right => Vector2.right,
            _ => throw new ArgumentOutOfRangeException(nameof(lastDirection))
        };
        return interactionDirection;
    }

    private RaycastHit2D InteractionCast(LayerMask mask, float boxCastDistance)
    {
        Vector2 interactionDirection = GetInteractionDirection();

        Vector3 raycastOrigin = transform.position;
        raycastOrigin.x += boxCollider.offset.x;
        raycastOrigin.y += boxCollider.offset.y;

        // TODO: Shorten length and change length based on direction
        RaycastHit2D hit = Physics2D.BoxCast(raycastOrigin,
            boxCollider.size, 0f, interactionDirection, boxCastDistance, mask);

        // Debug.DrawRay(raycastOrigin, interactionDirection * 0.25f, Color.red);

        return hit;
    }

    private void UseItem(ItemWithAmount item)
    {
        switch (item.itemData)
        {
            case HealingItemScriptableObject healingItemData:
                ConsumeHealingItem(item, healingItemData.healAmount);
                break;
            case WeaponItemScriptableObject weaponItemData:
                AttackWithWeapon(weaponItemData);
                break;
            case { name: "FishingRod" }:
                if (InteractionCast(waterMask, 0.5f).collider == null)
                {
                    DialogueBox.Instance.PlayDialogue("You must be facing water to fish.");
                    break;
                }

                StartCoroutine(fishingMinigame.StartFishing());
                break;
            default:
                Debug.Log($"Used item named {item.itemData.name}, which has no usage defined.");
                break;
        }
    }

    private void ConsumeHealingItem(ItemWithAmount item, int amountToHeal)
    {
        if (!healthController.AtMaxHealth())
        {
            healthController.IncreaseHealth(amountToHeal);
            inventory.RemoveItem(item);
        }
    }

    private void AttackWithWeapon(WeaponItemScriptableObject weaponItemData)
    {
        weaponSpriteRenderer.sprite = weaponItemData.weaponSprite;
        weaponController.damage = weaponItemData.damage;

        animator.Play($"Swing {lastDirection}");

        isAttacking = true;
    }

    private void PlayerDeath()
    {
        PauseController.Instance.GamePaused = true;
        OnPlayerDied();
    }

    private void HealthController_OnHealthChanged(int newHealth)
    {
        if (newHealth == 0)
        {
            PlayerDeath();
        }
    }

    public bool CanMove() => !isAttacking && !PauseController.Instance.GamePaused && !DialogueBox.Instance.DialogueBoxOpen();

    public ItemWithAmount GetSelectedItem() => inventory.GetItemAtIndex(itemSelectionController.SelectedItemIndex);

    public Inventory GetInventory() => inventory;
}
