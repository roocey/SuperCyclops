using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using System.Collections;

public class playerController : MonoBehaviour {

    Animator animator;
    Rigidbody2D rb;
    Camera cam;
    Renderer rend;
    Management manage;
    Text coinText;
    EndLevel el;
    ParticleSystem ps;
    AudioSource aud;
    Vortex vort;
    
    public AudioClip audJump;
    public AudioClip audDash;
    public AudioClip audDeath;

    float maxSpeed = 6f;
    float jumpSpeed = 8.25f;
    float jumpXVelocity = 0f;
    float jumpPenaltySpeed = 4.5f; //maximum horizontal velocity while jumping in the opposite direction the jump started in (e.g., making a right-facing jump go left). also applies when jumping with 0 velocity
    float dashing = 1.0f;
    float pushAwayDirection = 0f;
    float camY;
    float camX;
    float wallJumpingClock = 0f; //counter that measures how long the player after contacting a wall to make a wall jump (2/3rds of a second)
    float playerStartX;
    float elX;
    float timeToDie = 0.5f; //time it takes for the level to restart after death

    bool jumping = false;
    bool canDash = true;
    bool isGrounded = true;
    bool wallJumping = false;
    bool gotCoinThisLevel = false;
    bool canJump = false;
    bool dead = false;

    int pushAwayCounter = 0; //number of FixedUpdate ticks the player is pushed away after wall jumping (6)
    int longJumpCounter = 0; //number of Update ticks the player can hold down jump to extend their jump (10)
    int layerMask;

	// Use this for initialization
	void Start () {
        animator = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody2D>();
        aud = this.GetComponent<AudioSource>();
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        vort = cam.gameObject.GetComponent<Vortex>();
        coinText = GameObject.FindWithTag("TextCoin").GetComponent<Text>();
        rend = this.GetComponent<Renderer>();
        layerMask = LayerMask.GetMask("Ground");
        playerStartX = transform.position.x+13.6f;
        el = (EndLevel)FindObjectOfType(typeof(EndLevel));
        manage = (Management)FindObjectOfType(typeof(Management));
        elX = el.gameObject.transform.position.x-9.0f;
        ps = this.GetComponent<ParticleSystem>();
        transform.localScale = new Vector3(0.1f, 0.1f, transform.localScale.z);
    }

    // Update is called once per frame
    void Update () {
        if (vort.radius.x >= 0.1f)
        {
            vort.radius = new Vector2(vort.radius.x + (Time.deltaTime * 5f), vort.radius.y + (Time.deltaTime * 5f));
            if (transform.localScale.x > 0.2f)
            {
                transform.localScale = new Vector3(transform.localScale.x - (Time.deltaTime * 2), transform.localScale.y + (Time.deltaTime * 2), transform.localScale.z);
            }
            if (vort.radius.x >= 4.0f)
            {
                NextLevel();
            }
            return;
        }
        if ((transform.localScale.x != 1.0f || transform.localScale.y != 1.0f) && !dead)
        {
            transform.localScale = new Vector3(transform.localScale.x + (Time.deltaTime*2), transform.localScale.y + (Time.deltaTime*2), transform.localScale.z);
            if (transform.localScale.x >= 1.0f || transform.localScale.y >= 1.0f)
            {
                transform.localScale = new Vector3(1.0f, 1.0f, transform.localScale.z);
            }
        }
        coinText.text = "x" +  manage.coinCounter.ToString();
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        if (playerStartX-16 > transform.position.x || transform.position.x > elX+11)
        {
            //Debug.Log("Out of bounds - reloading level!");
            RestartLevel();
        }
        if (isGrounded)
        {
            camY = transform.position.y - 0.5f;
            canDash = true;
            //jumpXVelocity = 0;

            if (-0.9f > vertical && Mathf.Abs(horizontal) < 0.1f)
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


            if (camY < 7.75f)
            {
                camY = 7.75f;
            }
        }
        else
        {
            if (horizontal >= 0)
            {
                animator.SetInteger("Direction", 2);
                animator.Play("playerJumping"); //overly redundant?
            }
            else if (horizontal < 0)
            {
                animator.SetInteger("Direction", -2);
                animator.Play("playerJumpingLeft"); //see above
            }
            if (!rend.isVisible)
            {
                camY = transform.position.y;
            }
            
        }
        camX = transform.position.x+5;
        if (playerStartX > camX)
        {
            camX = playerStartX;
        }
        if (camX > elX)
        {
            camX = elX;
        }
        //cam.transform.position = new Vector3(cam.transform.position.x, camY, cam.transform.position.z);
        Vector3 nextCamPosition = new Vector3(camX, camY, cam.transform.position.z);
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
            jumpXVelocity = rb.velocity.x;
        }
        if (Input.GetButtonDown("Fire2"))
        {
            if (dashing == 1.0f && canDash)
            {
                aud.volume = 0.3f;
                aud.pitch = Random.Range(0.8f, 1.2f);
                aud.PlayOneShot(audDash);
                dashing = 5.0f;
                canDash = false;
            }
        }
        if (Input.GetButtonDown("Fire3"))
        {
            Application.Quit();
        }
        if (Input.GetButtonDown("Skip"))
        {
            NextLevel();
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 0.5f, layerMask);

        if (hit.collider != null)
        {
            isGrounded = true;
            canJump = true;
            if (Mathf.Abs(rb.velocity.x) > 0.1f)
            {
                wallJumping = false;
                wallJumpingClock = 0;
            }
        }
        else
        {
            isGrounded = false;
        }
        if (wallJumpingClock > 0)
        {
            wallJumpingClock -= Time.deltaTime;
            if (wallJumpingClock <= 0)
            {
                wallJumping = false;
                if (!isGrounded)
                {
                    canJump = false;
                }
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
        if (timeToDie >= 0 && dead)
        {
            timeToDie -= Time.deltaTime;
            transform.localScale = new Vector3(transform.localScale.x - (Time.deltaTime * 2), transform.localScale.y - (Time.deltaTime * 2), transform.localScale.z);
            if (timeToDie <= 0)
            {
                RestartLevel();
            }
        }

    }

    void FixedUpdate()
    {
        float xVelocity = rb.velocity.x;
        float yVelocity = rb.velocity.y;

        if (isGrounded)
        {
            if (vort.radius.x <= 0)
            {
                xVelocity = Input.GetAxis("Horizontal") * maxSpeed * dashing;
            }
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
        if (Input.GetButton("Fire1"))
        {
            if (longJumpCounter > 0)
            {
                yVelocity += jumpSpeed / 30f;
                longJumpCounter--;
            }
        }

        if (jumping && (canJump || wallJumping))
        {
            aud.volume = 0.2f;
            aud.pitch = Random.Range(0.8f, 1.2f);
            aud.PlayOneShot(audJump);
            if (isGrounded || wallJumping)
            {
                //Don't kick up dust if the player is doing a midair jump;
                ps.Play();
            }
            yVelocity = jumpSpeed;
            if (wallJumping)
            {
                canDash = true;
                pushAwayCounter = 6;
                pushAwayDirection = Input.GetAxis("Horizontal");
                wallJumpingClock = 0;
                jumpXVelocity = -pushAwayDirection;
            }
            wallJumping = false;
            canJump = false;
            longJumpCounter = 15;
        }

        jumping = false;

        if (pushAwayCounter > 0)
        {
            pushAwayCounter -= 1;
            xVelocity = -pushAwayDirection * maxSpeed * dashing * 1.11f;
        }
        if (dashing > 1)
        {
            if (yVelocity < 0)
            {
                yVelocity = 0;
            }
        }

        if (dead)
        {
            xVelocity = 0;
            yVelocity = 0;
        }
        rb.velocity = new Vector2(xVelocity, yVelocity);

    }
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Lethal")
        {
            dead = true;
            aud.pitch = Random.Range(0.8f, 1.2f);
            aud.PlayOneShot(audDeath);
        }
        else if (coll.gameObject.tag == "Goal")
        {
            vort.radius = new Vector2(0.1f, 0.1f);
            //NextLevel();
        }
        else if (coll.gameObject.tag == "Coin")
        {
            Destroy(coll.gameObject);
            gotCoinThisLevel = true;
            manage.coinCounter++;
        }
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        float horizontal = Input.GetAxis("Horizontal");
        if (!isGrounded)
        {
            if (horizontal > 0 && coll.transform.position.x > transform.position.x)
            {
                wallJumping = true;
                wallJumpingClock = 0.09f;
                transform.position = new Vector3(transform.position.x, transform.position.y - (Time.fixedDeltaTime), transform.position.z);
                canJump = true;
            }
            else if (horizontal < 0 && coll.transform.position.x < transform.position.x)
            {
                wallJumping = true;
                wallJumpingClock = 0.09f;
                transform.position = new Vector3(transform.position.x, transform.position.y - (Time.fixedDeltaTime), transform.position.z);
                canJump = true;
            }
        }
        else
        {
            //Debug.Log("Absolute horizontal input: " + Mathf.Abs(horizontal) + "& x-velocity: " + rb.velocity.x);
            if (Mathf.Abs(horizontal) > 0.2f && Mathf.Abs(rb.velocity.x) <= 0.1f)
            {
                wallJumping = true;
                wallJumpingClock = 0.09f;
                canJump = true;
            }
        }
    }

    void RestartLevel()
    {
        if (gotCoinThisLevel)
        {
            manage.coinCounter--;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void NextLevel()
    {
        //could use a test function of some kind (Scene.isValid() ? )
        vort.radius = new Vector2(0, 0);
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextScene);
    }
} 
