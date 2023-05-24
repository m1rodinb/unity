using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coin : MonoBehaviour
{
    public float amplitude = 0.1f; // AMPLITUDE OF COIN
    public float frequency = 1f; // FREQUENCY OF COIN

    private Vector3 startPos; // START POS

    // CALLED BEFORE FIRST FRAME
    void Start()
    {
        startPos = transform.position;
    }

    // CALLED ONCE PER FRAME
    void Update()
    {
        // CALCULATE NEW POS BASED ON TIME
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;

        // UPDATE POS
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}