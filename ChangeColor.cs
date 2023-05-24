using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public Color targetColor; // TARGET COLOR
    public float colorTransitionSpeed = 2f; // SPEED OF TRANSITION

    private Renderer cubeRenderer; // CUBES RENDERER COMPONENT
    private Color initialColor; // INITIAL COLOR
    private bool isTransitioning; // TRACK IF COLOR TRANSITION

    private void Start()
    {
        // GET RENDER COMPONENT
        cubeRenderer = GetComponent<Renderer>();
        // START COLOR
        initialColor = cubeRenderer.material.color;
    }

    private void OnTriggerStay(Collider other)
    {
        // CHECK IF ON CHECKPOINT
        if (other.gameObject.CompareTag("Player") && !isTransitioning)
        {
            // START TRANSITION
            StartCoroutine(TransitionColor());
        }
    }

    private System.Collections.IEnumerator TransitionColor()
    {
        isTransitioning = true;

        float elapsedTime = 0f;

        // LOOP UNTIL COMPLETE
        while (elapsedTime < colorTransitionSpeed)
        {
            // CALCULATE COLOR BASED ON TIME
            Color currentColor = Color.Lerp(initialColor, targetColor, elapsedTime / colorTransitionSpeed);

            // UPDATE COLOR
            cubeRenderer.material.color = currentColor;

            // INCREMENT ON ELAPSED TIME
            elapsedTime += Time.deltaTime;

            yield return null; // WAIT FOR NEXT FRAME
        }

        // SET FINAL COLOR TO TARGET COLOR
        cubeRenderer.material.color = targetColor;

        isTransitioning = false;
    }
}
