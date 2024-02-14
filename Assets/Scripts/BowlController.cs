using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlController : MonoBehaviour
{

    public Rigidbody2D rb;
    [SerializeField]
    public float speed = 1f;

    [SerializeField] // just debugging making it visible
    private float speedMultiplier;

    public float limiter;
    public bool isBowlStationary;

    [SerializeField]
    public bool isInitializing = true; // Flag to indicate the initial setup phase

    public Animator animator;

    public static Object prefab;
    [SerializeField]
    public int bowlType = 0;

    [SerializeField]
    private bool shouldFlick = false;
    private TutorialManager tutorialManager;

    int[] bowlTypeArr =
    {
        1, // HORIZONTAL,
        2, // VERTICAL,
        3,  // H + V
        4, // EIGHT
        5, // ACROSS_SCREEN,
        6, // WOBBLE,
        7, // SIN_WAVE,
        8, // SQR_WAVE,
        9  // ROTATING
    };

    float _x, _y;
    float startY; // For vertical
    float deviceWidth;
    float screenLimit = 0.5f;

    void InitializeStartBowls()
    {
        // Your logic here to place the initial set of bowls with fixed states
        // After placing them, set isInitializing to false
        isInitializing = false;
    }

    public BowlController Create()
    {
        prefab = Resources.Load("Prefabs/bowl");

        if (prefab == null)
        {
            Debug.LogError("Bowl prefab not found in Resources/Prefabs/bowl");
            return null;
        }
        GameObject newObject = Instantiate(prefab) as GameObject;
        BowlController yourObject = newObject.GetComponent<BowlController>();
        //do additional initialization steps here
        return yourObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeStartBowls();
        animator = gameObject.GetComponent<Animator>();
        tutorialManager = FindObjectOfType<TutorialManager>();

        if (speed == 0)
            speed = Random.Range(0.2f, 0.6f);
        //speed = 0.2f;
        limiter = .2f;
        // Generating a random number just to change the direction of start for the bowl

        if (isBowlStationary)
            rb.position = new Vector2(Random.Range(-0.6f, 0.6f), rb.position.y);
        //speed = Random.Range(0.5f, 1f);
        //Debug.Log("Start() with "+rnum +" named as "+gameObject.name);

        // TODO: Bowl type randomness

        // Initialize startY
        startY = rb.position.y + 0.6f; // 0.2 is an offset


        deviceWidth = Screen.width;
        //Debug.LogError(deviceWidth);

        //Vector3 tempPosY = transform.position;
        //Debug.LogError(Camera.main.WorldToScreenPoint(new Vector3(deviceWidth, 0, 0)));

        // Change mode
        if (bowlType == 0)
        {
            bowlType = Random.Range(1, 4);
        }
        // Debug.LogError("Bowl type " + bowlType);
    }

    public void playFlickAnimation()
    {

        //Debug.Log("");
        //Debug.Log("should flick");
        animator.SetTrigger("shouldFlick");
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && shouldFlick)
        {
            if (GameManager.instance.isPauseOrPlayTouched()) return;
            playFlickAnimation();
        }

        // Check if the bowl is near the screen edge and reverse direction if needed
        ReverseDirectionAtScreenEdges();
    }


    private void ReverseDirectionAtScreenEdges()
    {
        Vector3 screenPosition = Camera.main.WorldToViewportPoint(transform.position);
        if (screenPosition.x <= 0 || screenPosition.x >= 1)
        {
            // Reverse horizontal velocity
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name.Equals("egg2"))
        {
            shouldFlick = true;

            // TODO: Play dust particle animation
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        shouldFlick = false;
        //Debug.Log("exit2d");
        //animator.ResetTrigger("shouldFlick");
        if (this.gameObject.name == "bowl3")
        {
        //    Debug.LogError("Tag on Bowl is" + this.gameObject.name);
            tutorialManager.player.transform.position = tutorialManager.endBasket.transform.position;
        }
    }

    void FixedUpdate()    
    {
        if (!isBowlStationary)                  // increases the speed of the bowl momemnet by the speed multiplier
        {
            // Calculate the speed multiplier from the GameManager.
            speedMultiplier = GameManager.CalculateSpeedMultiplier();
                     move(bowlType, speed * speedMultiplier);
        }
    }

    void move(int type, float adjustedSpeed)
    {
        switch (type)
        {
            case 1: typeHorizontal(adjustedSpeed); break;
            //case 2: typeVertical(); break;
            //case 3: typeVH(); break;
            //case 5: typeHorizontal(true); break;
            //case 2: RotateBasket(); break;
            default: typeHorizontal(adjustedSpeed); break;
        }
    }

    void typeVH()
    {
        _y = rb.position.y;
        if (_y > startY + screenLimit)
        {
            rb.velocity += (Vector2.down * speed) / limiter * Time.fixedDeltaTime;
        }
        else if (_y < startY - screenLimit)
        {
            rb.velocity += (Vector2.up * speed) / limiter * Time.fixedDeltaTime;
        }

        _x = rb.position.x;
        if (_x > 0.5)
        {
            rb.velocity += (Vector2.left * speed) / limiter * Time.fixedDeltaTime;
        }
        else if (_x < -0.5)
        {
            rb.velocity += (Vector2.right * speed) / limiter * Time.fixedDeltaTime;
        }
    }
    void typeVertical()
    {
        _y = rb.position.y;
        if (_y > startY + screenLimit)
        {
            rb.velocity += (Vector2.down * speed) / limiter * Time.fixedDeltaTime;
        }
        else if (_y < startY - screenLimit)
        {
            rb.velocity += (Vector2.up * speed) / limiter * Time.fixedDeltaTime;
        }
    }

    void typeHorizontal(float adjustedSpeed, bool isAcrossScreen = false)
    {
        _x = rb.position.x;
        if (isAcrossScreen)
        {
            Vector3 tmpPos = Camera.main.WorldToScreenPoint(transform.position);

            tmpPos.x = Mathf.Clamp(tmpPos.x, -20, Screen.width + 150);
            rb.velocity = new Vector2(adjustedSpeed * (tmpPos.x > Screen.width ? -1 : 1), rb.velocity.y);
            if (tmpPos.x > Screen.width + 100)
            {
                transform.position = Camera.main.ScreenToWorldPoint(new Vector3(-20, tmpPos.y, tmpPos.z));
            }
        }
        else
        {
            if (_x > screenLimit)
            {
                rb.velocity = new Vector2(-adjustedSpeed, rb.velocity.y);
            }
            else if (_x < -screenLimit)
            {
                rb.velocity = new Vector2(adjustedSpeed, rb.velocity.y);
            }
        }
    }



    public float rot = 0f;
    public float rotationSpeed = 400f;
    void RotateBasket()
    {

        Vector3 tempPos = Camera.main.WorldToScreenPoint(transform.position);

        tempPos.x = Mathf.Clamp(tempPos.x, -50, Screen.width);
        if (tempPos.x > Screen.width - 150)
        {
            rb.velocity = Vector2.zero;
            //rb.velocity += (Vector2.left * speed) / limiter * Time.deltaTime;
            rot -= Time.deltaTime * rotationSpeed;
            transform.rotation = Quaternion.Euler(0, 0, rot);
            if (rot <= 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                rb.velocity += Vector2.left * speed;

            }

        }
        else if (tempPos.x < 150)
        {
            rb.velocity = Vector2.zero;
            //rb.velocity += (Vector2.right * speed) / limiter * Time.deltaTime;
            rot += Time.deltaTime * rotationSpeed;
            transform.rotation = Quaternion.Euler(0, 0, rot);
            if (rot >= 360)
            {

                transform.rotation = Quaternion.Euler(0, 0, 0);
                rb.velocity += Vector2.right * speed;

            }


        }
    }
}
