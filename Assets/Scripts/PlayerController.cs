using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public SpriteRenderer characterSprite;
    public Sprite lamAnh;
    public Sprite tinh;
    public Sprite anhVu;
    public float moveSpeed;
    private float moveSpeedOriginal;
    public float speedMultiplier;

    public float speedUpDistance;
    private float speedUpDistanceOriginal;
    private float speedUpDistanceCount;

    public float jumpForce;
    private Rigidbody2D myRigidbody;
   
    public bool grounded;
    public LayerMask WhatIsGround;
    public Transform groundCheck;
    public float groundCheckRadius;

    public AudioSource jumpSound;
    public AudioSource deathSound;

    //private Collider2D myCollider;

    private Animator myAnimator;

    public float jumpTime;
    private float jumpTimeCounter;
    private bool stoppedJumping;
    private bool canDoubleJump;
    private bool startedJumping;

    public GameManager theGameManager;

    [Header("Jump Settings")]
    public float maxHeightLimit = 4f; // Độ cao tối đa được phép nhảy (tính theo trục Y)
    public float hangTimeGravity = 0.2f; // Trọng lực khi đang lơ lửng ở đỉnh (nhỏ hơn 1 là bay chậm)
    public float fallMultiplier = 4.5f;  // Khi rơi xuống thì rơi nhanh hơn cho có lực
    private float defaultGravity;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();

        //myCollider = GetComponent<Collider2D>();

        myAnimator = GetComponent<Animator>();

        jumpTimeCounter = jumpTime;

        speedUpDistanceCount = speedUpDistance;

        moveSpeedOriginal = moveSpeed;

        speedUpDistanceOriginal = speedUpDistanceCount;

        stoppedJumping = true;
        startedJumping = false;
        int selected = PlayerPrefs.GetInt("SelectedCharacter", 0);

        switch (selected)
        {
            case 0:
                characterSprite.sprite = lamAnh;
                break;
            case 1:
                characterSprite.sprite = tinh;
                break;
            case 2:
                characterSprite.sprite = anhVu;
                break;
        }
        defaultGravity = myRigidbody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        //grounded = Physics2D.IsTouchingLayers(myCollider, WhatIsGround);

        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, WhatIsGround);

        if (transform.position.x > speedUpDistanceCount)
        {
            speedUpDistanceCount += speedUpDistance;

            speedUpDistance = speedUpDistance * speedMultiplier;

            moveSpeed = moveSpeed * speedMultiplier;
        }

        myRigidbody.linearVelocity = new Vector2(moveSpeed, myRigidbody.linearVelocity.y);

        // --- XỬ LÝ TRỌNG LỰC & TRẦN (ĐÃ SỬA) ---

        // 1. Kiểm tra xem có đang ở trên trần không
        bool isAboveCeiling = transform.position.y > maxHeightLimit;

        // Nếu đang vượt quá trần
        if (isAboveCeiling)
        {
            // Nếu vẫn đang cố bay lên -> Hãm phanh lại từ từ (Soft Ceiling)
            if (myRigidbody.linearVelocity.y > 0)
            {
                myRigidbody.linearVelocity = new Vector2(myRigidbody.linearVelocity.x, myRigidbody.linearVelocity.y * 0.5f);
            }

            // QUAN TRỌNG: Khi đang ở trần, dùng trọng lực nhẹ (Hang Time) hoặc Bình thường
            // Để tạo cảm giác "hơi khựng lại" (delay) trước khi rơi
            myRigidbody.gravityScale = defaultGravity * hangTimeGravity; 
        }
        // 2. Nếu đang ở đỉnh cú nhảy (vận tốc gần bằng 0) -> Treo lơ lửng (Hang Time)
        else if (Mathf.Abs(myRigidbody.linearVelocity.y) < 0.5f)
        {
            myRigidbody.gravityScale = defaultGravity * hangTimeGravity;
        }
        // 3. Nếu đang rơi xuống (VÀ phải thấp hơn trần) -> Mới cho rơi nhanh
        else if (myRigidbody.linearVelocity.y < 0)
        {
            myRigidbody.gravityScale = defaultGravity * fallMultiplier;
        }
        else
        {
            myRigidbody.gravityScale = defaultGravity;
        }

        if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (grounded)
            {
                myRigidbody.linearVelocity = new Vector2(myRigidbody.linearVelocity.x, jumpForce);
                stoppedJumping = false;
                startedJumping = true;
                jumpSound.Play();
            }
            if (!grounded && canDoubleJump)
            {
                myRigidbody.linearVelocity = new Vector2(myRigidbody.linearVelocity.x, jumpForce);
                jumpTimeCounter = jumpTime;
                stoppedJumping = false;
                canDoubleJump = false;
                jumpSound.Play();
            }

        }

        if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && !stoppedJumping)
        {
            if(jumpTimeCounter > 0)
            {
                myRigidbody.linearVelocity = new Vector2(myRigidbody.linearVelocity.x, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
            }

        }

        if (Input.GetKeyUp (KeyCode.Space) || Input.GetMouseButtonUp(0))
        {
            jumpTimeCounter = 0;
            stoppedJumping = true;
        }

        if (grounded)
        {
            jumpTimeCounter = jumpTime;
            canDoubleJump = true;
            startedJumping = false;
        }

        myAnimator.SetFloat("Speed", myRigidbody.linearVelocity.x);
        myAnimator.SetBool("Grounded", grounded);
    }

    void OnCollisionEnter2D (Collision2D other)
    {
        if(other.gameObject.tag == "killbox")
        {
            //PlatformDestroyer destroyer =
            //other.gameObject.GetComponentInChildren<PlatformDestroyer>();

            //if (destroyer != null && destroyer.myAnimator != null)
            //{
            //    destroyer.myAnimator.Play("VuNoLon");
            //}
            theGameManager.RestartGame();
            moveSpeed = moveSpeedOriginal;
            speedUpDistanceCount = speedUpDistanceOriginal;
            speedUpDistance = speedUpDistanceOriginal;
            deathSound.Play();
        }
    }
}
