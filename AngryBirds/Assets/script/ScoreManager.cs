using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    // Singleton instance to ensure only one ScoreManager exists.
    public static ScoreManager instance;

    // Stores the total score of the game.
    private int totalScore = 0;

    // Reference to the UI text element that displays the score.
    [SerializeField] private TextMeshProUGUI scoreText;

    // Called when the script instance is first loaded.
    private void Awake()
    {
        // Ensure only one instance of ScoreManager exists.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // If another instance already exists, destroy this duplicate.
            Destroy(gameObject);
        }
    }

    // Called at the start of the game to initialize the score display.
    private void Start()
    {
        UpdateScoreUI();
    }

    // Adds points to the total score and updates the UI.
    public void AddScore(int score)
    {
        totalScore += score; // Increase the total score.
        UpdateScoreUI(); // Refresh the UI display.
    }

    // Updates the score UI text with the latest score.
    private void UpdateScoreUI()
    {
        if (scoreText != null) // Ensure the text component is assigned.
        {
            scoreText.text = "Score : " + totalScore; // Display the updated score.
        }
    }
}
