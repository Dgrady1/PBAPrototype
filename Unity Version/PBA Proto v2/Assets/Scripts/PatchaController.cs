using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PatchaController : MonoBehaviour
{
    // Patcha Variables
    public int maxHealth = 100;
    public int currentHealth;
    public int Lives = 3;
    public int Pickups = 0;
    public int pickupCounter;
    private float customGravity = -15f;

    // Movement variables
    public float speed = 10f;

    public Vector3 move;

    // Component References
    private Rigidbody rb;

    // Script References
    public HealthBar healthBar;

    // Jumping Variables
    public float jumpForce = 5.0f;
    public bool isJumpPressed;
    public bool isJumping;
    private float jumpCooldown = 0.1f;
    private float jumpCooldownTimer;
    public bool isGrounded = true;
    public float ascentSpeed = 10f; 
    public float ascentDuration = 0.5f; 
    private float jumpTimer = 0f;
    public float ascentSpeedMultiplier = 2.0f;


    private void OnEnable()
    {
        InputSystem.EnableDevice((InputSystem.GetDevice<Keyboard>()));
    }

    private void OnDisable()
    {
        InputSystem.DisableDevice((InputSystem.GetDevice<Keyboard>()));
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialise components
        rb = GetComponent<Rigidbody>();

        // Initialise variables
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isJumpPressed = true;
        }
        else if (context.canceled)
        {
            isJumpPressed = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Damage test
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(20);
        }
        // Gain life Test
        if (Input.GetKeyDown(KeyCode.L))
        {
            AddLife(1);
        }
        // Lose Life Test
        if (currentHealth <= 0)
        {
            LoseLife(1);
            currentHealth = maxHealth;
            healthBar.SetMaxHealth(maxHealth);
        }
        // Pickup Test
        if (Input.GetKeyDown(KeyCode.P))
        {
            PickupFunc(1);
        }
        // Life from pickup Test
        if (pickupCounter >= 100)
        {
            AddLife(1);
            pickupCounter = 0;
            Debug.Log("You have gained a life from Pickups");
        }

        // Check if the character is grounded
        isGrounded = CheckGrounded();

        movePlayer();
        handleJump();

        if (transform.position.y < 2)
        {
            Vector3 newPos = transform.position;
            newPos.y = 2;
            transform.position = newPos;
        }
    }

    void FixedUpdate()
    {
        //movePlayer();
        //handleJump();
    }

    public void movePlayer()
    {
        Vector3 movement = new Vector3(move.x, 0f, move.y);

        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15f);
        }

        transform.Translate(movement * speed * Time.deltaTime, Space.World);
    }

    private void handleJump()
    {
        if (isJumpPressed && isGrounded && !isJumping && jumpCooldownTimer <= 0f)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            isJumpPressed = false;
            isJumping = true; // Set jumping flag
            jumpCooldownTimer = jumpCooldown;
        }
        else if (!isJumpPressed && isJumping)
        {
            if (rb.velocity.y > 0f && jumpTimer < ascentDuration)
            {
                // Increase the upward velocity during the ascent
                rb.velocity += Vector3.up * ascentSpeed * Time.deltaTime;
                jumpTimer += Time.deltaTime;
            }
            else
            {
                isJumping = false; // Reset jumping flag if the jump button is released or when descending
            }
        }

        if (!isGrounded && rb.velocity.y < 0f)
        {
            isJumping = false; // Reset jumping flag when descending
        }

        // Apply custom gravity
        if (!isGrounded && !isJumping) // Apply custom gravity only if not grounded and not already jumping
        {
            rb.AddForce(Vector3.up * customGravity, ForceMode.Acceleration);
        }

        if (jumpCooldownTimer > 0f)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }
    }


    private bool CheckGrounded()
    {
        float raycastDistance = 1.1f;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                Debug.DrawRay(transform.position, Vector3.down * raycastDistance, Color.red);
                return true;
            }
        }

        return false;
    }

    // Custom Functions for Patcha
    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        Debug.Log("Take Damage");
    }

    void AddLife(int Life)
    {
        Lives += Life;
        // Life Adding Test
        Debug.Log("Your Current Lives Are");
        Debug.Log(Lives);
    }

    void LoseLife(int lifelost)
    {
        Lives -= lifelost;
        Debug.Log("You have");
        Debug.Log(Lives);
        Debug.Log("Remaining");
    }

    void PickupFunc(int PickupValue)
    {
        Pickups += PickupValue;
        pickupCounter += PickupValue;
        Debug.Log("You have picked up a coin");
    }
}
