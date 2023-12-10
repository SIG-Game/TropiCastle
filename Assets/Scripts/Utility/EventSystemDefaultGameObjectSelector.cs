using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class EventSystemDefaultGameObjectSelector : MonoBehaviour
{
    [SerializeField] private GameObject defaultSelectedGameObject;
    [SerializeField] private PlayerInput playerInput;

    private InputAction navigateAction;
    private InputAction submitAction;

    private void Awake()
    {
        InputActionAsset inputActionAsset =
            GetComponent<InputSystemUIInputModule>().actionsAsset;

        navigateAction = inputActionAsset["Navigate"];
        submitAction = inputActionAsset["Submit"];
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

    public void SelectNull()
    {
        EventSystem.current.SetSelectedGameObject(null);
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
