using UnityEngine;
using UnityEngine.UI;

public class DebugAddItemDropdownTemplate : MonoBehaviour
{
    [SerializeField] private DebugAddItemDropdownController debugAddItemDropdownController;

    [Inject] private ItemSelectionController itemSelectionController;

    private ScrollRect scrollRect;

    private void Awake()
    {
        this.InjectDependencies();

        itemSelectionController.CanScroll = false;
    }

    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();

        // debugAddItemDropdownController.ScrollRectVerticalNormalizedPosition
        // is set in the Awake method of that script
        scrollRect.verticalNormalizedPosition =
            debugAddItemDropdownController.ScrollRectVerticalNormalizedPosition;

        scrollRect.onValueChanged.AddListener(ScrollRect_OnValueChanged);
    }

    private void ScrollRect_OnValueChanged(Vector2 value)
    {
        debugAddItemDropdownController.ScrollRectVerticalNormalizedPosition = value.y;
    }

    private void OnDestroy()
    {
        itemSelectionController.CanScroll = true;
    }
}
