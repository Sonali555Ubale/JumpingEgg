using UnityEngine;

public class TrajectoryLine : MonoBehaviour
{
    [SerializeField] [Range(0f, 1f)] private float _time;
    [SerializeField] private float power = 9f;
    [SerializeField] private FixedJoystick joystick;
    private LineRenderer lr;
    private Rigidbody2D rb;
    Vector2 startDragPosition;
    Vector2 currentVelocity;
    private bool isJumping;
    private bool isTrajectoryEnabled = false;
    Vector2 joystickInput;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        // jumping state based on the player's Y velocity
        isJumping = Mathf.Abs(rb.velocity.y) > 0.1f;
        Debug.LogError("IsJumping Status" + isJumping);
        joystickInput = new Vector2(joystick.Horizontal, joystick.Vertical);

        if (isJumping==false && joystickInput.magnitude > 0 && joystickInput.y <= 0)
        {
            EnableTrajectory();
        }
        else if (joystickInput.magnitude == 0 && isTrajectoryEnabled)
        {
            // Jump only if the trajectory was enabled and joystick input is released
           Jump();
        }
        else
        {
            DisableTrajectory();
        }
    }

    private void EnableTrajectory()
    {
        isTrajectoryEnabled = true;
        lr.enabled = true;
        Time.timeScale = _time;

        // Direction for left and right input is angled upward
        Vector2 direction = new Vector2(joystickInput.x, -joystickInput.y).normalized;
        startDragPosition = (Vector2)transform.position + direction * power;
        currentVelocity = (startDragPosition - (Vector2)transform.position) * power;

        AudioManager.instance.Play("jump");

        if(isJumping==false) PlotTrajectory((Vector2)transform.position, currentVelocity);
    }

    private void DisableTrajectory()
    {
        if (isTrajectoryEnabled && isJumping==true)
        {
            lr.enabled = false;
            Time.timeScale = 1f;
        }
    }

    private void Jump()
    {
        // Ensure time scale is reset before applying jump
        Time.timeScale = 1f;
        rb.velocity = currentVelocity;
        isJumping = true;
        DisableTrajectory();  
        isTrajectoryEnabled = false;
        Debug.LogError("Trajectory Disabled");
    }


    private void PlotTrajectory(Vector2 start, Vector2 velocity)
    {
        int steps = 500;
        Vector2[] results = new Vector2[steps];
        float timeStep = Time.fixedDeltaTime / Physics2D.velocityIterations;
        Vector2 gravityAccel =   rb.gravityScale * timeStep * timeStep* Physics2D.gravity;
        float drag = 1f - timeStep * rb.drag;
        Vector2 moveStep = velocity * timeStep;

        for (int i = 0; i < steps; i++)
        {
            moveStep += gravityAccel;
            moveStep *= drag;
            start += moveStep;
            results[i] = start;
        }

        lr.positionCount = results.Length;
        for (int i = 0; i < results.Length; i++)
        {
            lr.SetPosition(i, results[i]);
        }
    }

}
