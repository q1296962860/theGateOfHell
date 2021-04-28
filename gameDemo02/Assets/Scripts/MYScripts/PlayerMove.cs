using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Rigidbody2D rb;
    private BoxCollider2D coll;

    [Header("移动参数")]
    public float speed = 8f;
    public float crouchSpeedDivisor = 3f;

    [Header("跳跃参数")]
    public float jumpForce = 6.3f;
    public float jumpHoldForce = 1.9f;
    public float jumpHoldDuration = 0.1f;//跳跃长按时间
    public float crouchJumpBoost = 2.5f;

    float jumpTime;//保证跳跃只持续一定的短时间

    [Header("状态")]
    public bool isCrouch;  //默认初始值为false
    public bool isOnGround;
    public bool isJump;    //判断是否可以连续跳跃（即已经在空中）
    public bool isHeadBlocked;//判断头顶接触

    [Header("环境检测")]
    public float footOffset = 0.4f;
    public float headClearance = 0.5f;
    public float groundDistance = 0.2f;

    public LayerMask groundLayer;

    public float xVelocity;

    //按键设置
    bool jumpPressed;//单次跳跃
    bool jumpHeld;   //长按跳跃
    bool crouchHeld; //长按下蹲

    //碰撞体尺寸
    Vector2 ColliderStandSize;
    Vector2 ColliderStandOffset;
    Vector2 ColliderCrouchSize;
    Vector2 ColliderCrouchOffset;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();

        ColliderStandSize = coll.size;
        ColliderStandOffset = coll.offset;
        ColliderCrouchSize = new Vector2(coll.size.x, coll.size.y / 2f);
        ColliderCrouchOffset = new Vector2(coll.offset.x, coll.offset.y / 2f);
    }

    // Update is called once per frame
    void Update()
    {
        jumpPressed = Input.GetButtonDown("Jump");
        jumpHeld = Input.GetButton("Jump");
        crouchHeld = Input.GetButton("Crouch");
    }
    private void FixedUpdate()
    {
        PhysicsCheck();
        GroundMovement();
        MidAirMovement();
    }
    //物理检测
    void PhysicsCheck()
    {
        /*Vector2 pos = transform.position;
        Vector2 offset = new Vector2(-footOffset, 0f);
        //射线
        RaycastHit2D leftCheck = Physics2D.Raycast(pos+offset,Vector2.down,groundDistance ,groundLayer );
        Debug.DrawRay(pos + offset, Vector2.down, Color.red, 0.2f);
        */
        RaycastHit2D leftCheck = Raycast(new Vector2 (-footOffset ,0f),Vector2 .down,groundDistance,groundLayer );
        RaycastHit2D rightCheck = Raycast(new Vector2(footOffset, 0f), Vector2.down, groundDistance, groundLayer);
        if (leftCheck||rightCheck )
            isOnGround = true;
        else
            isOnGround = false;
        RaycastHit2D headCheck = Raycast(new Vector2(0f, coll.size.y), Vector2.up, headClearance, groundLayer);
        if (headCheck)
            isHeadBlocked = true;
        else
            isHeadBlocked = false;
    }
    //地面移动
    void GroundMovement()
    {
        if (crouchHeld && !isCrouch && isOnGround)
            Crouch();
        else if (!crouchHeld && isCrouch && !isHeadBlocked)
            StandUp();
        else if (!isOnGround && isCrouch)
            StandUp();

        if (isCrouch)
            xVelocity /= crouchSpeedDivisor;
        xVelocity = Input.GetAxis("Horizontal");//[-1f ,1f]
        rb.velocity = new Vector2(xVelocity*speed, rb.velocity.y);
        FilpDirction();
    }
    //跳跃
    void MidAirMovement()
    {
        if(jumpPressed && isOnGround && !isJump && !isHeadBlocked )
        {
            if(isCrouch)
            {
                StandUp();
                rb.AddForce(new Vector2(0f, crouchJumpBoost), ForceMode2D.Impulse);
            }
            isOnGround = false;
            isJump = true;
            jumpTime = Time.time + jumpHoldDuration;
            //施加一个向上的力，并且是突然发生的
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            AudioManager.PlayJumpAudio(); 
        }
        else if(isJump)
        {
            if (jumpHeld)
                rb.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);
            if(jumpTime<Time.time)
                isJump = false;
        }
    }
    //人物转向
    void FilpDirction()
    {
        if (xVelocity < 0)
            rb.transform.localScale = new Vector2(-1, 1);
        if (xVelocity > 0)
            rb.transform.localScale = new Vector2(1, 1);
    }
    void Crouch()
    {
        isCrouch = true;
        coll.size = ColliderCrouchSize;
        coll.offset = ColliderCrouchOffset;
    }
    void StandUp()
    {
        isCrouch = false;
        coll.size = ColliderStandSize;
        coll.offset = ColliderStandOffset;
    }
    //Raycast自定义、可复用
    RaycastHit2D Raycast(Vector2 offset,Vector2 rayDirection,float length,LayerMask layer)
    {
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(pos+offset, rayDirection, length, layer);
        Color color = hit ? Color.red : Color.green;
        Debug.DrawRay(pos + offset,rayDirection *length, color);
        return hit;
    }
}

