using UnityEngine;
using UnityEngine.UI;

public class BossArrowUI : MonoBehaviour
{
    [SerializeField] private Sprite[] spriteDirections = new Sprite[4]; // Assign in Inspector: Right, Up, Left, Down
    [SerializeField] private Transform destination; // The target in world space
    [SerializeField] private Image arrowImage; // Reference to the UI Image component

    private Camera mainCamera;
    private RectTransform arrowRectTransform;
    private RectTransform canvasRectTransform;

    private void Start()
    {
        mainCamera = Camera.main;
        arrowRectTransform = GetComponent<RectTransform>();
        canvasRectTransform = arrowRectTransform.parent.GetComponent<RectTransform>();

        if (arrowImage == null)
        {
            arrowImage = GetComponent<Image>();
        }

        Debug.Log("BossArrowUI initialized. Destination: " + destination);
    }

    private void Update()
    {
        UpdateArrowDirection();
    }

    private void UpdateArrowDirection()
    {
        if (destination == null || arrowImage == null || spriteDirections.Length < 4 || mainCamera == null)
        {
            Debug.LogWarning("Missing references in BossArrowUI.");
            return;
        }
        Debug.Log("Destination World Position: " + destination.position);
        Vector3 destinationScreenPosition = mainCamera.WorldToScreenPoint(destination.position);
        Debug.Log("Destination Screen Position: " + destinationScreenPosition);
        Vector2 localPoint;
        bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            destinationScreenPosition,
            mainCamera,
            out localPoint
        );

        if (!success)
        {
            Debug.LogWarning("Failed to convert screen point to local point.");
            return;
        }

        // Calculate the direction from the arrow to the destination
        Vector2 direction = localPoint - (Vector2)arrowRectTransform.localPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Normalize the angle to 0-360 degrees
        angle = (angle + 360) % 360;
        Debug.Log("Calculated Angle: " + angle);

        // Update the sprite based on the angle
        if (angle >= 45 && angle < 135)
        {
            arrowImage.sprite = spriteDirections[1]; // Up
            Debug.Log("Direction: Up");
        }
        else if (angle >= 135 && angle < 225)
        {
            arrowImage.sprite = spriteDirections[2]; // Left
            Debug.Log("Direction: Left");
        }
        else if (angle >= 225 && angle < 315)
        {
            arrowImage.sprite = spriteDirections[3]; // Down
            Debug.Log("Direction: Down");
        }
        else
        {
            arrowImage.sprite = spriteDirections[0]; // Right
            Debug.Log("Direction: Right");
        }
    }
}
