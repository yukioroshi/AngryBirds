using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdManager : MonoBehaviour
{
    [Header("Birds")]
    // R�f�rences aux prefabs des diff�rents types d'oiseaux
    [SerializeField] private GameObject normalBirdPrefab;
    [SerializeField] private int maxBirds; // Nombre maximum d'oiseaux disponibles

    private int remainingBirds; // Nombre d'oiseaux restants
    private Rigidbody2D bird; // R�f�rence au Rigidbody de l'oiseau actuel
    private Collider2D birdCollider; // R�f�rence au Collider de l'oiseau actuel
    private BirdType currentBirdType; // Type de l'oiseau actuel
    private bool hasDashed; // Indique si l'oiseau rapide a d�j� dash�
    private bool isLaunched; // Indique si l'oiseau a �t� lanc�

    [Header("Fast Bird Settings")]
    [SerializeField] private float dashForce = 10f; // Force du dash pour l'oiseau rapide

    [Header("Jump Bird Settings")]
    [SerializeField] private float jumpForce; // Force du saut pour l'oiseau double saut

    [Header("Explosive Bird Settings")]
    [SerializeField] private float radiusExplosion; // Rayon d'explosion pour l'oiseau explosif
    [SerializeField] private float explosionForce; // Force de l'explosion

    [Header("UI Manager")]
    [SerializeField] private UIManager UIManager;

    // Enum�ration des types d'oiseaux disponibles
    private enum BirdType
    {
        Normal
    }

    void Update()
    {
        // Si aucun oiseau n'est actif ou qu'il n'a pas �t� lanc�, ne rien faire
        if (bird == null || !isLaunched) return;

        // D�tection de l'entr�e utilisateur pour d�clencher une capacit� sp�ciale
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (currentBirdType)
            {
                case BirdType.Normal:
                    Jump();
                    break;
            }
        }

        UpdateBirdRotation();
    }

    // Initialise le nombre d'oiseaux et en cr�e un premier
    public void InitializeBirds()
    {
        remainingBirds = maxBirds;
        CreateBird();
    }

    // Cr�e un nouvel oiseau al�atoire parmi les types disponibles
    public void CreateBird()
    {
        if (remainingBirds <= 0) return; // V�rifie si des oiseaux sont encore disponibles

        float randomValue = Random.value;
        GameObject birdPrefab;


            birdPrefab = normalBirdPrefab;
            currentBirdType = BirdType.Normal;
        

        // Instancie le prefab de l'oiseau s�lectionn�
        bird = Instantiate(birdPrefab).GetComponent<Rigidbody2D>();
        birdCollider = bird.GetComponent<Collider2D>();
        birdCollider.enabled = false;
        bird.isKinematic = true; // Emp�che l'oiseau de bouger avant le lancement
        remainingBirds--;

        hasDashed = false;
        isLaunched = false;

        // Marque l'oiseau comme lanc� dans son script d�di�
        Bird birdScript = bird.GetComponent<Bird>();
        birdScript?.SetLaunched(true);
    }

    // R�initialise le type d'oiseau lorsqu'il atterrit
    public void BirdLanded()
    {
        currentBirdType = BirdType.Normal;

        // Lance la coroutine pour afficher l'UI de fin de jeu si aucun oiseau n'est restant
        if (remainingBirds <= 0)
        {
            StartCoroutine(ShowEndGameAfterDelay());
        }
    }

    // Affiche l'UI de fin de jeu apr�s un d�lai de 5 secondes
    private IEnumerator ShowEndGameAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        UIManager.ShowUIEndGame();
    }

    // Retourne l'oiseau actuellement en jeu
    public Rigidbody2D GetCurrentBird()
    {
        return bird;
    }

    // Active le collider de l'oiseau
    public void EnableCollider()
    {
        if (birdCollider)
        {
            birdCollider.enabled = true;
        }
    }

    // Met � jour la position et la rotation de l'oiseau avant son lancement
    public void UpdateBirdPosition(Vector3 position, Vector3 center, float offsetX, float offsetY)
    {
        if (bird == null || isLaunched) return;

        // Calcule le vecteur directionnel entre la position actuelle de la souris (position) et le centre de la fronde (center)
        Vector3 direction = position - center;

        // Normalise la direction pour obtenir un vecteur unitaire (de longueur 1)
        // Ensuite, applique un d�calage horizontal (offsetX) et vertical (offsetY) pour ajuster la position de l'oiseau
        bird.transform.position = position + direction.normalized * offsetX + new Vector3(0, offsetY, 0);

        // Oriente l'oiseau dans la direction oppos�e au vecteur directionnel
        bird.transform.right = -direction.normalized;
    }

    // Lance l'oiseau avec une force calcul�e
    public void Shoot(Vector3 currentPosition, Vector3 centerPosition, float force)
    {
        if (bird == null) return;
        bird.isKinematic = false;

        // Calcule la force de tir : (diff�rence entre la position actuelle et le centre de la fronde) * force
        // Multiplie par -1 pour que l'oiseau parte dans la direction oppos�e au tir
        Vector3 birdForce = (currentPosition - centerPosition) * force * -1;

        // Applique la force calcul�e � la v�locit� de l'oiseau pour le propulser
        bird.velocity = birdForce;

        // Marque l'oiseau comme lanc�
        isLaunched = true;
    }

    // Applique un dash � l'oiseau rapide
    private void Dash()
    {
        if (bird == null) return;

        // R�cup�re la direction actuelle du mouvement de l'oiseau sous forme d'un vecteur normalis�
        Vector2 dashDirection = bird.velocity.normalized;

        // Applique une force suppl�mentaire dans cette direction pour acc�l�rer l'oiseau
        bird.velocity += dashDirection * dashForce;

        hasDashed = true;
    }

    // Applique un saut � l'oiseau double saut
    private void Jump()
    {
        if (bird == null) return;

        // Augmente la composante verticale (y) de la v�locit� de l'oiseau pour lui donner un effet de saut
        bird.velocity = new Vector2(bird.velocity.x, jumpForce);
    }

    // Met � jour la rotation de l'oiseau en fonction de sa vitesse
    private void UpdateBirdRotation()
    {
        if (bird == null) return;

        float angle = Mathf.Atan2(bird.velocity.y, bird.velocity.x) * Mathf.Rad2Deg;
        bird.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
