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


    public float speed = 8;
    public float jumpForce = 8;
    //public float sensorLength = 5f;

    private float oldSpeed;
    private float oldJump;


    private enum MovementState { idle, running, jumping, falling }
    private bool death = false;


    // for new rules
    //NewRules Rls;
    //public Rules newRl;
    public bool ruleGenerated = false;





    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        boxC = GetComponent<BoxCollider2D>();
        //Rls = new NewRules("variables here"); // I need to introduce the variables here and made a funtion that will make the changes...

        oldSpeed = speed;
        oldJump = jumpForce;

        
        
    }


    void Update()
    {

        
        if (Input.GetKeyDown(KeyCode.R))
        {
            //generateNewRule();
            Debug.Log("the rule was generated from the begining of the code...");
        }
        

    }



    public void jumpActivate()
    {
        if (IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        
    }


    public bool IsGrounded()
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


    


    // need a old variable that will contain the original value as the rule so each time that we reset the rule will have the same value
    // neeed a DEFAULT CASE WHEN NON RULE IS CREATED 
    public void setNewRule(NewRules Rls, Rules newRl)
    {
        //Debug.Log("setNewRule");
        int usedEffect = Rls.getEffectUsedtoInt(newRl.effect);
        int valueToAdd = int.Parse(newRl.valueEffect);

        if (newRl.name == "speed")
        {
            switch (usedEffect)
            {
                case 0:
                    speed += valueToAdd;
                    break;
                case 1:
                    speed -= valueToAdd;
                    break;
                case 2:
                    speed *= valueToAdd;
                    break;
                case 3:
                    speed /= valueToAdd;
                    break;
                case 4:
                    speed %= valueToAdd;
                    break;
                default:
                    break;

            }

        }
        else if (newRl.name == "jumpForce")
        {
            switch (usedEffect)
            {
                case 0:
                    jumpForce += valueToAdd;
                    break;
                case 1:
                    jumpForce -= valueToAdd;
                    break;
                case 2:
                    jumpForce *= valueToAdd;
                    break;
                case 3:
                    jumpForce /= valueToAdd;
                    break;
                case 4:
                    jumpForce %= valueToAdd;
                    break;
                default:
                    break;

            }


        }
        else if (newRl.name == "position.x")
        {
            rb.transform.position = new Vector2(rb.transform.position.x + (float)valueToAdd, rb.transform.position.y);

        }
        else if (newRl.name == "position.y")
        {
            rb.transform.position = new Vector2(rb.transform.position.x, rb.transform.position.y + (float)valueToAdd);
        }


    }

    /// <summary>
    /// this function is for movement and jump given that those values are only available when the agent press a button...
    /// </summary>
    public void setOldRule(NewRules Rls, Rules newRl)
    {
        //Debug.Log("setOldRule");
        if (newRl.name == "speed")
        {
            speed = oldSpeed;
        }
        else if (newRl.name == "jumpForce")
        {
            jumpForce = oldJump;
        }

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
            
        }

        if (rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
            
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
            
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
            
            collision.gameObject.transform.position = new Vector3(200f, -200f, 1f);
            
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (state != MovementState.falling)
            {
                death = true;
            }
            
            
        }
    }
}




