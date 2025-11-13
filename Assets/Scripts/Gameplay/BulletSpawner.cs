using System.Collections;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra al jugador aquí para que el spawner sepa su posición")]
    public Transform player; 

    [Header("Configuración de Patrones")]
    [SerializeField] private float spawnRate = 0.5f;   // Tiempo entre disparos en un mismo patrón
    [SerializeField] private int bulletsPerWave = 12;  // Balas en el patrón circular
    [SerializeField] private float waveCooldown = 2f;  // Tiempo de espera entre patrones

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

             StartCoroutine(SpawnPatterns());
    }

        private IEnumerator SpawnPatterns()
    {
        while (true) 
        {
            // Patrón 1: Círculo
            yield return StartCoroutine(SpawnCircle(bulletsPerWave));
            yield return new WaitForSeconds(waveCooldown);

            // Patrón 2: Disparos directos (solo si el jugador sigue vivo)
            if (player != null)
            {
                 yield return StartCoroutine(SpawnDirectShot(5)); // Dispara 5 balas
                 yield return new WaitForSeconds(waveCooldown);
            }
        }
    }

    private IEnumerator SpawnDirectShot(int count)
    {
        for (int i = 0; i < count; i++)
        {
            
            if (player == null) 
                yield break; 

            Vector3 spawnPos = GetRandomPositionOffscreen();
            
            Vector3 direction = (player.position - spawnPos).normalized;

            SpawnBullet(spawnPos, direction);
            yield return new WaitForSeconds(spawnRate);
        }
    }

    private IEnumerator SpawnCircle(int bulletCount)
    {
        Vector3 spawnPos = GetRandomPositionOffscreen();
        float angleStep = 360f / bulletCount;
        float currentAngle = 0f;

        for (int i = 0; i < bulletCount; i++)
        {
            float rad = currentAngle * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
            
            SpawnBullet(spawnPos, direction);
            
            currentAngle += angleStep;
        }
        yield return null; 
    }

    void SpawnBullet(Vector3 position, Vector3 direction)
    {
        // 1. Pedimos una bala al pool
        GameObject bullet = ObjectPooler.Instance.GetPooledObject();
        
        // 2. La posicionamos
        bullet.transform.position = position;
        
        // 3. Le decimos a su script (PooledProjectile) hacia dónde moverse
        PooledProjectile projScript = bullet.GetComponent<PooledProjectile>();
        if (projScript != null)
        {
            projScript.SetDirection(direction);
        }
    }

    Vector3 GetRandomPositionOffscreen()
    {
        int edge = Random.Range(0, 4);
        Vector3 pos = Vector3.zero;
        
        if(edge == 0) pos = new Vector3(-12, Random.Range(-6f, 6f), 0); // Izquierda
        if(edge == 1) pos = new Vector3(12, Random.Range(-6f, 6f), 0);  // Derecha
        if(edge == 2) pos = new Vector3(Random.Range(-12f, 12f), 6, 0);   // Arriba
        if(edge == 3) pos = new Vector3(Random.Range(-12f, 12f), -6, 0);  // Abajo

        return pos;
    }
}