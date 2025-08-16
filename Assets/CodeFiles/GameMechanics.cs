//I have added everything I took inspiration from in each script I am using in the project
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMechanics : MonoBehaviour
{
    public GameObject[] rockPrefabs;
    public GameObject[] shardPrefabs;
    public Transform player;
    public Transform cameraTransform;
    public GameObject portal;
    public Material shieldBlue;
    public Material shieldRed;
    public Material shieldGreen;
    public Material coreBlack;
    public Renderer coreRenderer;
    //public AudioSource Rainsound = null; //Added feature I wanted to add
    
    private GameObject[] shieldParts;

    private List<GameObject> shields = new List<GameObject>();
    private bool isInvincible = false;
    private bool gameEnded = false;

    void Start()
    {

        
        if (shieldParts != null && shieldParts.Length > 0)//AI help
        {
            
            shields.AddRange(shieldParts);
        }
        else
        {
            
            for (int i = 0; i <= 28; i++)
            {
                Transform shieldPart = transform.Find("globe_puzzle_pieces" + (i < 10 ? "0" + i : i.ToString()));
                if (shieldPart != null)
                {
                    shields.Add(shieldPart.gameObject);
                    Debug.Log("Found shield: " + shieldPart.name);
                }
                else
                {
                    Debug.LogWarning("Could not find shield part: globe_puzzle_pieces" + (i < 10 ? "0" + i : i.ToString()));
                }
            }
        }
        
        Debug.Log("Total shields found: " + shields.Count);
        
        if (shields.Count == 0)
        {
            Debug.LogError("No shields were found! Check shield naming or hierarchy.");
        }
        
        StartCoroutine(SpawnRocks());
        StartCoroutine(SpawnShards());//Any startCoroutine I have studied in a course as it was easier for me to count time this way rather the method from the lecture in some cases :)
    }

    IEnumerator SpawnRocks()
    {
        yield return new WaitForSeconds(5f);
        while (!gameEnded)
        {
            GameObject rock = Instantiate(
                rockPrefabs[Random.Range(0, rockPrefabs.Length)],
                player.position + cameraTransform.forward * Random.Range(30, 40) + new Vector3(Random.Range(-10, 10), 40, 0),
                Quaternion.identity
            );
            Rigidbody rb = rock.GetComponent<Rigidbody>();
            if (rb == null)
                rb = rock.AddComponent<Rigidbody>();
            rb.mass = 10; //AI help
            rb.angularVelocity = Random.onUnitSphere * 5f;
            Rock rockScript = rock.GetComponent<Rock>();
            if (rockScript == null)
                rockScript = rock.AddComponent<Rock>();
            rockScript.Init(this);//AI help

            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        }
    }


    public void TakeDamage()
    {
        Debug.Log("TakeDamage() called");

        if (isInvincible)
        {
            Debug.Log("Player is invincible!");
            return;
        }

        if (gameEnded)
        {
            Debug.Log("Game already ended.");
            return;
        }

        if (shields.Count == 0)
        {
            Debug.Log("No shields left!");
            return;
        }

        GameObject lastShield = shields[shields.Count - 1];
        shields.RemoveAt(shields.Count - 1);
        lastShield.SetActive(false); 
        
        Debug.Log("Shield destroyed. Remaining: " + shields.Count);

        StartCoroutine(InvincibilityTimer());

        if (shields.Count == 0)
        {
            Debug.Log("Triggering LoseGame()");
            LoseGame();
        }
    }

    IEnumerator InvincibilityTimer()
    {
        isInvincible = true;
        SetShieldColor(shieldRed);
        yield return new WaitForSeconds(5f);
        if (!gameEnded) 
        {
            SetShieldColor(shieldBlue);
            isInvincible = false;
        }
    }

IEnumerator SpawnShards()
{
    while (!gameEnded)
    {
        yield return new WaitForSeconds(5f);
        if (Random.value <= 0.25f)
        {
            int shardIndex = Random.Range(0, shardPrefabs.Length);
            Vector3 spawnPosition = player.position + 
                                    cameraTransform.forward * Random.Range(30, 40)/*I have set (30,40) here to push the spawn point 30–40 units directly forward in 3D space,so the required Z axis random spawn point */ + 
                                    new Vector3(Random.Range(-10, 10), 40, 0);
            
            // Do a raycast to check terrain height and adjust spawn accordingly
            RaycastHit hit;
            if (Physics.Raycast(spawnPosition, Vector3.down, out hit, 100f, LayerMask.GetMask("Terrain", "Default")))
            {
                // Spawn a bit above the terrain to avoid initial collisions
                spawnPosition = new Vector3(spawnPosition.x, hit.point.y + 20f, spawnPosition.z); //AI help
                Debug.Log("Adjusted shard spawn height based on terrain: " + spawnPosition.y);
            }
            
            GameObject shard = Instantiate(
                shardPrefabs[shardIndex],
                spawnPosition,
                Quaternion.identity
            );
            
         
            shard.transform.localScale = new Vector3(10, 10, 10);
            
            
            Renderer[] renderers = shard.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                
                Material shardMaterial = new Material(Shader.Find("Transparent/Diffuse"));
                shardMaterial.color = new Color(50f/255f, 50f/255f, 255f/255f, 0.3f); 
                renderer.material = shardMaterial;
            }
            
            
            GameObject lightObject = new GameObject("ShardLight");
            lightObject.transform.parent = shard.transform;
            lightObject.transform.localPosition = Vector3.zero;
            
            Light pointLight = lightObject.AddComponent<Light>();
            pointLight.type = LightType.Point;
            pointLight.color = new Color(36f/255f, 25f/255f, 129f/255f, 1f);
            pointLight.range = 3f;
            pointLight.intensity = 10f;
            
            
            if (shard.GetComponent<Shard>() == null)
                shard.AddComponent<Shard>();
            
            Debug.Log("Spawned shard at " + spawnPosition);
        }
    }
}
    
    
    public void RestoreShield(GameObject shard)
    {
        if (shields.Count >= 29) 
        {
            Debug.Log("Already at max shields, cannot restore more");
            Destroy(shard);
            return;
        }

        
        Transform shieldTransform = null; //AI help
        for (int i = 0; i <= 28; i++)
        {
            string shieldName = "globe_puzzle_pieces" + (i < 10 ? "0" + i : i.ToString());
            shieldTransform = transform.Find(shieldName);
            
            if (shieldTransform != null && !shieldTransform.gameObject.activeSelf && 
                !shields.Contains(shieldTransform.gameObject))
            {
                shieldTransform.gameObject.SetActive(true);
                
                
                Renderer[] renderers = shieldTransform.GetComponentsInChildren<Renderer>();
                Material currentMaterial = isInvincible ? shieldRed : shieldBlue;
                
                foreach (Renderer renderer in renderers)
                {
                    renderer.material = currentMaterial;
                }
                
                shields.Add(shieldTransform.gameObject);
                Debug.Log("Shield restored: " + shieldName + ". Total: " + shields.Count);
                Destroy(shard);
                return;
            }
        }
        
        Debug.LogWarning("Could not find a shield to restore");
        Destroy(shard);
    }

    void SetShieldColor(Material mat)
    {
        foreach (var s in shields)
        {
            var renderers = s.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.material = mat;
            }
        }
    }

    void Update()
    {
        if (!gameEnded && Vector3.Distance(player.position, portal.transform.position) < 1f && shields.Count > 0)
        {
            Debug.Log("won");
            WinGame();
        }
    }

    public void WinGame()
    {
        gameEnded = true;
        SetShieldColor(shieldGreen);
        DisablePlayerInput();
        StartCoroutine(AscendAndSpin());
    }

    void LoseGame()
    {
        gameEnded = true;
        coreRenderer.material = coreBlack;
        DisablePlayerInput();
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true; //AI help
    }

    void DisablePlayerInput()
    {
        var moveScript = GetComponent<Move>();
        if (moveScript != null)
            moveScript.enabled = false;
    }

    IEnumerator AscendAndSpin()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        while (true)
        {
            transform.position += Vector3.up * Time.deltaTime;
            transform.Rotate(Vector3.one * 90 * Time.deltaTime);//AI help
            yield return null;
        }
    }
}