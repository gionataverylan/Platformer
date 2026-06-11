using UnityEngine;

[CreateAssetMenu(menuName = "Player/PlayerData", fileName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Движение")]

    public float runMaxSpeed = 10f;
    public float runAccelerationTime = 0.1f;
    public float runDecelerationTime = 0.15f;

    [Header("Прыжок")]

    public float jumpHeight = 4f;
    public float jumpTimeToApex = 0.4f;
    public float fallGravityMultiplier = 2.5f;
    public float jumpCutGravityMultiplier = 2f;

    [Header("=== COYOTE TIME и JUMP BUFFER ===")]

    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;
    public float GravityStrength => -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
    public float JumpForce => Mathf.Abs(GravityStrength) * jumpTimeToApex;
}
