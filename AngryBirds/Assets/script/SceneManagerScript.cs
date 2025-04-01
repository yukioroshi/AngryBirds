using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Level");
    }
}
