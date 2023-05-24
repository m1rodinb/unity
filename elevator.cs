using System.Collections;
using UnityEngine;

public class elevator : MonoBehaviour
{
    public bool canMove;
    public float maxHeight = 10f; // HEIGHT LIMIT / SAFTEY
    public float delayTime = 1f; // DELAY AFTER JUMPING OFF

    [SerializeField] float speed;
    [SerializeField] int startPoint;
    [SerializeField] Transform[] points; // ARRAY OF POSITIONS

    private int currentIndex;
    private bool reverse;
    private bool playerOnElevator;

    private Vector3 bottomPosition; // BOTTOM POS

    private void Start()
    {
        currentIndex = startPoint;
        transform.position = points[currentIndex].position;

        // BOTTOM POS
        bottomPosition = points[0].position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnElevator = true;
            StartCoroutine(StartMovingAfterDelay());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnElevator = false;
        }
    }

    private IEnumerator StartMovingAfterDelay()
    {
        yield return new WaitForSeconds(delayTime);

        // CHECK IF PLAYER ON ELEVATOR
        if (playerOnElevator)
        {
            canMove = true;
        }
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, points[currentIndex].position) < 0.01f)
        {
            if (currentIndex == points.Length - 1)
            {
                // CHECK IF PLAYER ON ELEVATOR
                if (!playerOnElevator)
                {
                    reverse = true;
                    currentIndex--;
                    canMove = false; // STOP MOVEMENT
                }
            }
            else if (currentIndex == 0)
            {
                reverse = false;
                currentIndex++;
            }
            else
            {
                if (reverse)
                    currentIndex--;
                else
                    currentIndex++;
            }
        }

        if (canMove)
        {
            // MOVE TO CURRENT POINT
            transform.position = Vector3.MoveTowards(transform.position, points[currentIndex].position, speed * Time.deltaTime);

            // MAX HEIGHT / SAFTEY
            float clampedY = Mathf.Clamp(transform.position.y, bottomPosition.y, bottomPosition.y + maxHeight);
            transform.position = new Vector3(transform.position.x, clampedY, transform.position.z);
        }
        else
        {
            // MOVE ELEVATOR TO BOTTOM IF NOT MOVING
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, bottomPosition, step);
        }
    }
}
