using UnityEngine;

public class SlingShot : MonoBehaviour
{
    
    [SerializeField] private LineRenderer[] lineRenderers; 
    [SerializeField] private Transform[] stripPositions; 
    [SerializeField] private Transform centerPosition; 
    [SerializeField] private Transform idlePosition; 
    [SerializeField] private Collider2D slingshotCollider; 
    [SerializeField] private float bottomBoundary; 
    [SerializeField] private float maxLenght; 
    [SerializeField] private float force; 

    
    [SerializeField] private BirdManager birdManager;
    [SerializeField] private float birdPositionOffsetX; 
    [SerializeField] private float birdPositionOffsetY; 

    private Vector3 currentPosition; 
    private bool isMouseDown;

    // Called when the script starts.
    private void Start()
    {
        // Set the number of positions for both slingshot line renderers.
        lineRenderers[0].positionCount = 2;
        lineRenderers[1].positionCount = 2;

        // Set the initial position of the slingshot strings to their anchor points.
        lineRenderers[0].SetPosition(0, stripPositions[0].position);
        lineRenderers[1].SetPosition(0, stripPositions[1].position);

        // Initialize the birds at the start of the game.
        birdManager.InitializeBirds();
    }

    // Called once per frame to handle player input and updates.
    private void Update()
    {
        if (isMouseDown) // Check if the player is holding the mouse button.
        {
            // Get the mouse position in screen space and convert it to world space.
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10; // Depth value to ensure proper conversion.
            currentPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // Limit the bird's position within the allowed radius around the center.
            currentPosition = centerPosition.position + Vector3.ClampMagnitude(currentPosition - centerPosition.position, maxLenght);

            // Restrict the bird's vertical movement within the bottom boundary.
            currentPosition.y = Mathf.Clamp(currentPosition.y, bottomBoundary, currentPosition.y);

            // Update the slingshot bands' positions.
            SetStrips(currentPosition);

            // Calculate the applied force based on the pull distance.
            Vector3 offsetPosition = currentPosition + new Vector3(birdPositionOffsetX, birdPositionOffsetY, 0);
            float appliedForce = (centerPosition.position - offsetPosition).magnitude * force;

            // Display the predicted trajectory based on the applied force.
            TrajectoryManager.Instance.DisplayTrajectory(birdManager.GetCurrentBird(), offsetPosition, centerPosition.position, appliedForce);

            // Enable the bird's collider so it can interact with the environment after launch.
            birdManager.EnableCollider();
        }
        else // If the mouse button is released, reset the slingshot bands.
        {
            ResetStrips();
            TrajectoryManager.Instance.HideTrajectory();
        }
    }

    // Called when the player clicks on the slingshot.
    private void OnMouseDown()
    {
        isMouseDown = true; // Register that the mouse is being held down.
    }

    // Called when the player releases the mouse button.
    private void OnMouseUp()
    {
        isMouseDown = false; // Register that the mouse button is released.
        Shoot(); // Launch the bird.
    }

    // Resets the slingshot bands to their idle position.
    private void ResetStrips()
    {
        currentPosition = idlePosition.position; // Set the current position to idle.
        SetStrips(currentPosition); // Update the visual position of the slingshot bands.
    }

    // Updates the slingshot bands' positions based on the bird's position.
    private void SetStrips(Vector3 position)
    {
        lineRenderers[0].SetPosition(1, position); // Update the left band.
        lineRenderers[1].SetPosition(1, position); // Update the right band.

        // Update the bird's position relative to the slingshot.
        birdManager.UpdateBirdPosition(position, centerPosition.position, birdPositionOffsetX, birdPositionOffsetY);
    }

    // Launches the bird when the player releases the slingshot.
    private void Shoot()
    {
        // Apply the shooting force to the bird.
        birdManager.Shoot(currentPosition, centerPosition.position, force);

        // Disable the slingshot collider to prevent further interactions.
        slingshotCollider.enabled = false;

        // Schedule the creation of the next bird after 2 seconds.
        Invoke("NextBird", 2);
    }

    // Creates the next bird and reactivates the slingshot.
    private void NextBird()
    {
        birdManager.CreateBird(); // Spawn a new bird.
        slingshotCollider.enabled = true; // Re-enable the slingshot for the next shot.
    }
}
