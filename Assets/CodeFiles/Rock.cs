using UnityEngine;
// I have most of this part from this article https://discussions.unity.com/t/trying-to-spawn-random-falling-rocks-cant-figure-out-why-its-not-working-help-would-be-appreciated/173132
public class Rock : MonoBehaviour
{
    private GameMechanics gm;
    private Rigidbody rb;
    private bool hasHitPlayer = false;
    private bool hasLanded = false;
    private float landingThreshold = 0.1f; // Velocity threshold to consider rock as landed
    private float landingCheckDelay = 1.0f; // Time to wait before checking if rock has landed
    private float landingTime = 0f;
    
    public void Init(GameMechanics g)
    {
        Debug.Log("Rock Init() called");
        gm = g;
        rb = GetComponent<Rigidbody>();
        
        // Add collider if missing
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<SphereCollider>();
        }
    }
    
    void Update()
    {
        if (rb == null) 
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null) return;
        }
        
        // Check if rock has potentially landed
        if (!hasLanded && rb.linearVelocity.magnitude < landingThreshold && rb.angularVelocity.magnitude < landingThreshold)
        {
            // Start tracking when it first became slow
            if (landingTime == 0f)
            {
                landingTime = Time.time;
            }
            
            // If it's been slow for the delay time, consider it landed
            if (Time.time - landingTime > landingCheckDelay)
            {
                hasLanded = true;
                Debug.Log("Rock has landed and will no longer damage player");
            }
        }
        else if (!hasLanded)
        {
            // Reset landing timer if rock starts moving again
            landingTime = 0f;
        }
        
        
        if (hasLanded && rb.linearVelocity.magnitude < 0.01f && rb.angularVelocity.magnitude < 0.01f)
        {
            Destroy(gameObject);
        }
    }
    
    void OnCollisionEnter(Collision col)
    {
        if (hasHitPlayer || gm == null || hasLanded) return;
        
        
        if (rb.linearVelocity.magnitude < landingThreshold && rb.angularVelocity.magnitude < landingThreshold) return;//AI help
        
        if (col.gameObject.CompareTag("Player"))
        {
            Debug.Log("Rock hit the PLAYER!");
            gm.TakeDamage();
            hasHitPlayer = true;
        }
    }
}