using UnityEngine;

public class PooledProjectile : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 5f; // Tiempo antes de volver al pool

    private float lifeTimer;
    private Vector3 moveDirection = Vector3.right; // Dirección por defecto

    // OnEnable se llama CADA VEZ que el pool activa el objeto.
    // Es el "Start" para un objeto del pool.
    void OnEnable()
    {
        lifeTimer = lifetime;
    }

    // El Spawner usará esto para decirle a la bala hacia dónde moverse
    public void SetDirection(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
        
        lifeTimer -= Time.deltaTime;
        
        // Cuando se acaba el tiempo, vuelve al pool
        if (lifeTimer <= 0)
        {
            ObjectPooler.Instance.ReturnToPool(gameObject);
        }
    }

    // También vuelve al pool si choca con el jugador
    // (El script del jugador ya maneja su propia desactivación)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ObjectPooler.Instance.ReturnToPool(gameObject);
        }
    }
}