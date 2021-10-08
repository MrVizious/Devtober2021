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
    Rigidbody2D rb;
    Vector2 playerInput;
    Vector2 velocity;

    [Header("JUMP")]
    // Jump Variables
    [Range(0f, 10f)]
    public float jumpHeight = 2f;
    [Range(0f, 5f)]
    public float maxExtendedJumpHeight = 0.3f;
    private float jumpSpeed;
    private float maxExtendedJumpTime;
    [Range(0f, 4f)]
    public int maxNumberOfAirJumps = 1;
    private int timesJumpedOnAir = 0;
    private float timeSinceLastJump = float.MaxValue;

    [Header("JUMP TRICKS")]
    [Range(0f, 0.5f)]
    public float jumpTimeBufferBeforeGrounded = 0.15f;
    [Range(0f, 0.5f)]
    public float jumpTimeBufferCoyoteTime = 0.05f;
    public LayerMask groundLayers;

    // Jump button variables
    private bool jumpButtonJustPressed = false;
    private bool jumpButtonPressed = false;
    private float timeSinceJumpPressed = float.MaxValue;
    private float onAirTime = 0f;

    [Header("GROUND")]
    [SerializeField]
    private bool grounded = true;

    [Range(0f, 90f)]
    public float maxSlopeAngle = 33f;

    [Header("LADDER")]
    public float ladderSpeed = 2f;
    [SerializeField]
    private bool usingLadder = false;
    private GameObject ladderBehind;





    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * (jumpHeight + 0.05f * maxExtendedJumpHeight)) * (1 / 0.98f);
        maxExtendedJumpTime = SolveQuadratic((float)0.5 * Physics2D.gravity.y, jumpSpeed, -maxExtendedJumpHeight);

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
        UpdateLadder();
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

    /**********************
    ***     JUMPING     ***
    ***********************/

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
            else if (timeSinceLastJump <= maxExtendedJumpTime && timeSinceJumpPressed == timeSinceLastJump && jumpButtonPressed)
            {
                Debug.Log("Higher Jump");
                AddJumpSpeed();
            }
            else if (usingLadder && jumpButtonJustPressed)
            {
                ReleaseLadder();
                Jump();
            }

            onAirTime += Time.deltaTime;
        }

        // Reset pressed button variable
        jumpButtonJustPressed = false;
        timeSinceJumpPressed += Time.deltaTime;
        timeSinceLastJump += Time.deltaTime;
    }

    private void Jump() {
        grounded = false;
        AddJumpSpeed();
        ReleaseLadder();
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
            if (normal.y >= Mathf.Cos(maxSlopeAngle)) return true;
        }
        return false;
    }

    /**********************
    ***     LADDERS     ***
    ***********************/

    public void UseLadder(InputAction.CallbackContext context) {
        if (context.performed)
        {
            UseLadder();
        }
    }
    private void UseLadder() {
        if (!usingLadder && ladderBehind != null)
        {
            usingLadder = true;
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("LadderTop"), true);
            transform.position = new Vector2(ladderBehind.transform.position.x, transform.position.y);
        }
    }

    private void ReleaseLadder() {
        if (usingLadder)
        {
            usingLadder = false;
            rb.gravityScale = 1f;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("LadderTop"), false);
        }
    }

    public void ToggleLadder(InputAction.CallbackContext context) {
        if (usingLadder)
        {
            ReleaseLadder();
        }
        else UseLadder();
    }

    private void UpdateLadder() {
        if (Mathf.Abs(playerInput.y) >= Mathf.Epsilon)
        {
            UseLadder();
        }
        else if (Mathf.Abs(playerInput.x) >= Mathf.Epsilon)
        {
            ReleaseLadder();
        }
        if (usingLadder) rb.velocity = new Vector2(rb.velocity.x, ladderSpeed * playerInput.y);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag.Equals("Ladder"))
        {
            ladderBehind = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag.Equals("Ladder"))
        {
            if (ladderBehind != null && ladderBehind == other.gameObject)
            {
                ladderBehind = null;
                ReleaseLadder();
            }
        }
    }



    // quadratic equation is a second order of polynomial equation in a single variable 
    // x = [ -b +/- sqrt(b^2 - 4ac) ] / 2a
    public static float SolveQuadratic(float a_value, float b_value, float c_value) {
        float sqrtpart = b_value * b_value - 4 * a_value * c_value;
        float x1, x2;
        if (sqrtpart > 0)
        {
            x1 = (-b_value + Mathf.Sqrt(sqrtpart)) / (2 * a_value);
            x2 = (-b_value - Mathf.Sqrt(sqrtpart)) / (2 * a_value);
            return Mathf.Min(x1, x2);
        }
        else if (sqrtpart == 0)
        {
            return (-b_value + Mathf.Sqrt(sqrtpart)) / (2 * a_value);
        }
        return float.NaN;

    }
}