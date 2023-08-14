using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Platformer.Model;
using Platformer.Mechanics;
using Platformer.Gameplay;
using System.IO;
using Debug = UnityEngine.Debug;
using System;

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



    private Vector3 playTest1;
    private Vector3 playTest2; 

    private Vector2 oldPlayerLocation;
    private Vector2 playerPosition;
    private Vector2 goalPosition;
    private Vector2 toGoal;

    private Vector3 m_v3MoveDirection = Vector3.zero;//Which way the agent should move
    private bool m_bJump = false;//Should the agent jump?
    private float m_fFurthestXPos = 0;//The furthest X position the agent has reached in this iteration
    private Vector2 furthestPos;


    //Metrics

    DateTime start;
    DateTime end; 

    //private Metrics agentMetrics;

    private bool goalReached;
    private int timesGoalReached;
    private int timesUseNewRule;
    private int deaths;
    private int actionsToGoal;
    private int enemiesKilled;
    private int itemsCollected;
    private int leftUsed;
    private int rigthUsed;
    private int jumpUsed;
    private int nonMovement;
    private int insidePosiblePath;
    private double timeToGoal;
    //private List<double> times;
    private int indexTimes;
    private TimeSpan ts;

    public Stopwatch stopwatch;


    //moving rules to agent
    NewRules Rls = new NewRules("player variables here");

    public Rules newRl = new Rules();

    public bool ruleGenerated = true ; // False if we need to use the same rule...?
    public bool neigborsGenerated = false;
    int counterEpisodes = 0;


    
    

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        //agentMetrics = new Metrics();

        playTest1 = new Vector3(0f, 0.68f, 1f);
        playTest2 = new Vector3(28f, -0.80f, 1f);
        //times = new List<double>();
        indexTimes = 0;
       

    }

    private void Awake()
    {
        goalReached = false;

        //Rls = new NewRules("player variables here");
        //newRl = new Rules();

        player = GetComponent<PlayerMovement>();
        tokens = GameObject.FindGameObjectsWithTag("tokenItem");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        oldPlayerLocation = player.transform.position;

        playerPosition = new Vector2(player.transform.position.x, player.transform.position.y);
        goalPosition = new Vector2(targerTransform.position.x, targerTransform.position.y);
        

        toGoal = goalPosition - playerPosition;

        stopwatch = new Stopwatch(); // to measure the time once it get to the location

        //to generate the empty file
        generateEmptyFile();

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

        
        if (!ruleGenerated)
        {
            //generateNewRule();   // this is part of the generation without rules   so no new rules are generated?
        }


    }

   

    public override void OnEpisodeBegin()
    {
        //Debug.Log("---starting Agent---");
        player.stopMoving();
        player.transform.localPosition = playTest2; // POSITION 
        m_v3MoveDirection = Vector3.zero;//Reset velocity vector
        player.isNotDead();
        furthestPos = player.transform.position;
        //fireTime = Time.time;

        // THIS WILL USE ONCE ALL HAS BEEN PROVED JUST TO GET RULES CAPABLE OF REACHING THE GOAL...??

        


        //timer
        

        loadRule();
        calculatePastMetric();


        // timing 
        start = DateTime.Now;
        
        //stopwatch.Start();
        //Debug.Log("starting at ms:");
        //Debug.Log(stopwatch.ElapsedMilliseconds);

        //metrics 
        goalReached = false;
        timesGoalReached = 0;
        timesUseNewRule = 0;
        deaths = 0;
        actionsToGoal = 0;
        enemiesKilled = 0;
        itemsCollected = 0;
        leftUsed = 0 ;
        rigthUsed = 0;
        jumpUsed = 0;
        nonMovement = 0;
        insidePosiblePath = 0;



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

        counterEpisodes += 1;

        
        if (counterEpisodes == 100)//random number just to be sure that all the rules are the same
        {
            getNeighbors();

        }
        

    }

    //Observations of the environment, I don't know if the use of a rule could be an observation??

        /// <summary>
        /// this function is used to collect observations, we figure it out that ussing the observations made by the different sensors of agents
        /// is enought for training on this environment. In case of been requered we can add observations.
        /// </summary>
        /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor)
    {
        //const float rayDistance = 3f;
        //List<string> detectables = new List<string>{ "Enemy", "Goal", "tokenItem" };



        //sensor.AddObservation(playerPosition); //player position 2obs
        //Vector2 vel = player.playerVelocity();
        //sensor.AddObservation(vel.x);// player velocity 2 obs
        //Debug.Log("playerVelocity");
        //Debug.Log(player.playerVelocity());


        //sensor.AddObservation(goalPosition); // goal position  2 obs


        Vector2 toGoal = goalPosition - playerPosition; 
        sensor.AddObservation(toGoal.normalized);//2 observations direction
        //sensor.AddObservation(toGoal);//1 observations distance

        //Debug.Log("direction to goal");
        //Debug.Log(toGoal);

        //Vector2 toEnemy = nearestEnemy() - playerPosition;
        //Vector2 toToken = nearestToken() - playerPosition;
        //sensor.AddObservation(toEnemy.normalized); //2 observations
        //sensor.AddObservation(toToken.normalized); //2 observations

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



        if (toGoal.x > goalPosition.x - furthestPos.x && toGoal.y > goalPosition.y - furthestPos.y)//If the agent has progressed further through the level
        {
            furthestPos = player.transform.position;//Set the furthest poss
            AddReward(0.005f);//Encourage Moving further right
        }


    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        //base.OnActionReceived(actions);
        int Movement = actions.DiscreteActions[0];
        int newButton = actions.DiscreteActions[1];
        m_v3MoveDirection = Vector3.zero;

        //float moveX = actions.ContinuousActions[0];

        //Debug.Log("Entra a onAction Received");

        //this is the button recreation for our new mechanic
        if (newButton == 1 )             //need to test if it makes changes on velocity and jump...
        {
            player.ruleGenerated = true;
            
            player.setNewRule(Rls, newRl);
            timesUseNewRule += 1;
            actionsToGoal += 1;
        }
        else if (newButton == 0 )
        {
            player.ruleGenerated = true;
            
            player.setOldRule(Rls, newRl);
        }

        switch (Movement)
        {
            case 0:
                m_v3MoveDirection = new Vector3(0.0f, 0, 0.0f);
                //player.transform.position += m_v3MoveDirection;
                nonMovement += 1; 
                break;
            case 1:
                m_v3MoveDirection = new Vector3(-1.0f, 0, 0.0f) ; //izquieda
                actionsToGoal += 1;
                leftUsed += 1;
                //player.transform.position += m_v3MoveDirection * Time.deltaTime * player.speed;
                break;
            case 2:
                m_v3MoveDirection = new Vector3(1.0f, 0, 0.0f) ; // derecha
                actionsToGoal += 1;
                rigthUsed += 1; 
                //player.transform.position += m_v3MoveDirection * Time.deltaTime * player.speed;
                break;
            case 3:
                m_bJump = true;
                actionsToGoal += 1;
                jumpUsed += 1;
                //player.jumpActivate();  
                break;     
        }

        

        /*
        if (rb.velocity.magnitude > player.speed)//If the velocity is above the max
        {
            player.transform.velocity = Vector3.ClampMagnitude(rb.velocity, player.speed);//Clamp the velocity to the max velocity
        }*/

        AddReward(-1f /  MaxStep);

        
        

    }

   

    public void FixedUpdate()
    {

        //moveX = Input.GetAxisRaw("Horizontal");
        player.UpdateAnimationState(moveX);

        
        if (m_bJump == true)//If I should Jump
        {
            player.jumpActivate();
            m_bJump = false;//Reset jump flag
        }
        if (m_v3MoveDirection.magnitude > 0)//If the movement vector is not 0
        {
            
            m_v3MoveDirection.x *= player.speed * Time.deltaTime;//Add the speed multiplier
            
            //rb.transform.position += m_v3MoveDirection;
            rb.transform.Translate(m_v3MoveDirection);
            //rb.AddForce(m_v3MoveDirection);//Add that force
            m_v3MoveDirection = Vector3.zero;//Reset the movement vector to zero
        }

        
        if (rb.velocity.magnitude> player.speed)//If the velocity is above the max
        {
            
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, player.speed);//Clamp the velocity to the max velocity
        }

        if (transform.position.x > m_fFurthestXPos)//If the agent has progressed further through the level
        {
            m_fFurthestXPos = transform.position.x;//Set the furthest X pos
            AddReward(0.005f);//Encourage Moving further right
        }

        

    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        moveX = Input.GetAxisRaw("Horizontal");


        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            discreteActions[0] = 1;
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            discreteActions[0] = 2;   
        }
        else if (Input.GetAxisRaw("Horizontal") == 0)
        {
            discreteActions[0] = 0;
        }

        if (Input.GetButtonDown("Jump"))
        {
            discreteActions[0] = 3;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            discreteActions[1] = 1;
        }
        else
        {
            discreteActions[1] = 0;
        }
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
            AddReward(5.0f);
            //get the value if we got to the goal only....
            end = DateTime.Now;
            goalReached = true;
            EndEpisode();
            timesGoalReached += 1;
            UnityEngine.Debug.Log("Goal detected");
            
        }
        if (collision.gameObject.tag == "OutZone")
        {
            //AddReward(-0.6f);
            deaths += 1;
            AddReward(-1.0f);
            EndEpisode();
        }
        if (collision.gameObject.CompareTag("tokenItem"))
        {
            AddReward(0.5f);
            itemsCollected += 1; 
        }
        if (collision.gameObject.CompareTag("PossibleBest"))
        {
            insidePosiblePath += 1;
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            
            if (!player.isDead() && player.isFalling())
            {
                enemiesKilled += 1;
                AddReward(0.5f); 
                collision.gameObject.transform.position = new Vector3(200f, -200f, 1f);
                
            }
            else
            {
                //AddReward(-0.6f);
                deaths += 1;
                AddReward(-1.0f);
                EndEpisode();
                
                
                //player.RestartLevel();
                //EndEpisode();
            }

        }

    }

    private void generateEmptyFile()
    {
        Metrics agentMetrics = new Metrics();
        string file = JsonUtility.ToJson(agentMetrics);
        File.WriteAllText(Application.dataPath + "/saveRuleandMetric.json", file);
    }

    private void calculatePastMetric() // save the file if reach the goal? 
    {


        
        string json = File.ReadAllText(Application.dataPath + "/saveRuleandMetric.json");

        Metrics agentMetrics = JsonUtility.FromJson<Metrics>(json);

        agentMetrics.generatedRule = newRl.allToString;
        agentMetrics.timesGoalReached += timesGoalReached;
        agentMetrics.timesUseNewRule += timesUseNewRule;
        agentMetrics.deaths += deaths;
        agentMetrics.actionsToGoal += actionsToGoal;
        agentMetrics.enemiesKilled += enemiesKilled;
        agentMetrics.itemsCollected += itemsCollected;
        agentMetrics.counter += 1;
        agentMetrics.leftUsed += leftUsed;
        agentMetrics.rigthUsed += rigthUsed;
        agentMetrics.jumpUsed += jumpUsed;
        agentMetrics.nonMovement += nonMovement;
        agentMetrics.insidePosiblePath += insidePosiblePath;
        agentMetrics.timeToGoal = ts.TotalMilliseconds;
        if (goalReached)
        {
            ts = (end - start);
            agentMetrics.times.Add(Math.Round(ts.TotalMilliseconds, 2));
            agentMetrics.rithU.Add(rigthUsed);
            agentMetrics.leftU.Add(leftUsed);
            agentMetrics.jumpU.Add(jumpUsed);
            agentMetrics.enemiesK.Add(enemiesKilled);
            agentMetrics.itemsC.Add(itemsCollected);
            agentMetrics.IsideBest.Add(insidePosiblePath);
            agentMetrics.NewRule.Add(timesUseNewRule);
        }
            



        string file = JsonUtility.ToJson(agentMetrics);
        File.WriteAllText(Application.dataPath + "/saveRuleandMetric.json", file);
        

    }

    [System.Serializable]
    private class Metrics
    {
        public string generatedRule;
  
        public int counter;
        public bool goalReached;
        public int timesGoalReached;
        public int timesUseNewRule;
        public int deaths;
        public int actionsToGoal;
        public int enemiesKilled;
        public int itemsCollected;
        public int leftUsed;
        public int rigthUsed;
        public int jumpUsed;
        public int nonMovement;
        public int insidePosiblePath;
        public double timeToGoal;
        public List<double> times;
        public List<int> NewRule;
        public List<int> leftU;
        public List<int> rithU;
        public List<int> jumpU;
        public List<int> enemiesK;
        public List<int> itemsC;
        public List<int> IsideBest;


    }


    public void generateNewRule()
    {

        UnityEngine.Debug.Log("generating new rule...");

        newRl = Rls.getRandomRule(Rls.varList, 19, 21, 1, 5);
        ruleGenerated = true;
        ruleToJson test = new ruleToJson();
        test.name = newRl.name;
        test.comparator = newRl.comparator;
        test.valueComparator = newRl.valueComparator;
        test.effect = newRl.effect;
        test.valueEffect = newRl.valueEffect;

        string f1 = JsonUtility.ToJson(test);
        File.WriteAllText(Application.dataPath + "/ruleGen.json", f1);

        newRl.printValues();
    }

    [System.Serializable]
    private class ruleToJson
    {
        public new string name;
        public string comparator;
        public string valueComparator;
        public string effect;
        public string valueEffect;
        

    }


    public void loadRule()
    {

        string json = File.ReadAllText(Application.dataPath + "/ruleGen.json");
        ruleToJson x = JsonUtility.FromJson<ruleToJson>(json);

        newRl.setValues(x.name, x.comparator, x.valueComparator, x.effect, x.valueEffect);



    }

    public void getNeighbors()
    {
        List<Rules> neig = new List<Rules>();

        neig = Rls.getNeighbors(newRl);
        ruleToJson empty = new ruleToJson();
        string empty1 = JsonUtility.ToJson(empty);
        File.WriteAllText(Application.dataPath + "/ruleNeig.json", empty1);

        foreach (Rules rule in neig)
        {
            ruleToJson test = new ruleToJson();
            test.name = rule.name;
            test.comparator = rule.comparator;
            test.valueComparator = rule.valueComparator;
            test.effect = rule.effect;
            test.valueEffect = rule.valueEffect;

            string f1 = JsonUtility.ToJson(test);
            File.AppendAllText(Application.dataPath + "/ruleNeig.json", f1);
            

        }


    }




}
