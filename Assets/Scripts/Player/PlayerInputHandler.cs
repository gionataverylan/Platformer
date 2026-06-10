using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputActions _inputActions;

    public Vector2 MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool JumpReleased { get; private set; }
    public bool JumpHeld { get; private set; }

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
    }

    private void Update()
    {
        MoveInput = _inputActions.Player.Move.ReadValue<Vector2>();

        JumpPressed = _inputActions.Player.Jump.WasPressedThisFrame();
        JumpReleased = _inputActions.Player.Jump.WasReleasedThisFrame();

        JumpHeld = _inputActions.Player.Jump.IsPressed();
    }
}