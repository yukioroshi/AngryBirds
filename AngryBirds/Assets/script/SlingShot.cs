using UnityEngine;

public class SlingShot : MonoBehaviour
{
    [Header("Slingshot")]
    [SerializeField] private LineRenderer[] lineRenderers; // Contient les LineRenderers repr�sentant les �lastiques de la fronde
    [SerializeField] private Transform[] stripPositions; // Points d'attache des �lastiques (gauche et droite)
    [SerializeField] private Transform centerPosition; // Position centrale de la fronde (d'o� part l'oiseau)
    [SerializeField] private Transform idlePosition; // Position par d�faut de l'�lastique quand il est rel�ch�
    [SerializeField] private Collider2D slingshotCollider; // Collider de la fronde pour �viter les interactions apr�s un tir
    [SerializeField] private float bottomBoundary; // Limite basse pour �viter que l'oiseau ne soit trop bas avant d'�tre tir�
    [SerializeField] private float maxLenght; // Longueur maximale � laquelle on peut tirer l'�lastique
    [SerializeField] private float force; // Force de propulsion de l'oiseau au l�cher de l'�lastique

    [Header("Birds")]
    [SerializeField] private BirdManager birdManager;
    [SerializeField] private float birdPositionOffsetX; // D�calage horizontal appliqu� � la position de l'oiseau dans l'�lastique
    [SerializeField] private float birdPositionOffsetY; // D�calage vertical appliqu� � la position de l'oiseau dans l'�lastique

    private Vector3 currentPosition; // Position actuelle de la souris convertie en coordonn�es du monde
    private bool isMouseDown; // Indique si la souris est en train d'�tre maintenue enfonc�e

    private void Start()
    {
        // Initialise les LineRenderers pour afficher les �lastiques de la fronde
        lineRenderers[0].positionCount = 2; // Chaque �lastique est une ligne avec 2 points
        lineRenderers[1].positionCount = 2;

        // Place les extr�mit�s des �lastiques aux points d'attache initiaux
        lineRenderers[0].SetPosition(0, stripPositions[0].position);
        lineRenderers[1].SetPosition(0, stripPositions[1].position);

        // Initialise les oiseaux (g�n�ralement en instanciant le premier oiseau)
        birdManager.InitializeBirds();
    }

    private void Update()
    {
        if (isMouseDown) // Si la souris est maintenue enfonc�e
        {
            // R�cup�re la position de la souris en pixels et la convertit en coordonn�es monde
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10; // D�finir une profondeur pour la conversion en 3D
            currentPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // Calcule la position limit�e de l'�lastique en fonction de la longueur max
            currentPosition = centerPosition.position + Vector3.ClampMagnitude(currentPosition - centerPosition.position, maxLenght);

            // Emp�che l'�lastique d'aller trop bas (�vite que l'oiseau ne passe sous une certaine limite)
            currentPosition.y = Mathf.Clamp(currentPosition.y, bottomBoundary, currentPosition.y);

            // Met � jour la position des �lastiques
            SetStrips(currentPosition);

            // Ajoutez les offsets ici
            Vector3 offsetPosition = currentPosition + new Vector3(birdPositionOffsetX, birdPositionOffsetY, 0);
            float appliedForce = (centerPosition.position - offsetPosition).magnitude * force;
            // Affiche la trajectoire pr�vue de l'oiseau
            TrajectoryManager.Instance.DisplayTrajectory(birdManager.GetCurrentBird(), offsetPosition, centerPosition.position, appliedForce);

            // Active le collider de l'oiseau (s'il �tait d�sactiv� auparavant)
            birdManager.EnableCollider();
        }
        else
        {
            ResetStrips();
            TrajectoryManager.Instance.HideTrajectory();
        }
    }

    private void OnMouseDown()
    {
        isMouseDown = true; // D�tecte que la souris a �t� press�e
    }

    private void OnMouseUp()
    {
        isMouseDown = false; // D�tecte que la souris a �t� rel�ch�e
        Shoot(); // Tire l'oiseau
    }

    // R�initialise la position des �lastiques � l'�tat initial
    private void ResetStrips()
    {
        currentPosition = idlePosition.position; // Remet la position de l'�lastique � l'idle
        SetStrips(currentPosition);
    }

    // Met � jour la position des �lastiques et la position de l'oiseau
    private void SetStrips(Vector3 position)
    {
        lineRenderers[0].SetPosition(1, position); // D�place le deuxi�me point de l'�lastique gauche
        lineRenderers[1].SetPosition(1, position); // D�place le deuxi�me point de l'�lastique droit

        // Met � jour la position de l'oiseau avec un d�calage d�fini
        birdManager.UpdateBirdPosition(position, centerPosition.position, birdPositionOffsetX, birdPositionOffsetY);
    }

    // Tire l'oiseau en appliquant la force calcul�e
    private void Shoot()
    {
        birdManager.Shoot(currentPosition, centerPosition.position, force); // Applique la force de tir � l'oiseau

        slingshotCollider.enabled = false; // D�sactive le collider pour �viter les interactions inutiles apr�s le tir

        Invoke("NextBird", 2); // Pr�pare le prochain oiseau apr�s un d�lai de 2 secondes
    }

    // Pr�pare le prochain oiseau apr�s le tir
    private void NextBird()
    {
        birdManager.CreateBird(); // G�n�re un nouvel oiseau
        slingshotCollider.enabled = true; // R�active le collider pour le prochain tir
    }
}
