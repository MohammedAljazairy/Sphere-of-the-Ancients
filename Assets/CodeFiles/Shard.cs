using UnityEngine;

public class Shard : MonoBehaviour
{
    private Rigidbody rb;
    private bool isCollected = false;
    private bool hasLanded = false;
    private float landingCheckTime = 0f;
    
    void Start()
    {
        // Ensure there's a proper collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            // Add a slightly larger sphere collider for better collision detection
            SphereCollider sphereCol = gameObject.AddComponent<SphereCollider>();
            sphereCol.radius = 1.0f;  // Adjust radius as needed for your model
            sphereCol.isTrigger = false;  // IMPORTANT: This is NOT a trigger
        }
        else if (col is MeshCollider)
        {
            // Mesh colliders can sometimes cause issues, convert to sphere
            Destroy(col);
            SphereCollider sphereCol = gameObject.AddComponent<SphereCollider>();
            sphereCol.radius = 1.0f;
            sphereCol.isTrigger = false;
        }
        else
        {
            col.isTrigger = false;  // Make sure it's not a trigger for physics
        }
        
        // Add a separate trigger collider for player interaction
        GameObject triggerObj = new GameObject("TriggerCollider");
        triggerObj.transform.parent = transform;
        triggerObj.transform.localPosition = Vector3.zero;
        SphereCollider triggerCol = triggerObj.AddComponent<SphereCollider>();
        triggerCol.radius = 1.5f;  // Slightly larger than physical collider
        triggerCol.isTrigger = true;
        
        // Add script to handle trigger
        ShardTrigger trigger = triggerObj.AddComponent<ShardTrigger>();
        trigger.parentShard = this;
        
        // Set up rigidbody with proper settings for falling
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        //AI help
        // Configure rigidbody for better physics
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.mass = 5f;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.5f;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;  // Important for fast-moving objects
        
        // Add initial downward velocity to ensure it falls
        rb.linearVelocity = new Vector3(0, -2, 0);
        
        // Add slight rotation
        rb.angularVelocity = Random.onUnitSphere * 1f;
    }
    
    void Update()
    {
        // Check if the shard has landed
        if (!hasLanded && rb != null)
        {
            if (rb.linearVelocity.magnitude < 0.1f)
            {
                if (landingCheckTime == 0)
                {
                    landingCheckTime = Time.time;
                }
                else if (Time.time - landingCheckTime > 1.0f)
                {
                    // If velocity has been low for 1 second, consider it landed
                    hasLanded = true;
                    rb.constraints = RigidbodyConstraints.FreezePositionY;  // Prevent sinking
                    Debug.Log("Shard has landed and is stabilized");
                }
            }
            else
            {
                landingCheckTime = 0;
            }
        }
    }
    
    public void OnPlayerCollect(Collider playerCollider)
    {
        if (isCollected) return;
        
        GameMechanics playerMechanics = playerCollider.GetComponent<GameMechanics>();
        if (playerMechanics != null)
        {
            isCollected = true;
            playerMechanics.RestoreShield(gameObject);
        }
        else
        {
            Debug.LogError("Player is missing GameMechanics component!");
        }
    }
}


public class ShardTrigger : MonoBehaviour
{
    public Shard parentShard;
    
    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player") && parentShard != null)
        {
            parentShard.OnPlayerCollect(col);
        }
    }
}