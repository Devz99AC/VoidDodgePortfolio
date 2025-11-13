using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDodge : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float moveSpeed = 8f;

    private PlayerControls controls;
    private Vector2 moveInput;

     public static event System.Action OnPlayerDied;

    private void Awake()
    {
        controls = new PlayerControls();
    }

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

        private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

     private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void Update()
    {
        Vector3 moveVector = new Vector3(moveInput.x, moveInput.y, 0);
        
              transform.Translate(moveVector * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Comparamos el Tag para identificar la bala
        if (other.CompareTag("EnemyProjectile"))
        {
                        OnPlayerDied?.Invoke();

            // Desactivamos al jugador
            gameObject.SetActive(false);
        }
    }
}