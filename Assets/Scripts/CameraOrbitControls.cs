using System.Collections;
using UnityEngine;

public class CameraOrbitControls : MonoBehaviour
{
    public float rotationSpeed = 1f;
    public Transform target;
    public float transitionDuration = 0.5f;                                 // Duration of the transition in seconds

    private float mouseX;
    private float mouseY;

    private Coroutine transitionCoroutine;                                  // Coroutine reference for the transition
    private Vector3 initialPos = new Vector3(0, 1, -4);

    private void OnEnable()
    {
        GradeButton.OnButtonClickedEvent += ChangeTarget;
    }

    private void OnDisable()
    {
        GradeButton.OnButtonClickedEvent -= ChangeTarget;
    }

    private void Start()
    {
        ResetPosition();
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            ResetPosition();
        }
    }

    private void ChangeTarget(string grade, Transform stackTransform)
    {
        target = stackTransform;

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);                                    // Stop the current transition if it's in progress
        }

        transitionCoroutine = StartCoroutine(TransitionToTarget());
    }

    private IEnumerator TransitionToTarget()
    {
        Quaternion startRotation = transform.rotation;
        Vector3 startPosition = transform.position;

        Vector3 targetOffset = initialPos;
        Vector3 targetPosition = target.position + targetOffset;

        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;                                  // Calculate the interpolation factor (0 to 1)

            Quaternion rotation = Quaternion.Slerp(startRotation, target.rotation, t);   // Smoothly interpolate the rotation and position
            Vector3 position = Vector3.Lerp(startPosition, targetPosition, t);

            transform.rotation = rotation;
            transform.position = position;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = target.rotation;                                                // Ensure the camera reaches the final target position and rotation
        transform.position = targetPosition;
        mouseX = 0;
        mouseY = 0;
        transitionCoroutine = null;
    }


    private void ResetPosition()
    {
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;

        mouseY = Mathf.Clamp(mouseY, -90f, 90f);

        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);
        Vector3 position = rotation * initialPos + target.position;

        transform.rotation = rotation;
        transform.position = position;
    }
}
