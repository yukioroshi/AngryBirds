using UnityEngine;

public class Bird : MonoBehaviour
{
    private BirdManager birdManager;
    private Rigidbody2D rb; 
    private bool isLaunched; 

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Bird rigidbody
        birdManager = FindObjectOfType<BirdManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Bird launched ?
        if (isLaunched)
        {
            birdManager.BirdLanded(); // Bird landed
        }
    }

    public void SetLaunched(bool launched)
    {
        isLaunched = launched; // Bird launch state
    }
}
