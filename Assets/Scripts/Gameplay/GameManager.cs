using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // ¡Añade esto para la UI!

public class GameManager : MonoBehaviour
{
    [Header("Referencias de Escena")]
    [SerializeField] private GameObject bulletSpawner;
    
    [Header("UI")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private Button restartButton;


    void Awake()
    {
        PlayerDodge.OnPlayerDied += HandlePlayerDeath;
        Time.timeScale = 1f;

        // 2. Desactiva la pantalla al inicio (por si acaso) y configura el botón
        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);
        
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame); // Conecta el clic a la función
    }

    void OnDestroy()
    {
        PlayerDodge.OnPlayerDied -= HandlePlayerDeath;

        // Es bueno desuscribir el listener del botón también
        if (restartButton != null)
            restartButton.onClick.RemoveListener(RestartGame);
    }

    private void HandlePlayerDeath()
    {
        if (bulletSpawner != null)
        {
            bulletSpawner.SetActive(false);
        }

        // 3. Activa la pantalla de Game Over
        if (gameOverScreen != null)
            gameOverScreen.SetActive(true);
        
        Debug.Log("GAME OVER - El GameManager ha recibido la señal de muerte.");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}