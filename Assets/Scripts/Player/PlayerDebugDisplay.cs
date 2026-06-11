using UnityEngine;

public class PlayerDebugDisplay : MonoBehaviour
{
    private PlayerController _controller;
    private PlayerInputHandler _input;
    private Rigidbody2D _rb;

    private GUIStyle _style;

    private void Awake()
    {
        _controller = GetComponent<PlayerController>();
        _input = GetComponent<PlayerInputHandler>();
        _rb = GetComponent<Rigidbody2D>();

        _style = new GUIStyle();
        _style.fontSize = 14;
        _style.normal.textColor = Color.white;
    }

    private void OnGUI()
    {
        var sb = new System.Text.StringBuilder();

        sb.AppendLine($"=== PLAYER DEBUG ===");
        sb.AppendLine($"Velocity:  {_rb.linearVelocity:F2}");
        sb.AppendLine($"Grounded:  {_controller.IsGrounded}");
        sb.AppendLine($"Jumping:   {_controller.IsJumping}");
        sb.AppendLine($"JumpCut:   {_controller.IsJumpCut}");
        sb.AppendLine($"CoyoteT:   {_controller.CoyoteTimeCounter:F2}");
        sb.AppendLine($"JumpBuf:   {_controller.JumpBufferCounter:F2}");
        sb.AppendLine($"GravScale: {_rb.gravityScale:F2}");
        sb.AppendLine($"MoveInput: {_input.MoveInput.x:F2}");

        GUI.Label(new Rect(10, 10, 300, 300), sb.ToString(), _style);
    }
}