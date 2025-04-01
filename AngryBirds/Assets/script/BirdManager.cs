using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdManager : MonoBehaviour
{
    [SerializeField] private GameObject normalBirdPrefab;
    [SerializeField] private int maxBirds;

    private int remainingBirds; 
    private Rigidbody2D bird; 
    private Collider2D birdCollider; 
    private BirdType currentBirdType; 
    private bool isLaunched; 
    public bool Hasjumped = false;

    [SerializeField] private float jumpForce;

    [SerializeField] private UIManager UIManager;

    // Bird type available
    private enum BirdType
    {
        Normal
    }

    void Update()
    {
        Hasjumped = false;

        if (bird == null || !isLaunched) return;

        // make the bird jump with space
        if (Input.GetKeyDown(KeyCode.Space) && Hasjumped == false)
        {
            switch (currentBirdType)
            {
                case BirdType.Normal:
                    Jump();
                    Hasjumped = true;
                    break;
            }

        }

        UpdateBirdRotation();
    }

    // Initialize birds and how many left
    public void InitializeBirds()
    {
        remainingBirds = maxBirds;
        CreateBird();
    }

    
    public void CreateBird()
    {
        if (remainingBirds <= 0) return; // verify how many birds left

        float randomValue = Random.value;
        GameObject birdPrefab;


            birdPrefab = normalBirdPrefab;
            currentBirdType = BirdType.Normal;
        

        // Instantiate selected bird
        bird = Instantiate(birdPrefab).GetComponent<Rigidbody2D>();
        birdCollider = bird.GetComponent<Collider2D>();
        birdCollider.enabled = false;
        bird.isKinematic = true; 
        remainingBirds--;


        isLaunched = false;

        // set bird as launched in his script
        Bird birdScript = bird.GetComponent<Bird>();
        birdScript?.SetLaunched(true);
    }


    public void BirdLanded()
    {
        currentBirdType = BirdType.Normal;

        // start count if no birds left
        if (remainingBirds <= 0)
        {
            StartCoroutine(ShowEndGameAfterDelay());
        }
    }


    private IEnumerator ShowEndGameAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        UIManager.ShowUIEndGame();
    }

    // return bird in use
    public Rigidbody2D GetCurrentBird()
    {
        return bird;
    }


    public void EnableCollider()
    {
        if (birdCollider)
        {
            birdCollider.enabled = true;
        }
    }

    // Updates the bird's position based on the input position, center, and offsets.
    public void UpdateBirdPosition(Vector3 position, Vector3 center, float offsetX, float offsetY)
    {
        // If the bird object is missing or has already been launched, do nothing.
        if (bird == null || isLaunched) return;

        // Calculate the direction from the center to the current position.
        Vector3 direction = position - center;

        // Set the bird's position with an additional offset.
        bird.transform.position = position + direction.normalized * offsetX + new Vector3(0, offsetY, 0);

        // Make the bird face the opposite direction of the movement.
        bird.transform.right = -direction.normalized;
    }

    // Launches the bird by applying a force in the direction opposite to the pull.
    public void Shoot(Vector3 currentPosition, Vector3 centerPosition, float force)
    { 
        // If the bird object is missing, do nothing.
        if (bird == null) return;
        bird.isKinematic = false;

        // Calculate the force to apply to the bird.
        Vector3 birdForce = (currentPosition - centerPosition) * force * -1;

        // Apply the force to the bird's velocity.
        bird.velocity = birdForce;


        isLaunched = true;
    }

    // Makes the bird jump by applying an upward velocity.
    private void Jump()
    {
        if (bird == null) return;

        // Apply vertical force to simulate a jump.
        bird.velocity = new Vector2(bird.velocity.x, jumpForce);
    }

    // Updates the bird's rotation based on its velocity direction.
    private void UpdateBirdRotation()
    {
        if (bird == null) return;

        // Calculate the angle of movement using arctangent.
        float angle = Mathf.Atan2(bird.velocity.y, bird.velocity.x) * Mathf.Rad2Deg;

        // Apply the rotation to the bird.
        bird.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
