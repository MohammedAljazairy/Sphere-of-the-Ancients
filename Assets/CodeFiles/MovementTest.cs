using UnityEngine;

public class MovementTest : MonoBehaviour
{ //This part only to make the empty gameobject follow the player and not rotate with it, also I have searched on how I can't implement the rain without it being way to unrealistic https://www.youtube.com/watch?v=CXdOPMWKHko
        public Transform target; 

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position;
            transform.rotation = Quaternion.identity;
        }
    }
}
