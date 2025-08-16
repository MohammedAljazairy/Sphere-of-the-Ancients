using UnityEngine;

public class Move : MonoBehaviour
{   
    public float speed = 50.0f;
    public float jumpthrust = 50.0f;
    public float runMultiplier = 2f;
    public float maxAngularVelocity = 100f;// I have found out that Unity max velocity is 7 https://docs.unity3d.com/ScriptReference/Rigidbody-maxAngularVelocity.html
    public Transform cine;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private bool isGrounded;
    public GameObject check;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxAngularVelocity;
    }

    void Update()
    {
        
        if (Input.GetKey(KeyCode.LeftShift))
            speed = 50 * runMultiplier; 
        else
            speed = 50;

        
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            float jumpPower = Input.GetKey(KeyCode.LeftShift) ? 
                            jumpthrust * runMultiplier : 
                            jumpthrust;
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            
        }
    }

    void FixedUpdate()
    {

        
        isGrounded = Physics.CheckSphere(transform.position, 0.6f, groundLayer);//youtube video https://docs.unity3d.com/ScriptReference/Physics.CheckSphere.html  https://www.youtube.com/watch?v=WSlcPIVLI1w

    
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 movementVector = new Vector3(x, 0, z);
        
        movementVector = Quaternion.AngleAxis(cine.rotation.eulerAngles.y, Vector3.up) * movementVector;
        movementVector.Normalize();

        rb.AddTorque(new Vector3(movementVector.z, 0, -movementVector.x) * speed); //https://www.youtube.com/watch?v=VVznfJBqfuo
    }

    
   
}