using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDodge : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float moveSpeed = 8f;

    private PlayerControls controls;
    private Vector2 moveInput;

    // Evento estático para notificar a otros scripts (GameManager) sobre la muerte.
    public static event System.Action OnPlayerDied;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    // Usamos OnEnable y OnDisable para suscribir/desuscribir eventos.
    // Esto previene errores si el objeto se activa/desactiva en runtime.
    private void OnEnable()
    {
        controls.Player.Move.performed += OnMovePerformed;
        controls.Player.Move.canceled += OnMoveCanceled;
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Move.performed -= OnMovePerformed;
        controls.Player.Move.canceled -= OnMoveCanceled;
        controls.Player.Disable();
    }

    // Esta función es llamada por el Input System (via 'performed')
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // Esta función es llamada por el Input System (via 'canceled')
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void Update()
    {
        Vector3 moveVector = new Vector3(moveInput.x, moveInput.y, 0);
        
        // Usamos Time.deltaTime para que el movimiento sea independiente de los FPS
        transform.Translate(moveVector * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Comparamos el Tag para identificar la bala
        if (other.CompareTag("EnemyProjectile"))
        {
            // Lanzamos el evento para que el GameManager reaccione
            OnPlayerDied?.Invoke();

            // Desactivamos al jugador
            gameObject.SetActive(false);
        }
    }
}