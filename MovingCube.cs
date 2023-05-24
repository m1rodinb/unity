using UnityEngine;

public class MovingCube : MonoBehaviour
{
    public float fallSpeed = 2f; // FALLING SPEED

    private bool isFalling = false; // CHECK IF CUBE IS FALLING

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isFalling)
        {
            // START FALLING
            isFalling = true;

            // DISABLE COLISSION
            GetComponent<BoxCollider>().enabled = false;

            // APPLY DOWNWARD VELOCITY
            GetComponent<Rigidbody>().velocity = Vector3.down * fallSpeed;

            // Destroy the cube after a delay
            Destroy(gameObject, 30f); // ADJUST FALL TIME
        }
    }
}
