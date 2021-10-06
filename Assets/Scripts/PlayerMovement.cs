using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float maxHeightReached;
    [Header("MOVEMENT")]
    // Speed
    [Range(0, 20)]
    public float horizontalSpeed = 2f;
    [Range(0f, 100f)]
    public float horizontalAcceleration = 10f;
    [Range(0f, 100f)]
    public float horizontalAirAcceleration = 10f;

    [Header("JUMP")]
    // Jump Variables
    [Range(0f, 10f)]
    public float jumpHeight = 2f;
    private float jumpSpeed;
    private float maxExtendedJumpTime;
    [Range(0f, 4f)]
    public int maxNumberOfAirJumps = 1;
    public int timesJumpedOnAir = 0;
    [SerializeField]
    private float timeSinceLastJump = float.MaxValue;
    public float jumpTimeBufferBeforeGrounded = 0.15f;
    public float jumpTimeBufferCoyoteTime = 0.05f;
    public float maxExtendedJumpHeight = 0.3f;
    public LayerMask groundLayers;

    // Jump button variables
    private bool jumpButtonJustPressed = false;
    private bool jumpButtonPressed = false;
    [SerializeField]
    private float timeSinceJumpPressed = float.MaxValue;
    private float onAirTime = 0f;

    [Header("GROUND")]
    [SerializeField]
    private bool grounded = true;

    [Range(0f, 1f)]
    public float maxSlopeAngle = 0.9f;

    Vector2 playerInput;
    Vector2 velocity;



    Rigidbody2D rb;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
        maxExtendedJumpTime = (-jumpSpeed + Mathf.Sqrt(
                        jumpSpeed * jumpSpeed - 4 * (-maxExtendedJumpHeight) * Physics.gravity.y)
                        ) / 2 * (-maxExtendedJumpHeight);

        maxHeightReached = transform.position.y;
    }


    public void InputJump(InputAction.CallbackContext context) {
        if (context.performed)
        {
            jumpButtonJustPressed = true;
            jumpButtonPressed = true;
            timeSinceJumpPressed = 0f;
        }
        else if (context.canceled)
        {
            jumpButtonPressed = false;
        }
    }

    public void UpdateMovementInput(InputAction.CallbackContext context) {
        playerInput = context.ReadValue<Vector2>();
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);
    }

    private void FixedUpdate() {
        Movement();
        UpdateJump();
        if (transform.position.y > maxHeightReached) maxHeightReached = transform.position.y;
    }

    private void Movement() {

        Vector3 desiredHorizontalSpeed = new Vector3(playerInput.x, 0f, playerInput.y) * horizontalSpeed;

        float acceleration;
        if (grounded)
        {
            acceleration = horizontalAcceleration;
        }
        else
        {
            acceleration = horizontalAirAcceleration;
        }
        float maxSpeedChange = acceleration * Time.fixedDeltaTime;


        velocity = rb.velocity;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredHorizontalSpeed.x, maxSpeedChange);
        rb.velocity = velocity;
    }

    private void UpdateJump() {

        // Reset number of jumps
        if (grounded)
        {
            timesJumpedOnAir = 0;
            onAirTime = 0f;

            // Normal grounded jump
            if (jumpButtonJustPressed)
            {
                Jump();
                Debug.Log("Normal jump");
            }
            // Buffered grounded jump
            else if (jumpButtonPressed && timeSinceJumpPressed < jumpTimeBufferBeforeGrounded && timeSinceJumpPressed > 0)
            {
                Jump();
                Debug.Log("Buffered jump");
            }
        }
        else
        {
            // Coyote jump
            if (jumpButtonJustPressed && onAirTime < jumpTimeBufferCoyoteTime && onAirTime <= timeSinceLastJump)
            {
                Jump();
                Debug.Log("Coyote jump");
            }
            // Normal air jump
            else if (jumpButtonJustPressed && timesJumpedOnAir < maxNumberOfAirJumps)
            {
                Jump();
                onAirTime = 0f;
                timesJumpedOnAir++;
                Debug.Log("Air jump");
            }
            onAirTime += Time.deltaTime;
        }

        if (timeSinceLastJump < maxExtendedJumpTime && jumpButtonPressed)
        {
            Debug.Log("Higher Jump");
            AddJumpSpeed();
        }

        // Reset pressed button variable
        jumpButtonJustPressed = false;
        timeSinceJumpPressed += Time.deltaTime;
        timeSinceLastJump += Time.deltaTime;
    }

    private void Jump() {
        grounded = false;
        AddJumpSpeed();
        timeSinceLastJump = 0f;
        timeSinceJumpPressed = 0f;
    }

    private void AddJumpSpeed() {
        rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        grounded |= IsCollisionUnder(other);
    }
    private void OnCollisionExit2D(Collision2D other) {
        grounded = IsCollisionUnder(other);
    }
    private void OnCollisionStay2D(Collision2D other) {
        if (!jumpButtonPressed)
        {
            grounded |= IsCollisionUnder(other);
        }
    }

    private bool IsCollisionUnder(Collision2D collision) {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= maxSlopeAngle) return true;
        }
        return false;
    }
}