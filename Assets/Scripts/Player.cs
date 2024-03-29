using UnityEngine;


[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour
{
    public float jumpHeight = 4;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    float moveSpeed = 6;
    
    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    [SerializeField] float slideSpeed = 9.0f;

    Controller2D controller;

    void Start()
    {
        controller = GetComponent<Controller2D> ();
        
        gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print ("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);
    }

    void Update()
    {
        if (controller.collisions.above || controller.collisions.below)
            velocity.y = 0;

        Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
        
        // JUMP
        if (Input.GetKeyDown (KeyCode.Space) && controller.collisions.below)
            velocity.y = jumpVelocity;

        // GROUNDED SLIDE (Remove second condition to make it a dash that can also be performed in the air)
        if (Input.GetKeyDown(KeyCode.S) && controller.collisions.below)
            velocity.x = velocity.x > 0 ? velocity.x += slideSpeed : velocity.x -= slideSpeed;

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
        controller.Move (velocity * Time.deltaTime);
    }
}


// if (velocity.x > 0)
//     velocity.x += slideSpeed;
// else
//     velocity.x -= slideSpeed;
