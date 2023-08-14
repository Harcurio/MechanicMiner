using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Platformer.Model;
using Platformer.Mechanics;
using Platformer.Gameplay;

public class MoveToGoal : Agent
{

    [SerializeField] private Transform targerTransform;
    bool isJumpDown;
    float moveX = 0f;
    private PlayerMovement player;
    private GameObject[] tokens;
    private GameObject[] enemies;

    private Rigidbody2D rb;
    //private RayPerceptionSensorComponent2D leftRay;
    //private RayPerceptionSensorComponent2D rightRay;

    private List<float> locationItemX = new List<float>();
    private List<float> locationItemY = new List<float>();

    private List<float> locationEX = new List<float>();
    private List<float> locationEY = new List<float>();
    //private float[] ItemslocationsY { };


    //for time 
    private float fireTime;
    private float badReward = 0f;

    private Vector2 oldPlayerLocation;
    private Vector2 playerPosition;
    private Vector2 goalPosition;
    private Vector2 toGoal;


    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        //leftRay = GetComponent<RayPerceptionSensorComponent2D>();
        //rightRay = GetComponent<RayPerceptionSensorComponent2D>();
    }

    // function to updateNearestEnemy

    

    private void Awake()
    {
        
        player = GetComponent<PlayerMovement>();
        tokens = GameObject.FindGameObjectsWithTag("tokenItem");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        oldPlayerLocation = player.transform.position;

        playerPosition = new Vector2(player.transform.position.x, player.transform.position.y);
        goalPosition = new Vector2(targerTransform.position.x, targerTransform.position.y);

        toGoal = goalPosition - playerPosition;

        foreach (GameObject t in tokens)
        {
            locationItemX.Add( t.transform.localPosition.x);
            locationItemY.Add(t.transform.localPosition.y);
        }

        foreach (GameObject x in enemies)
        {
            locationEX.Add(x.transform.localPosition.x);
            locationEY.Add(x.transform.localPosition.y);
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJumpDown = true;
        }

        moveX = Input.GetAxisRaw("Horizontal");
        player.UpdateAnimationState(moveX);
       
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("---starting Agent---");
        player.stopMoving();
        player.transform.localPosition = new Vector3(0f, 0.68f, 1f);
        player.isNotDead();
        //fireTime = Time.time;
        badReward = 0f;



        int counter = 0;
        foreach (GameObject t in tokens)
        {
            t.transform.localPosition = new Vector3(locationItemX[counter], locationItemY[counter],14);
            counter++;
          
        }
        counter = 0;

        foreach (GameObject t in enemies)
        {
            t.transform.localPosition = new Vector3(locationEX[counter], locationEY[counter], 0);
            counter++;

        }

        //Debug.Log("Inicio el episodio");


    }

    //Observations of the environment
    public override void CollectObservations(VectorSensor sensor)
    {
        //const float rayDistance = 3f;
        //List<string> detectables = new List<string>{ "Enemy", "Goal", "tokenItem" };

        

        sensor.AddObservation(playerPosition); //player position 2obs
        Vector2 vel = player.playerVelocity();
        //sensor.AddObservation(vel.x);// player velocity 2 obs
        //Debug.Log("playerVelocity");
        //Debug.Log(player.playerVelocity());

        
        sensor.AddObservation(goalPosition); // goal position  2 obs
        

        Vector2 toGoal = goalPosition - playerPosition; 
        sensor.AddObservation(toGoal.normalized);//2 observations direction
        sensor.AddObservation(toGoal.x);//1 observations distance

        //Debug.Log("direction to goal");
        //Debug.Log(toGoal);

        Vector2 toEnemy = nearestEnemy() - playerPosition;
        Vector2 toToken = nearestToken() - playerPosition;
        sensor.AddObservation(toEnemy.normalized); //2 observations
        sensor.AddObservation(toToken.normalized); //2 observations

        //Debug.Log("direction to token");
        //Debug.Log(toToken.normalized);

        //AddReward(-0.001f);

        /* to check this code
         if (_justCalledDone)
        {
            _justCalledDone = false;
        }
        else
        {
            AddReward(Random.Range(-1f, 1f));
            Done();
            _justCalledDone = true;
        }
         
        Vector2 actualPosition = player.transform.position;
        if (oldPlayerLocation.x < actualPosition.x)
        {
            AddReward(-0.001f);

        }
        else
        {
            oldPlayerLocation = actualPosition;
        }
        */


    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        //base.OnActionReceived(actions);
        //int moveL = actions.DiscreteActions[0];
        //int moveR = actions.DiscreteActions[1];
        float moveX = actions.ContinuousActions[0];

        if (moveX < 0)
        {
           
            //discreteActions[0] = 1;
            player.transform.position += new Vector3(moveX , 0, 0) * Time.deltaTime * player.speed;
            //moveX = -1f;
        }
        else if (moveX > 0)
        {
            //discreteActions[1] = 1;
            player.transform.position += new Vector3(moveX, 0, 0) * Time.deltaTime * player.speed;
            //moveX = 1f;
        }


        //player.transform.position += new Vector3(moveX, 0, 0) * Time.deltaTime * player.speed;
        
        if (actions.ContinuousActions[1] > 0)
        {
            player.jumpActivate();
            //badReward = badReward - 0.001f;
            //AddReward(-0.001f);//just trying to jump less
        }

        //Debug.Log(actions.ContinuousActions[0]);

        badReward = badReward - 0.001f;
        //Debug.Log(badReward);

        if (badReward <= -1.0f)
        {
            Debug.Log("dead by time...");
            SetReward(-0.6f);
            EndEpisode();
        }
       

    }

    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        //continousActions[0] = Input.GetAxisRaw("Horizontal");


        //ActionSegment<int> discreteActions = actionsOut.DiscreteActions;


        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            continousActions[0] = -1;
            
            //player.transform.position += new Vector3((discreteActions[0] *-1), 0, 0) * Time.deltaTime * player.speed;
        }
        else if(Input.GetAxisRaw("Horizontal") > 0)
        {
            continousActions[0] = 1;
            
            //player.transform.position += new Vector3(discreteActions[1], 0, 0) * Time.deltaTime * player.speed;
        }

        continousActions[1] = isJumpDown ? 1 : 0;
        isJumpDown = false;

    }

    private Vector2 nearestEnemy()
    {
        Transform tMin = null;
        Vector2 min;
        float minDist = Mathf.Infinity;
        Vector2 currentPos = transform.position;
        foreach (GameObject t in enemies)
        {
            float dist = Vector2.Distance(t.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = t.transform;
                minDist = dist;
            }
        }

        min.x = tMin.position.x;
        min.y = tMin.position.y;
        return min;
    }

    private Vector2 nearestToken()
    {
        Transform tMin = null;
        Vector2 min;
        float minDist = Mathf.Infinity;
        Vector2 currentPos = transform.position;
        foreach (GameObject t in tokens)
        {
            float dist = Vector2.Distance(t.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = t.transform;
                minDist = dist;
            }
        }
        min.x = tMin.position.x;
        min.y = tMin.position.y;
        return min;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            AddReward(+0.5f);
            EndEpisode();
            Debug.Log("Goal detected");
            Debug.Log(badReward);
        }
        if (collision.gameObject.tag == "OutZone")
        {
            AddReward(-0.6f);
            EndEpisode();
        }
        if (collision.gameObject.CompareTag("tokenItem"))
        {
            AddReward(+0.010f);
            //Debug.Log("agarro token");
            //collision.gameObject.transform.position = new Vector3(2.85f,-12.5f,1f);
            //player.collected = true;
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            
            if (!player.isDead() && player.isFalling())
            {
                AddReward(+0.150f);
                //Debug.Log("mato enemigo");
                collision.gameObject.transform.position = new Vector3(200f, -200f, 1f);
                //Destroy(collision.gameObject);
            }
            else
            {
                AddReward(-0.6f);
                EndEpisode();
                
                
                //player.RestartLevel();
                //EndEpisode();
            }

        }

    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        //SetReward(1f);
        //EndEpisode();

        //

        // the way of setting adding and rest 
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(+1f);
            EndEpisode();
        }
        if (other.TryGetComponent<DeathZone>(out DeathZone death))
        {
            SetReward(-1f);
            EndEpisode();
        }

    }*/




}
