using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class playerController : MonoBehaviour {

    Animator animator;
    Rigidbody2D rb;
    Camera cam;
    Renderer rend;
    float maxSpeed = 6f;
    float jumpSpeed = 13f;
    float jumpXVelocity = 0f;
    float jumpPenaltySpeed = 4.5f; //maximum horizontal velocity while jumping in the opposite direction the jump started in (e.g., making a right-facing jump go left). also applies when jumping with 0 velocity
    bool jumping = false;
    float dashing = 1.0f;
    bool isGrounded = true;
    bool wallJumping = false;
    float slipJumpTimer = 0f;
    bool slipCheck = true;
    int pushAwayCounter = 0; //number of FixedUpdate ticks the player is pushed away after wall jumping
    float pushAwayDirection = 0f;
    float camY = 7.75f;
    int layerMask;
    float wallJumpingClock = 0f; //counter that measures how long the player after contacting a wall to make a wall jump (2/3rds of a second)

	// Use this for initialization
	void Start () {
        animator = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody2D>();
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        rend = this.GetComponent<Renderer>();
        layerMask = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void Update () {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        if (isGrounded)
        {
            camY = transform.position.y - 0.5f;

            if (vertical < 0 && Mathf.Abs(horizontal) < 0.2f)
            {
                animator.SetInteger("Direction", 3);
                camY = transform.position.y - 1.75f;
            }
            else if (horizontal > 0)
            {
                animator.SetInteger("Direction", 1);
            }
            else if (horizontal < 0)
            {
                animator.SetInteger("Direction", -1);
            }
            else
            {
                animator.SetInteger("Direction", 0);
            }


            if (camY < 10.0f)
            {
                camY = 10.0f;
            }
        }
        else
        {
            if (jumpXVelocity >= 0)
            {
                animator.SetInteger("Direction", 2);
                animator.Play("playerJumping"); //overly redundant?
            }
            else
            {
                animator.SetInteger("Direction", -2);
                animator.Play("playerJumpingLeft"); //see above
            }
            if (!rend.isVisible)
            {
                camY = transform.position.y;
            }
            
        }
        //cam.transform.position = new Vector3(cam.transform.position.x, camY, cam.transform.position.z);
        Vector3 nextCamPosition = new Vector3(transform.position.x, camY, cam.transform.position.z);
        if (rend.isVisible)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, nextCamPosition, Time.deltaTime*1.33f);
        }
        else
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, nextCamPosition, Time.deltaTime*2.67f);
        }

        if (Input.GetButtonDown("Fire1")) {
            jumping = true;
            slipCheck = false;
            jumpXVelocity = rb.velocity.x;
        }
        if (Input.GetButtonDown("Fire2"))
        {
            if (dashing == 1.0f)
            {
                dashing = 5.0f;
            }
        }
        if (Input.GetButtonDown("Fire3"))
        {
            Application.Quit();
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 1.0f, layerMask);

        if (hit.collider != null)
        {
            isGrounded = true;
            slipCheck = true;
            wallJumping = false;
            wallJumpingClock = 0;
        }
        else
        {
            isGrounded = false;
            if (slipCheck)
            {
                slipJumpTimer = 0.35f;
                slipCheck = false;
            }
        }
        if (wallJumpingClock > 0)
        {
            wallJumpingClock -= Time.deltaTime;
            if (wallJumpingClock <= 0)
            {
                wallJumping = false;
            }
        }
        if (slipJumpTimer > 0)
        {
            slipJumpTimer -= Time.deltaTime;
            if (slipJumpTimer <= 0)
            {
                slipJumpTimer = 0;
            }
        }
        if (dashing > 1)
        {
            dashing -= Time.deltaTime*10;
            if (dashing <= 1)
            {
                dashing = 1;
            }
        }

    }

    void FixedUpdate()
    {
        float xVelocity = rb.velocity.x;
        float yVelocity = rb.velocity.y;

        if (isGrounded)
        {
            xVelocity = Input.GetAxis("Horizontal") * maxSpeed * dashing;
        }
        else
        {
            xVelocity = Input.GetAxis("Horizontal") * maxSpeed * dashing;
            if (jumpXVelocity > -0.09 && jumpXVelocity < 0.09)
            {
                if (xVelocity < -jumpPenaltySpeed)
                {
                    xVelocity = -jumpPenaltySpeed * dashing;
                }
                else if (xVelocity > jumpPenaltySpeed)
                {
                    xVelocity = jumpPenaltySpeed * dashing;
                }
            }
            else if (jumpXVelocity > 0)
            {
                if (xVelocity < -jumpPenaltySpeed)
                {
                    xVelocity = -jumpPenaltySpeed * dashing;
                }
            }
            else if (jumpXVelocity < 0)
            {
                if (xVelocity > jumpPenaltySpeed)
                {
                    xVelocity = jumpPenaltySpeed * dashing;
                }
            }
        }

        if (jumping && (isGrounded || wallJumping || slipJumpTimer > 0))
        {
            yVelocity = jumpSpeed;
            slipJumpTimer = 0;
            if (wallJumping)
            {
                pushAwayCounter = 12;
                pushAwayDirection = Input.GetAxis("Horizontal");
                wallJumping = false;
                wallJumpingClock = 0;
                jumpXVelocity = -pushAwayDirection;
            }
        }

        jumping = false;

        if (pushAwayCounter > 0)
        {
            pushAwayCounter -= 1;
            xVelocity = -pushAwayDirection * maxSpeed * dashing * 1.11f;
        }
        if (dashing > 1)
        {
            yVelocity = yVelocity * 0.9f;
        }

        rb.velocity = new Vector2(xVelocity, yVelocity);

    }

    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Lethal")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (!isGrounded)
        {
            float horizontal = Input.GetAxis("Horizontal");
            if (horizontal > 0 && coll.transform.position.x > transform.position.x)
            {
                wallJumping = true;
                wallJumpingClock = 0.09f;
                transform.position = new Vector3(transform.position.x, transform.position.y - (Time.fixedDeltaTime), transform.position.z);
            }
            else if (horizontal < 0 && coll.transform.position.x < transform.position.x)
            {
                wallJumping = true;
                wallJumpingClock = 0.09f;
                transform.position = new Vector3(transform.position.x, transform.position.y - (Time.fixedDeltaTime), transform.position.z);
            }
        }
    }
} 
