using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isGrounded = true;
    public bool isFalling = false;

    [SerializeField] Animator playerAnim;
    [SerializeField] Rigidbody2D playerRb;

    float speed = 150;
    float jumpForce = 4;
    float horizontalInput;
    float smoothTime = 0.3f;

    Vector3 refVelocity = Vector3.zero;

    bool jump = false;
    bool attacking = false;
    bool crouching = false;
    bool side = true;//true for right

    BoxCollider2D myCol;

    // Start is called before the first frame update
    void Start()
    {
        myCol = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput > 0 && !side)
            Flip();
        else if (horizontalInput < 0 && side)
            Flip();

        //Debug.Log(horizontalInput);
        if (horizontalInput != 0)
        {
            playerAnim.SetBool("running", true);
        }
        else
        {
            playerAnim.SetBool("running", false);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jump = true;
        }
        if (isFalling)
        {
            playerAnim.SetBool("falling", true);
        }
        else
        {
            playerAnim.SetBool("falling", false);
        }

        if (isGrounded)
        {
            playerAnim.SetBool("Jump", false);
            playerAnim.SetBool("OnGround", true);
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            playerAnim.SetBool("Attacking", true);
            attacking = true;
            //playerAnim.speed *= 2;
            StartCoroutine(AttackStop());
        }

        if(Input.GetKeyDown(KeyCode.C) && !crouching)
        {
            crouching = true;
        }
        else if(Input.GetKeyDown(KeyCode.C) && crouching)
        {
            crouching = false;
        }

        if (crouching)
        {
            myCol.offset = new Vector2(myCol.offset.x, -0.1040f);
            myCol.size = new Vector2(myCol.size.x, 0.2125f);
            playerAnim.SetBool("Crouching", true);
        }
        else
        {
            myCol.offset = new Vector2(myCol.offset.x, -0.0471f);
            myCol.size = new Vector2(myCol.size.x, 0.3263f);
            playerAnim.SetBool("Crouching", false);
        }
    }

    IEnumerator AttackStop()
    {
        yield return new WaitForSeconds(0.5f);
        playerAnim.SetBool("Attacking", false);
        attacking = false;
        //playerAnim.speed /= 2;
    }

    private void FixedUpdate()
    {
        if (isGrounded && !attacking)
        {
            //Flip(horizontalInput);
            Vector2 targetVelocity = new Vector2(horizontalInput * speed * Time.fixedDeltaTime, playerRb.velocity.y);
            playerRb.velocity = Vector3.SmoothDamp(playerRb.velocity, targetVelocity, ref refVelocity, smoothTime);
        }
        else
        {
            //Flip(horizontalInput);
            Vector2 targetVelocity = new Vector2(horizontalInput * (speed / 2) * Time.fixedDeltaTime, playerRb.velocity.y);
            playerRb.velocity = Vector3.SmoothDamp(playerRb.velocity, targetVelocity, ref refVelocity, smoothTime);
        }

        if (jump)
        {
            isGrounded = false;
            playerAnim.SetBool("Jump", true);
            playerAnim.SetBool("OnGround", false);
            playerRb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            jump = false;
        }

        

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Ground"))
        {
            //Debug.Log("Collision Occured");
            isGrounded = true;
            isFalling = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("FallDetector"))
        {
            isFalling = true;
        }
    }

    protected void Flip(/*float value*/)
    {
        side = !side;

        Vector3 myScale = transform.localScale;

        myScale.x *= -1;
        transform.localScale = myScale;
        //Debug.Log(side);
        //if (value == 1)
        //{
        //    if (side == false)
        //    {
        //        gameObject.transform.Rotate(faceRight);
        //        side = true;
        //    }
        //}
        //else if (value == -1)
        //{
        //    if (side == true)
        //    {
        //        gameObject.transform.Rotate(faceLeft);
        //        side = false;
        //    }
        //}
    }
}
