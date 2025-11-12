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
        // Si olvidamos asignar al jugador en el Inspector, lo buscamos por su Tag
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        // Iniciamos la Corutina principal que manejará los patrones
        StartCoroutine(SpawnPatterns());
    }

    // Corutina principal que llama a otros patrones en un bucle
    private IEnumerator SpawnPatterns()
    {
        // 'while (true)' en una Corutina está bien,
        // porque 'yield return' pausa la ejecución.
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

    // PATRÓN 1: Dispara 'count' balas directas al jugador
    private IEnumerator SpawnDirectShot(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // Si el jugador muere a mitad del patrón, detenemos esta Corutina
            if (player == null) 
                yield break; 

            Vector3 spawnPos = GetRandomPositionOffscreen();
            // Calculamos la dirección desde el punto de spawn hacia el jugador
            Vector3 direction = (player.position - spawnPos).normalized;

            SpawnBullet(spawnPos, direction);
            yield return new WaitForSeconds(spawnRate);
        }
    }

    // PATRÓN 2: Dispara un círculo de 'bulletCount' balas
    private IEnumerator SpawnCircle(int bulletCount)
    {
        Vector3 spawnPos = GetRandomPositionOffscreen();
        float angleStep = 360f / bulletCount;
        float currentAngle = 0f;

        for (int i = 0; i < bulletCount; i++)
        {
            // Usamos trigonometría para calcular la dirección de cada bala
            float rad = currentAngle * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0);
            
            SpawnBullet(spawnPos, direction);
            
            currentAngle += angleStep;
        }
        yield return null; // Espera un frame antes de continuar
    }

    // Función AYUDANTE para pedir y configurar una bala del pool
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

    // Función AYUDANTE para obtener un punto de spawn fuera de la pantalla
    Vector3 GetRandomPositionOffscreen()
    {
        // Valores fijos (hardcoded) para los bordes de la pantalla (ajusta si es necesario)
        // Asumimos una cámara ortográfica en (0,0) con vista de ~12x6 unidades
        int edge = Random.Range(0, 4);
        Vector3 pos = Vector3.zero;
        
        if(edge == 0) pos = new Vector3(-12, Random.Range(-6f, 6f), 0); // Izquierda
        if(edge == 1) pos = new Vector3(12, Random.Range(-6f, 6f), 0);  // Derecha
        if(edge == 2) pos = new Vector3(Random.Range(-12f, 12f), 6, 0);   // Arriba
        if(edge == 3) pos = new Vector3(Random.Range(-12f, 12f), -6, 0);  // Abajo

        return pos;
    }
}