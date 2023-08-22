using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class EventSystemDefaultGameObjectSelector : MonoBehaviour
{
    [SerializeField] private GameObject defaultSelectedGameObject;
    [SerializeField] private InputManager inputManager;

    private PlayerInput playerInput;
    private InputAction navigateAction;
    private InputAction submitAction;

    public static EventSystemDefaultGameObjectSelector Instance;

    private void Awake()
    {
        Instance = this;

        InputActionAsset inputActionAsset =
            GetComponent<InputSystemUIInputModule>().actionsAsset;

        navigateAction = inputActionAsset["Navigate"];
        submitAction = inputActionAsset["Submit"];
    }

    private void Start()
    {
        playerInput = inputManager.GetPlayerInput();
    }

    private void Update()
    {
        bool selectDefaultInputPressedThisFrame =
            navigateAction.WasPressedThisFrame() || submitAction.WasPressedThisFrame();

        if (selectDefaultInputPressedThisFrame &&
            EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(defaultSelectedGameObject);
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void SetDefaultSelectedGameObject(GameObject defaultSelectedGameObject)
    {
        this.defaultSelectedGameObject = defaultSelectedGameObject;

        if (playerInput.currentControlScheme == "Gamepad")
        {
            EventSystem.current.SetSelectedGameObject(defaultSelectedGameObject);
        }
    }

    public void SelectGameObjectIfSubmitPressedThisFrame(GameObject selectedGameObject)
    {
        if (submitAction.WasPressedThisFrame())
        {
            EventSystem.current.SetSelectedGameObject(selectedGameObject);
        }
    }
}
