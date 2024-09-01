using UnityEngine;

public class InteractiveCube : MonoBehaviour
{
    public float viewThreshold = 0.7f; // Threshold for when the cube is considered in view
    public float floatSpeed = 0.5f; // Speed at which the cube floats upwards
    public float slamSpeed = 10f;   // Speed at which the cube slams down
    private Vector3 startPosition;  // Starting position of the cube
    private Renderer cubeRenderer;
    private Color originalColor;
    private bool isFlashing = false;
    private float flashSpeed = 5f;  // Speed of the flashing
    private Color flashColor1 = Color.red;
    private Color flashColor2 = Color.white;

    void Start()
    {
        cubeRenderer = GetComponent<Renderer>();
        if (cubeRenderer != null)
        {
            originalColor = cubeRenderer.material.color;
        }
        else
        {
            Debug.LogError("No Renderer component found on this object.");
        }
        startPosition = transform.position; // Store the starting position
    }

    void Update()
    {
        if (!IsInView())
        {
            FloatUpwards();
        }
        else
        {
            SlamDown();
            if (!isFlashing) StartFlashing();
        }
    }

    bool IsInView()
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        bool inView = viewPos.x > 0.5f - viewThreshold / 2 && viewPos.x < 0.5f + viewThreshold / 2 &&
                      viewPos.y > 0.5f - viewThreshold / 2 && viewPos.y < 0.5f + viewThreshold / 2 &&
                      viewPos.z > 0;

        if (inView)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, transform.position - Camera.main.transform.position, out hit))
            {
                if (hit.transform != this.transform)
                {
                    inView = false;
                }
            }
        }

        return inView;
    }

    void FloatUpwards()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + (floatSpeed * Time.deltaTime), transform.position.z);
        if (isFlashing)
        {
            StopFlashing();
        }
    }

    void SlamDown()
    {
        float step = slamSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, startPosition, step);
    }

    void StartFlashing()
    {
        isFlashing = true;
        InvokeRepeating("Flash", 0f, 1f / flashSpeed);
    }

    void Flash()
    {
        cubeRenderer.material.color = (cubeRenderer.material.color == flashColor1) ? flashColor2 : flashColor1;
    }

    void StopFlashing()
    {
        isFlashing = false;
        CancelInvoke("Flash");
        cubeRenderer.material.color = originalColor;
    }
}