using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    // The maximum health of the object before it gets destroyed.
    [SerializeField] private int maxHealth = 100;

    // The current health of the object.
    private int currentHealth;

    // An array of sprites to visually represent different damage states.
    [SerializeField] private Sprite[] damageSprites;

    // Reference to the object's sprite renderer.
    private SpriteRenderer spriteRenderer;

    // A multiplier to scale the damage received based on collision force.
    [SerializeField] private int damageMultiplier = 0;

    // The number of points awarded per unit of health lost.
    [SerializeField] private int nbpoints = 100;

    // Initializes the object's health and sets the initial sprite.
    void Start()
    {
        currentHealth = maxHealth; // Set the object's health to full.
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component.
        UpdateSprite(); // Update the sprite based on current health.
    }

    // Handles collision events.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object was hit by a bird or another destructible object.
        if (collision.gameObject.CompareTag("Bird") || collision.gameObject.CompareTag("Destructible"))
        {
            // Calculate the force of the collision based on relative velocity.
            float collisionForce = collision.relativeVelocity.magnitude;

            // Determine damage based on the collision force and the damage multiplier.
            int damage = Mathf.RoundToInt(collisionForce * damageMultiplier);

            // Apply damage to the object.
            TakeDamage(damage);
        }
    }

    // Applies damage to the object and updates its state accordingly.
    private void TakeDamage(int damage)
    {
        int oldHealth = currentHealth; // Store the previous health for point calculations.

        // Reduce the object's health by the damage amount.
        currentHealth -= damage;

        // Ensure the health value stays within the valid range (0 to maxHealth).
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update the object's sprite to reflect the new damage state.
        UpdateSprite();

        // Calculate the lost health and determine how many points to award.
        int lostHealth = oldHealth - currentHealth;
        int pointsGagnes = lostHealth * nbpoints;

        // If points were gained, add them to the score.
        if (pointsGagnes > 0)
        {
            ScoreManager.instance.AddScore(pointsGagnes);
        }

        // If the object's health has reached 0, destroy it.
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Updates the object's sprite based on its current health.
    private void UpdateSprite()
    {
        // Calculate the appropriate sprite index based on the current damage level.
        int spriteIndex = Mathf.FloorToInt((1 - (float)currentHealth / maxHealth) * (damageSprites.Length - 1));

        // Update the sprite renderer to show the correct damage state.
        spriteRenderer.sprite = damageSprites[spriteIndex];
    }
}
