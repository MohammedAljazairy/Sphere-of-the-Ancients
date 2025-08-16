using UnityEngine;

public class WinAtPortal : MonoBehaviour
{
    private GameMechanics gameMechanics;
    
    void Start()
    {
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            gameMechanics = player.GetComponent<GameMechanics>();
            if (gameMechanics == null)
            {
                Debug.LogError("Player object does not have GameMechanics component!");
            }
        }
        else
        {
            Debug.LogError("Cannot find player object! Make sure Player has the 'Player' tag.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered portal trigger!");
            if (gameMechanics != null)
            {
                gameMechanics.WinGame();
            }
            else
            {
                Debug.LogError("GameMechanics reference is null!");
            }
        }
    }
}