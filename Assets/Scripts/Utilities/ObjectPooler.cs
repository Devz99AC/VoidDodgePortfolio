using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    [Header("Configuración del Pool")]
    public GameObject prefabToPool;
    public int amountToPool = 200;

    private Queue<GameObject> pooledObjects;

    // Awake() se ejecuta ANTES que todos los métodos Start()
    void Awake()
    {
        // 1. Configuración del Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 2. Inicialización del Pool (movido desde Start)
        pooledObjects = new Queue<GameObject>();

        for (int i = 0; i < amountToPool; i++)
        {
            GameObject obj = Instantiate(prefabToPool);
            obj.transform.SetParent(this.transform); 
            obj.SetActive(false);
            pooledObjects.Enqueue(obj);
        }
    }

    // El método Start() ahora puede estar vacío o ser eliminado
    void Start()
    {
        // No dejes nada aquí
    }

    public GameObject GetPooledObject()
    {
        // Esta línea (ahora en Awake) previene el error 'if (pooledObjects.Count == 0)'
        if (pooledObjects.Count == 0)
        {
            GameObject obj = Instantiate(prefabToPool);
            obj.transform.SetParent(this.transform);
            obj.SetActive(false);
            pooledObjects.Enqueue(obj);
        }

        GameObject objectToSpawn = pooledObjects.Dequeue();
        objectToSpawn.SetActive(true);
        return objectToSpawn;
    }

    public void ReturnToPool(GameObject objectToReturn)
    {
        objectToReturn.SetActive(false);
        pooledObjects.Enqueue(objectToReturn);
    }
}