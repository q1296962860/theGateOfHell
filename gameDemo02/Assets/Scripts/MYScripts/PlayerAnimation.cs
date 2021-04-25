using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    Animator anim;
    Rigidbody2D rb;
    PlayerMove move;//获得绑定在Player上的脚本
    int groundID;//编号形式记录状态（bool型）
    int crouchID;
    int speedID;
    int fallID;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        move = GetComponentInParent<PlayerMove >();
        groundID = Animator.StringToHash("isOnGround");
        crouchID = Animator.StringToHash("isCrouching");
        speedID = Animator.StringToHash("speed");
        fallID = Animator.StringToHash("verticalVelocity");
    }

    void Update()
    {
        anim.SetFloat(speedID, Mathf.Abs(move.xVelocity));
        //anim.SetBool("isOnGround",move.isOnGround );
        anim.SetBool(groundID, move.isOnGround);
        anim.SetBool(crouchID, move.isCrouch);
        anim.SetFloat(fallID, rb.velocity.y);
    }
}
