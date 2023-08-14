using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rb;
    SpriteRenderer sprite;
    Animator anim;
    BoxCollider2D boxC;
    Vector2 move;
    //float dirX = 0f;


    [SerializeField] LayerMask platformLayerMask;

    //public bool collected;
    MovementState state;


    public float speed = 0;
    public float jumpForce = 0;


    public float sensorLength = 5f;




    private enum MovementState { idle, running, jumping, falling }
    private bool death = false;



    



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        boxC = GetComponent<BoxCollider2D>();
    }

    /*
    public void Sensors()
    {
        RaycastHit2D hit;
        Vector2 sensorStartPos = transform.position;


    }*/


    void Update()
    {

        //dirX = Input.GetAxis("Horizontal");
        //move.y = rb.velocity.y;
        //rb.velocity = new Vector2( dirX * 7f, rb.velocity.y);
        /*
        if (Input.GetButtonDown("Jump") )
        {
            jumpActivate();
        }*/
       // UpdateAnimationState();


    }

    public void jumpActivate()
    {
        if (IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        
        //isJumping = true;
    }


    private bool IsGrounded()
    {
        float extraHeightText = .1f;
        RaycastHit2D  raycasthit = Physics2D.Raycast(boxC.bounds.center, Vector2.down, boxC.bounds.extents.y + extraHeightText, platformLayerMask);
        Color rayColor;
        if (raycasthit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        //Debug.DrawRay(boxC.bounds.center, Vector2.down * (boxC.bounds.extents.y + extraHeightText));
        //Debug.Log(raycasthit.collider);
        return raycasthit.collider != null;
    }

    public Vector2 playerVelocity()
    {
        return rb.velocity;

    }

    public void stopMoving()
    {
        Vector2 moving = new Vector2( 0f, 0f );
        rb.velocity = moving;
        
    }

   

    public void UpdateAnimationState(float dir)
    {
        
        if (dir > 0f)
        {
            state = MovementState.running;
            sprite.flipX = false;
            
        }
        else if (dir < 0f)
        {
            state = MovementState.running;
            sprite.flipX = true;
        }
        else
        {
            state = MovementState.idle;
            //isJumping = false;
        }

        if (rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
            //isJumping = true;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
            //isJumping = true;
        }

        anim.SetInteger("state", (int)state);

    }

    public bool isDead()
    {
        return death;
    }

    public void isNotDead()
    {
        death = false;
    }

    public bool isFalling()
    {
        return state == MovementState.falling;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("tokenItem"))
        {
            //Destroy(collision.gameObject);
            collision.gameObject.transform.position = new Vector3(200f, -200f, 1f);
            
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (state != MovementState.falling)
            {
                //Destroy(collision.gameObject); // move object
                //collision.gameObject.transform.position = new Vector3(200f, -200f, 1f);
                death = true;
            }
            //else
            //{
                //transform.position = new Vector3(-2.84f,0.68f,1f);
                //RestartLevel();
                
                //Debug.Log("Muerto en la vida real");
                //DEAD
            //}
            
        }
    }
}







/*
    void UpdateAnimationState()
   {
       if (dirX > 0f)
       {
           anim.SetBool("running", true);
           sprite.flipX = false;

       }
       else if (dirX < 0f)
       {
           anim.SetBool("running", true);
           sprite.flipX = true;
       }
       else
       {
           anim.SetBool("running", false);
       }

   }*/
