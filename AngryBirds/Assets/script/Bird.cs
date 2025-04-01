using UnityEngine;

public class Bird : MonoBehaviour
{
    private BirdManager birdManager;
    private Rigidbody2D rb; // Composant Rigidbody2D permettant la gestion de la physique de l'oiseau
    private bool isLaunched; // Indique si l'oiseau a �t� lanc� par le lance-pierre

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // R�cup�re le composant Rigidbody2D attach� � l'oiseau
        birdManager = FindObjectOfType<BirdManager>(); // Trouve automatiquement l'instance du BirdManager dans la sc�ne
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // V�rifie si l'oiseau a �t� lanc� avant de signaler l'atterrissage
        if (isLaunched)
        {
            birdManager.BirdLanded(); // Informe le BirdManager que l'oiseau a atterri apr�s un tir
        }
    }

    public void SetLaunched(bool launched)
    {
        isLaunched = launched; // D�finit l'�tat de lancement de l'oiseau
    }
}
