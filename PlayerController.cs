using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float gravityScale = 1;
    public float jumpForce = 5f;
    public float speed = 0;
    public TextMeshProUGUI countText;

    // ARRAY SPAWNPOINTS
    public Transform[] spawnPoints;

    public Color checkpointColorStart;
    public Color checkpointColorEnd;
    public float checkpointColorTransitionDuration = 0.5f;

    private Rigidbody rb;
    private int count;
    private float movementX;
    private float movementY;
    private bool isJumping = false;
    private bool playerIsOnGround = true;
    private Transform currentSpawnPoint; // CURRENT SPAWNPOINT
    private bool disableVelocity = false; // FLAG TO DISABLE VELOCITY
    private float velocityDisableDuration = 0.2f; // DURATION TO DISABLE VELOCITY 
    private float velocityDisableTimer = 0f; // TIMER TO CALCULATE ELAPSED TIME

    // CALLED BEFORE FIRST FRAM
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;

        // MAKE SURE ITS CORRECT BEFORE SETTING COUNT
        if (countText != null)
        {
            SetCountText();
        }
        else
        {
            Debug.LogError("countText is not assigned in the inspector!");
        }

        // SET THE FIRST SPAWN AND ARRAY
        if (spawnPoints.Length > 0)
        {
            currentSpawnPoint = spawnPoints[0];
        }
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    private void OnJump()
    {
        if (playerIsOnGround)
        {
            isJumping = true;
        }
    }

    void SetCountText()
    {
        if (count >= 8)
        {
            countText.text = "Win!";
        }
        else
        {
            countText.text = "Count: " + count.ToString();
        }
    }

        void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        rb.AddForce(movement * speed);
        rb.AddForce(Physics.gravity * (gravityScale - 1) * rb.mass);

        if (isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            isJumping = false;
            playerIsOnGround = false;
        }

        // CHECK IF THE PLAYER FALLS 
        if (transform.position.y < -7f)
        {
            Respawn();
        }

        // CHECK IF VELOCITY SHOULD BE DISABLED
        if (disableVelocity)
        {
            // DISABLE X AND Z VELOCITY
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);

            // INCREMENT TIMER
            velocityDisableTimer += Time.deltaTime;

            // CHECK IF DURATION HAS PASSED
            if (velocityDisableTimer >= velocityDisableDuration)
            {
                // ENABLE VELOCITY
                velocityDisableTimer = 0f;
                disableVelocity = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            playerIsOnGround = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count = count + 1;
            SetCountText();
        }

        // CHECK IF COLLIDING WITH CHECKPOINT
        if (other.gameObject.CompareTag("Checkpoint"))
        {
            // SET SPAWN
            currentSpawnPoint = other.gameObject.transform;
        }
    }

    void Respawn()
    {
        // RESET VELOCITY
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // DISABLE INPUT
        enabled = false;

        // START COUROUTINE TO SPAWN
        StartCoroutine(MovePlayerToSpawnPoint(currentSpawnPoint.position));
    }

    IEnumerator MovePlayerToSpawnPoint(Vector3 targetPosition)
    {
        // STORE VELOCITY
        Vector3 playerVelocity = rb.velocity;

        // RESET VELOCITY
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // DISABLE MOVEMENT
        enabled = false;

        // CALCULATE POS OF PLAYER
        Vector3 initialPosition = transform.position;

        // CALCULATE SPAWN POINT
        Vector3 spawnPosition = targetPosition +Vector3.up * 0.5f; // ADJUST HEIGHT OF SPAWN

        // MOVE PLAYER SMOOTHLY
        float duration = 0.5f; // ADJUST DURATION
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // CALCULAT THE INTERPOLATION FROM TIME AND DURATION
            float t = elapsedTime / duration;

            // APPLY EASE IN EASE OUT TO RESPAWN
            t = Mathf.SmoothStep(0f, 1f, t);

            // MOVE PLAYER SMOOTH TO CHECKPOINT
            transform.position = Vector3.Lerp(initialPosition, spawnPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // REACHES TARGET POS
        transform.position = spawnPosition;

        // RESTORE VELOCITY
        rb.velocity = playerVelocity;

        // ENABLE INPUT AFTER RESPAWN
        enabled = true;

        // DISABLE VELOCITY FOR A SHORT TIME // WAS A GLITCH
        disableVelocity = true;
        velocityDisableTimer = 0f;
    }
}
