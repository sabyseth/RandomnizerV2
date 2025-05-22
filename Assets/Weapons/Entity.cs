using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private float StartingHealth;
    [SerializeField] private BoxCollider collider;
    [SerializeField] private GameObject targetPrefab;
    
    private float health;
    public HealthBar healthBar;

    public float Health
    {
        get => health;
        set
        {
            health = value;
            Debug.Log("Entity's health: " + health);
            
            if (healthBar != null)
                healthBar.SetHealth(health);
            
            if (health <= 0f)
                Die();
        }
    }

    void Start()
    {
        Health = StartingHealth;
        if (healthBar != null)
            healthBar.SetMaxHealth(StartingHealth);
    }

    private void Die()
    {
        if (targetPrefab != null)
            Instantiate(targetPrefab, transform.position, transform.rotation);
        
        Destroy(gameObject); // This should NOT pause the game
    }
}