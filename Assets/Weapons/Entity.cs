using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private float StartingHealth;
    [SerializeField] private BoxCollider collider;
    [SerializeField] private GameObject targetPrefab;
    
    private float health;

    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
            Debug.Log("box's health: " + health);

            if (health <= 0f)
            {
                Destroy(gameObject);
                GameObject newTarget = Instantiate(targetPrefab);
                float randomX = Random.Range(collider.bounds.min.x, collider.bounds.max.x);
                float randomY = Random.Range(collider.bounds.min.y, collider.bounds.max.y);
                float randomZ = Random.Range(collider.bounds.min.z, collider.bounds.max.z);
                newTarget.transform.position = new Vector3(randomX, randomY, randomZ);
                //newTarget.
                health = StartingHealth;
                Debug.Log(health);
                
            }
        }
    }
    void Start()
    {
        Health = StartingHealth;
    }
   

}

