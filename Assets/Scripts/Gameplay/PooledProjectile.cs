using UnityEngine;

public class PooledProjectile : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 5f; 

    private float lifeTimer;
    private Vector3 moveDirection = Vector3.right; 
    void OnEnable()
    {
        lifeTimer = lifetime;
    }

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

        private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ObjectPooler.Instance.ReturnToPool(gameObject);
        }
    }
}