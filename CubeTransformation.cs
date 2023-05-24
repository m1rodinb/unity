using System.Collections;
using UnityEngine;

public class CubeTransformation : MonoBehaviour
{
    public Transform cubeToTransform; // THE CUBE TO TRANSFORM
    public Vector3 targetPosition; // POS TO MOVE CUBE TO
    public float totalDuration = 2f; // TOTAL DURATION OF TRANSFORMATION
    public Material initialMaterial; // INITAL MATERIAL
    public Material triggeredMaterial; // MATERIAL TO APPLY WHEN TRIGGERED

    public Color materialColorStart; // START COLOR
    public Color materialColorEnd; // END COLOR
    public float materialColorTransitionDuration = 0.5f; // DURATION OF COLOR CHANGE

    private Vector3 initialPosition; // START POS
    private bool isTransformed = false; // TRACK IF CUBE TRANSFORMED
    private float elapsedTime = 0f; // ELAPSED TIME
    private Renderer triggerCubeRenderer; // RENDER COMPONENT TO TRIGGER
    private bool hasTriggered = false; // TRACK COLOR CHANGER TRIGGERED

    private void Start()
    {
        initialPosition = cubeToTransform.position;

        // GET RENDERER COMPONENT 
        triggerCubeRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (!isTransformed)
            return;

        elapsedTime += Time.deltaTime;

        // CALCULATE INTERPOLATION
        float t = EasingFunction(elapsedTime / totalDuration);

        // INTERPOLATE BETWEEN POSITION AND TARGET
        Vector3 newPosition = Vector3.Lerp(initialPosition, targetPosition, t);

        // MOVE CUBE
        cubeToTransform.position = newPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;

            isTransformed = true;

            // CHANGE COLOR OF TRIGGER
            if (triggerCubeRenderer != null && triggeredMaterial != null)
            {
                // START COLOR CHANGE
                StartCoroutine(TransitionMaterialColor(triggerCubeRenderer));
            }
        }
    }

    // SMOOTH / EASE IN EASE OUT
    private float EasingFunction(float t)
    {
        t = Mathf.Clamp01(t);

        // EASE IN EASE OUT
        return Mathf.SmoothStep(0f, 1f, t);
    }

    // RESET CUBE
    public void ResetCube()
    {
        isTransformed = false;
        elapsedTime = 0f;

        // RESET CUBE POSITION
        cubeToTransform.position = initialPosition;

        // RESET MATERIAL OF TRIGGER CUBE
        if (triggerCubeRenderer != null && initialMaterial != null)
        {
            triggerCubeRenderer.material = initialMaterial;
        }
    }

    IEnumerator TransitionMaterialColor(Renderer cubeRenderer)
    {
        // GET MATERIAL OF OBJECT
        Material cubeMaterial = cubeRenderer.material;

        // GET INITIAL COLOR
        Color initialColor = cubeMaterial.color;

        // SMOOTHLY TRANSITION COLOR
        float elapsedTime = 0f;

        while (elapsedTime < materialColorTransitionDuration)
        {
            // CALCULATE INTERPOLATION BASED ON TIME AND DURATION
            float t = elapsedTime / materialColorTransitionDuration;

            // EASE IN EASE OUT
            t = Mathf.SmoothStep(0f, 1f, t);

            // INTERPOLATE THE COLOR BETWEEN START AND END COLORS
            Color targetColor = Color.Lerp(materialColorStart, materialColorEnd, t);

            // UPDATE COLOR
            cubeMaterial.color = targetColor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // MAKE SURE THE MATERIAL COLOR REACHES THE COLOR
        cubeMaterial.color = materialColorEnd;
    }
}
