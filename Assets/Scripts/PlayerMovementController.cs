using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using System.Collections;

public class PlayerMovementController : MonoBehaviour {

    Rigidbody2D rb;
    Management manage;
    Text coinText;
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
    float wallJumpingClock = 0f; //counter that measures how long the player after contacting a wall to make a wall jump (2/3rds of a second)
    float timeToDie = 0.5f; //time it takes for the level to restart after death
    [HideInInspector]
    public float horizontal;
    [HideInInspector]
    public float vertical;

    bool jumping = false;
    bool canDash = true;
    bool wallJumping = false;
    bool gotCoinThisLevel = false;
    bool canJump = false;
    bool dead = false;
    [HideInInspector]
    public bool isGrounded = true;

    int pushAwayCounter = 0; //number of FixedUpdate ticks the player is pushed away after wall jumping
    int longJumpCounter = 0; //number of Update ticks the player can hold down jump to extend their jump
    int layerMask;

	// Use this for initialization
	void Start () {
        rb = this.GetComponent<Rigidbody2D>();
        aud = this.GetComponent<AudioSource>();
        vort = (Vortex)FindObjectOfType(typeof(Vortex));
        coinText = GameObject.FindWithTag("TextCoin").GetComponent<Text>();
        layerMask = LayerMask.GetMask("Ground");
        manage = (Management)FindObjectOfType(typeof(Management));
        ps = this.GetComponent<ParticleSystem>();
        transform.localScale = new Vector3(0.1f, 0.1f, transform.localScale.z);
    }

    // Update is called once per frame
    void Update () {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        if (vort.radius.x >= 0.1f)
        {
            LevelEndEffect();
        }
        if ((transform.localScale.x != 1.0) && !dead)
        {
            LevelStartEffect();
        }
        coinText.text = "x" +  manage.coinCounter.ToString();
        if (isGrounded)
        {
            canDash = true;
        }
        if (Input.GetButtonDown("Fire1")) {
            jumping = true;
            jumpXVelocity = rb.velocity.x;
        }
        if (Input.GetButtonDown("Fire2"))
        {
            if (dashing == 1.0f && canDash)
            {
                PlaySound(audDash);
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
        CheckForCollision();

        TickTock();
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
                aud.pitch += 0.02f;
            }
        }

        if (jumping && (canJump || wallJumping))
        {
            PlaySound(audJump, 0.2f);
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
            PlaySound(audDeath);
        }
        else if (coll.gameObject.tag == "Goal")
        {
            vort.radius = new Vector2(0.1f, 0.1f); //setting the vortex's radius to >= 0.1f will trigger LevelEndEffect() which will, in turn, trigger NextLevel()
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
                WallJump();
            }
            else if (horizontal < 0 && coll.transform.position.x < transform.position.x)
            {
                WallJump();
            }
        }
        else
        {
            //Debug.Log("Absolute horizontal input: " + Mathf.Abs(horizontal) + "& x-velocity: " + rb.velocity.x);
            if (Mathf.Abs(horizontal) > 0.2f && Mathf.Abs(rb.velocity.x) <= 0.1f)
            {
                WallJump(false);
            }
        }
    }

    void WallJump(bool sliding=true)
    {
        wallJumping = true;
        wallJumpingClock = 0.09f;
        canJump = true;
        if (sliding)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - (Time.fixedDeltaTime), transform.position.z);
        }
    }

    public void RestartLevel()
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

    void PlaySound(AudioClip ac, float volume=0.3f)
    {
        aud.volume = volume;
        aud.pitch = Random.Range(0.8f, 1.2f);
        aud.PlayOneShot(ac);
    }

    void LevelEndEffect()
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
    }

    void LevelStartEffect()
    {
        transform.localScale = new Vector3(transform.localScale.x + (Time.deltaTime * 2), transform.localScale.y + (Time.deltaTime * 2), transform.localScale.z);
        if (transform.localScale.x >= 1.0f || transform.localScale.y >= 1.0f)
        {
            transform.localScale = new Vector3(1.0f, 1.0f, transform.localScale.z);
        }
    }

    void TickTock() //countdown various things
    {
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
            dashing -= Time.deltaTime * 10;
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

    void CheckForCollision()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 0.6f, layerMask);
        Debug.DrawRay(transform.position, -Vector2.up, Color.green, 0.3f);

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
    }
} 
