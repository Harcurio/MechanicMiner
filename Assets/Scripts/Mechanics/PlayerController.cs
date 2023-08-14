using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 7;
        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        /*internal new*/ public Collider2D collider2d;
        /*internal new*/ public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public Bounds Bounds => collider2d.bounds;

        //variables for randomBot
        public bool lft = false;
        public bool rgt = false;
        public bool spc = false;

        // New Rules
        public NewRules RulesGenerator;

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();


            RulesGenerator = new NewRules("Give me New Rules pls");

            Rules generatedRule;
            generatedRule = RulesGenerator.getRandomRule(RulesGenerator.varList, 650, 651, 3, 9); // here need to catch the new rules ( don't remember the values though 

            if (generatedRule.used) // HERE TO SEE WHAT RULE WAS MADE...
            {
                //Debug.Log(generatedRule.name);
                //Debug.Log(generatedRule.comparator);
                //Debug.Log(generatedRule.valueComparator);
                //Debug.Log(generatedRule.effect);
                //Debug.Log(generatedRule.valueEffect);
            }


        }

        protected override void Update()
        {
            //initializing random bot variables
            lft = randomBot();
            rgt = randomBot();
            spc = randomBot();

            if (controlEnabled)
            {
                // bot random just to know how to move the character
                /*
                if (lft)
                {
                    move.x = -1;
                }
                else if (lft == false)
                {
                    move.x = 0;
                }

                if (rgt)
                {
                    move.x = 1;
                }
                else if(rgt == false)
                {
                    move.x = 0;
                }

                if (spc)
                {
                    jumpState = JumpState.PrepareToJump;
                }
                else if (spc == false)
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }*/

                /*
                move.x = Input.GetAxis("Horizontal");
                if (jumpState == JumpState.Grounded && Input.GetButtonDown("Jump"))
                    jumpState = JumpState.PrepareToJump;
                else if (Input.GetButtonUp("Jump"))
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }*/
            }
            UpdateJumpState();
            base.Update();
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }
        
        protected override void ComputeVelocity()
        {
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }

        public bool randomBot()
        {

            int randomNumber = Random.Range(0, 10);

            if (randomNumber < 5)
            {
                return true;
            }

            return false;
        }




        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }
}