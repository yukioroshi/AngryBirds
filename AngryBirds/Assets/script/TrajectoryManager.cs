using System.Collections.Generic;
using UnityEngine;

public class TrajectoryManager : MonoBehaviour
{
    // Instance unique du TrajectoryManager (singleton)
    public static TrajectoryManager Instance { get; private set; }


    [SerializeField] private GameObject trajectoryPointPrefab; 
    [SerializeField] private int trajectoryPointCount = 30; 
    [SerializeField] private float trajectoryTimeStep = 0.1f;

    private List<GameObject> trajectoryPoints = new List<GameObject>();

    // Called when the script instance is being loaded.
    private void Awake()
    {
        // Ensure only one instance of this class exists.
        if (Instance == null)
        {
            Instance = this;
        }

        // Initialize the trajectory points for trajectory visualization.
        InitializeTrajectoryPoints();
    }

    // Creates and initializes trajectory points for visualizing the bird's path.
    private void InitializeTrajectoryPoints()
    {
        for (int i = 0; i < trajectoryPointCount; i++)
        {
            // Instantiate a trajectory point prefab.
            GameObject point = Instantiate(trajectoryPointPrefab);

            // Initially deactivate the point.
            point.SetActive(false);

            // Add the point to the trajectoryPoints list.
            trajectoryPoints.Add(point);
        }
    }

    // Displays the predicted trajectory of the bird before launching.
    public void DisplayTrajectory(Rigidbody2D bird, Vector3 currentPosition, Vector3 centerPosition, float force)
    {
        // If the bird object is missing, do nothing.
        if (bird == null) return;

        // Calculate the initial velocity based on the pull direction and force.
        Vector3 initialVelocity = (centerPosition - currentPosition).normalized * force;

        // Start from the bird's current position.
        Vector3 currentPos = bird.transform.position;

        // Set the initial velocity.
        Vector3 currentVelocity = initialVelocity;

        // Iterate through each trajectory point to update its position.
        for (int i = 0; i < trajectoryPointCount; i++)
        {
            // Position the trajectory point at the calculated position.
            trajectoryPoints[i].transform.position = currentPos;

            // Activate the point to make it visible.
            trajectoryPoints[i].SetActive(true);

            // Update the position for the next point using velocity.
            currentPos += currentVelocity * trajectoryTimeStep;

            // Apply gravity to simulate realistic motion.
            currentVelocity += (Vector3)Physics2D.gravity * trajectoryTimeStep;
        }
    }

    // Hides the trajectory points when they are no longer needed.
    public void HideTrajectory()
    {
        // Deactivate all trajectory points.
        foreach (var point in trajectoryPoints)
        {
            point.SetActive(false);
        }
    }
}
