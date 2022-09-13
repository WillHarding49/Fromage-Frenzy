using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
* Type: Class
* 
* Name: CheeseMovement
*
* Author: Will Harding
*
* Description: Moves cheese independently of camera movement between lanes
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 22/06/2021    WH          1.00        -Initial Created, cheese can move between "lanes"
* 28/06/2021    WH          1.01        -Added up and down movement and limits for all directions
* 04/07/2021    WH          1.10        -Cheese moves following path rather than using triggers
* 06/07/2021    WH          1.11        -Cheese now moves properly across path and using curves
* 07/07/2021    WH          1.12        -Inherited from PathParent to clean up stuff
* 12/07/2021    WH          1.13        -Added lap stuff and started lane loops
* 13/07/2021    WH          1.14        -Using distance of curve to calculate speed and make it move
*                                        at a consistent rate
* 14/07/2021    WH          1.15        -Added lane switching on starights, needs improvement
* 16/07/2021    WH          1.16        -Cleaned update by conbining if statements
* 16/07/2021    JG          1.16        -added functions for pick ups from an older versison of cheesemovement
* 19/07/2021    WH          1.17        -Added shortcut movement function to limit lanes to switch
* 20/07/2021    WH          1.18        -Fixed camera movement and added correct naming conventions
* 21/07/2021    JG          1.19        -Moved Will input code into functions so the playerInput child can use them
* 21/07/2021    WH          1.20        -Added function to return distance to next node for positioning
* 22/07/2021    WH          1.21        -Fixed lane distance bug
* 23/07/2021    WH          1.22        -Fixed 1st curve movement bug and added sound
* 24/07/2021    WH          1.23        -Added defualt max speed for rubberbanding and speed change
* 27/07/2021    JG          1.24        - removed every static var and function from class
* 28/07/2021    WH          1.25        -Path is now got from tag
* 30/07/2021    WH          1.26        -Fixed camrea snapping thing and added comments to stuff
* 01/08/2021    WH          1.27        -Added respawn function to respawn cheese
* 02/08/2021    JG          1.28       - reset accel after hazzard 
* 03/08/2021    WH          1.29        -Added race manager stuff to start game loop state stuff
* 04/08/2021    JG          1.30        -added Game manager tag for all scripts that need to be in the scene but don't have a go
* 09/08/2021    JG          1.31        -added a spin counter for each racer for the spinout/track hazzard. Removed melted cheese speed control code.
* 14/08/2021    JG          1.32        -added an animation bool so racer move on start
* 15/08/2021    JG          1.33       -added a mutiplyer to lower animation speed
* 17/08/2021    JG          1.34        -removed spinamount var for hazzard as hazzard has been reformated
* 18/08/2021    WH          1.35        -Added tooltips
* 19/08/2021    Wh          1.36        -Moved speed and movement out of update and into their own
*                                        functions to clean things
****************************************************************************************************/
public class CheeseMovement : PathParent
{
    private float m_speed;

    [HideInInspector]
    public float m_maxSpeed;

    [Tooltip("Default max speed the cheese can move at")]
    public float m_defualtMaxSpeed = 5f;

    [SerializeField]
    [Tooltip("Speed the cheese rotates around corners")]
    private float m_rotationSpeed = 3f;

    [SerializeField]
    [Tooltip("Multipler for animation speed")]
    private float m_animationMutiplyer = 0.2f;

    [SerializeField]
    [Tooltip("Multipler for animation speed")]
    private float m_acceleration;

    private float m_lastAccelaration;

    [SerializeField]
    [Tooltip("Default Acceleration of the cheese")]
    private float m_defaultAcceleration = 3f;

    [HideInInspector]
    public string m_activePickUp;

    private GameObject m_pathObject;

    private bool m_loop;

    private Transform m_target;

    [HideInInspector]
    public int m_lap = 1;

    [HideInInspector]
    public int m_node = 0;

    [Tooltip("Lane in which the cheese is in. Set this initally to set it's start lane and move the cheese inline with said lane for cleanest look.")]
    public int m_lane = 0;
    
    private float m_curveMove = 1f;

    private float m_laneDist;

    private int m_laneMin = 0;
    private Vector3 m_targetPosition;

    private float m_curveLength;
    private bool m_onCurve = false;

    private AudioSource m_audioSource;

    protected GameObject m_raceManager;
    protected bool m_finished = false;
    private Animator m_cheeseAnimation;
    /**************************************************************************************
    * Type: Function
    * 
    * Name: Start
    *
    * Author: Will Harding
    *
    * Description: Sets variables at start and calculates lane distance
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 22/06/2021    WH          1.00        -Initial Created 
    * 04/07/2021    WH          1.01        -Makes path and then sets target
    * 07/07/2021    WH          1.10        -Added inherietence to clean stuff up
    * 12/07/2021    WH          1.11        -Added loop bool
    * 14/0702021    WH          1.12        -Added lane distance calculation
    * 22/07/2021    WH          1.13        -Fixed lane distance out of bounds bug
    * 23/07/2021    WH          1.14        -Added audio stuff
    * 24/07/2021    WH          1.15        -Added max speed set to defualt
    * 28/07/2021    WH          1.16        -Path is got from tag
    * 04/08/2021    Wh          1.17        -Set audio to play when race starts
    * 04/08/2021    JG          1.18        -updated racer tag to game Manager tag
    * 14/08/2021    JG          1.19        -added animator 
    **************************************************************************************/
    protected void Start()
    {
        m_pathObject = GameObject.FindWithTag("Path");

        //Get path nodes list
        MakeList(m_pathObject.transform);

        //Gets next node to target
        m_target = m_nodes[m_node][m_lane];

        //Gets if the path is a loop or not
        m_loop = m_pathObject.GetComponent<PathDraw>().m_loop;

        //Distance between each lane
        m_laneDist = Vector3.Distance(m_nodes[m_node][0].position, m_nodes[m_node][1].position);

        //Plays cheese racing sound
        m_audioSource = GetComponent<AudioSource>();
        m_audioSource.loop = true;
        m_audioSource.Stop();


        m_maxSpeed = m_defualtMaxSpeed;

        m_raceManager = GameObject.FindWithTag("GameManager");
        m_cheeseAnimation = transform.Find("Model").Find("Animation").GetComponent<Animator>();
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: RotateCamera
    * Parameters: Vector3 p_target
    *
    * Author: Will Harding
    *
    * Description: Rotates camera to keep looking staright with the cheese
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 07/07/2021    WH          1.00        -Initial Created, moves camera
    * 30/07/2021    WH          1.10        -Stopped relative pos being zero and causing 
    *                                        snapping issues and debug error
    **************************************************************************************/
    void RotateCamera(Vector3 p_target)
    {
        Vector3 relativePos = p_target - transform.position;
        
        if (relativePos != Vector3.zero)
        {
            Quaternion rotationToTarget = Quaternion.LookRotation(relativePos);

            //Rotates towards target over time so it looks smoother than snapping
            transform.rotation = Quaternion.Slerp(transform.rotation, rotationToTarget, m_rotationSpeed * Time.deltaTime);
        }
    }


    /**************************************************************************************
    * Type: Function
    * 
    * Name: CurveMove
    * Parameters: Vector3 p_point0, p_point1, p_point2, p_point3
    *
    * Author: Will Harding
    *
    * Description: Moves cheese across curve in path
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 07/07/2021    WH          1.00        -Initial Created, moves cheese
    * 13/07/2021    WH          1.10        -Added better curve speed using distance
    * 20/07/2021    WH          1.11        -Fixed camera move look target
    * 21/07/2021    WH          1.12        -Made Curvelength a member variable
    **************************************************************************************/
    void CurveMove(Vector3 p_point0, Vector3 p_point1, Vector3 p_point2, Vector3 p_point3)
    {
        //Chord of curve (distance between start and end points)
        float chord = Vector3.Distance(p_point0, p_point3);

        //Sum of distances of control net sides (distance between point 0 and 1, 1 and 2, and 2 and 3)
        float controlNetSides = Vector3.Distance(p_point0, p_point1) + Vector3.Distance(p_point1, p_point2) + Vector3.Distance(p_point2, p_point3);

        //Curve length is average of it's chord length + the sum of the lengths of the control sides
        m_curveLength = (controlNetSides + chord) / 2;

        //Speed divided by the length of the curve gives the speed it needs to move each frame 
        m_curveMove += (m_speed * Time.deltaTime) / m_curveLength;

        //curveMove must be between 0 and 1
        if (m_curveMove >= 1f)
        {
            m_curveMove = 1f;
            //Debug.Log("Curve stop");
        }

        //Moves cheese across curve
        transform.position = CubicBezier(p_point0, p_point1, p_point2, p_point3, m_curveMove);

        //Gets derivite of curve at current position to get direction to rotate camrea
        Vector3 curveLookPos = CubicBezierDer1(p_point0, p_point1, p_point2, p_point3, m_curveMove) + transform.position;
        RotateCamera(curveLookPos);
    }


    /**************************************************************************************
    * Type: Function
    * 
    * Name: Lap
    *
    * Author: Will Harding
    *
    * Description: Function to do lap stuff
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 07/07/2021    WH          1.00        -Initial Created
    **************************************************************************************/
    void Lap()
    {
        m_lap++;

        if(m_lap > m_raceManager.GetComponent<RaceManager>().m_lapAmount)
        {
            //m_raceManager.GetComponent<RaceManager>().m_finish = true;
            //m_raceManager.GetComponent<RaceManager>().m_racing = false;
            m_raceManager.GetComponent<RaceManager>().RaceFinish();
            m_finished = true;
        }
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: ChangeAcclerationSpeed
    *
    * Author: Joe Gilmore
    *
    * Description: called from hazzard to create a spinout effect
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 16/06/2021    JG          1.00        -Initial Created, added from older version of cheese movement
    * 27/07/2021    JG          1.01        -removed static 
    * 30/07/2021    WH          1.02        -Fixed names and added naming conventions to 
    *                                        parameters
    **************************************************************************************/

    public void ChangeAcclerationSpeed(float p_deccleration, float p_changeSpeed)
    {
        m_acceleration = p_deccleration;
        m_speed = p_changeSpeed;
    }


    /**************************************************************************************
    * Type: Function
    * 
    * Name: ChangeAccleration
    *
    * Author: Joe Gilmore
    *
    * Description: called from health to change acceleration dependning on cheese size 
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 16/06/2021    JG          1.00        -Initial Created, added from older version of cheese movement
    * 27/07/2021    JG          1.01         removed static 
    * 30/07/2021    WH          1.02        -Fixed names and added naming conventions to 
    *                                        parameters
    *01/08/2021    JG          1.03         - changed to a multiplyer instead of just setting value 
    **************************************************************************************/

    public void ChangeAcceleration(float p_accelerationMultiplier)
    {
        m_acceleration = m_defaultAcceleration * p_accelerationMultiplier;
    }


    /**************************************************************************************
    * Type: Function
    * 
    * Name: MeltedCheeseHit
    *
    * Author: Joe Gilmore
    *
    * Description: called from melted cheese prefab to slow racer down
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 16/06/2021    JG          1.00        -Initial Created, added from older version of cheese movement
    * 27/07/2021    JG          1.01         removed static 
    * 09/08/2021    JG          1.02        -removed melted cheese bool
    **************************************************************************************/
    public void MeltedCheeseHit()
    {
        //will ned to pass active  racer when Ai is on 
        //lowers speed
        m_speed = m_speed / 1.5f;
        //saves last accelration
        m_lastAccelaration = m_acceleration;
        m_acceleration = -0.8f; //decreases acell to slow player
        
    }


    /**************************************************************************************
    * Type: Function
    * 
    * Name: MeltedCheeseExit
    *
    * Author: Joe Gilmore
    *
    * Description: called from melted cheese prefab to reset values after exit melted cheese 
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 16/06/2021    JG          1.00        -Initial Created, added from older version of cheese movement
    * 27/07/2021    JG          1.01        -removed static 
    * 09/08/2021    JG          1.02        -removed melted cheese bool 
    * 19/08/2021    JG          1.03        -Added health update to potentially fix the slow cheese bug   
    **************************************************************************************/
    public void MeltedCheeseExit()
    {
        //reset racer after it exits cheese
        m_acceleration = m_lastAccelaration;
        GameObject.FindGameObjectWithTag("GameManager").GetComponentInParent<HealthSystem>().UpdateRacersHealth(transform.Find("Model").tag, 0);
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: ChangeLaneNo
    * Parameters: int p_newLaneAmount, p_newLaneMin
    *
    * Author: Will Harding
    *
    * Description: Changes what lanes the cheese can switch between
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 19/07/2021    WH          1.00        -Initial Created
    **************************************************************************************/
    public void ChangeLaneNo(int p_newLaneAmount, int p_newLaneMin)
    {
        m_laneAmount = p_newLaneAmount;
        m_laneMin = p_newLaneMin;
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: GetDistToNode
    * Return: float distance to node
    *
    * Author: Will Harding
    *
    * Description: Returns distance to next node for position tracker class
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 22/07/2021    WH          1.00        -Initial Created
    * 23/07/2021    WH          1.01        -Made curve distance positive
    * 30/07/2021    WH          1.02        -Changed positive to use mathf.abs and added
    *                                        comments
    **************************************************************************************/
    public float GetDistToNode()
    {
        float dist;

        if (m_onCurve)
        {
            //Positive value of how much distance of the curve is left to cross
            dist = Mathf.Abs(m_curveLength - (m_curveLength / m_curveMove));
        }
        else if (m_target != null)
        {
            dist = Vector3.Distance(transform.position, m_target.position);
        }
        else
        {
            dist = 0f;
        }

        return dist;
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: ChangeMaxSpeed
    * Parameters: float p_multiplier
    *
    * Author: Will Harding
    *
    * Description: Changes the max speed to be a the defult max speed * a multiplier
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 22/07/2021    WH          1.00        -Initial Created
    **************************************************************************************/
    public void ChangeMaxSpeed(float p_multiplier)
    {
        m_maxSpeed = m_defualtMaxSpeed * p_multiplier;
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: Respawn
    *
    * Author: Will Harding
    *
    * Description: Respawns cheese at previous node. Called in health system when dead
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 01/08/2021    WH          1.00        -Initial Created
    * 03/08/2021    WH          1.01        -Fixed so now goes to correct node
    * 18/08/2021    WH          1.02        -Fixed out of bound error
    **************************************************************************************/
    public void Respawn()
    {
        if (m_node == 0)
        {
            m_node = 1;
        }

        transform.position = m_nodes[m_node - 1][m_lane].position;
    }


    /**************************************************************************************
    * Type: Function
    * 
    * Name: Update
    *
    * Author: Will Harding
    *
    * Description: Moves cheese across the path
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 22/06/2021    WH          1.00        -Initial Created, cheese can move between
    *                                        "lanes"
    * 28/06/2021    WH          1.01        -Added up and down movement and limits for
    *                                        all directions
    * 04/07/2021    WH          1.10        -Cheese moves followig path rather than 
    *                                        using triggers
    * 06/07/2021    WH          1.11        -Cheese now moves properly across path and
    *                                        using curves
    * 07/07/2021    WH          1.12        -Inherited from PathParent and added functions
    *                                        to clean loop
    * 12/07/2021    WH          1.13        -Added basic lap stuff to improve later
    *                                        Started lane stuff
    * 14/07/2021    WH          1.14        -Added lane switching for straights
    * 16/07/2021    WH          1.15        -Cleaned up loops by combining the move and
    *                                        the change target if statements into one.
    *                                        
    * 19/07/2021    WH          1.16        -Changed lane swithing to accomodate shortcuts
    * 23/07/2021    WH          1.17        -Made curvemove be 0 after straights to fix bug
    * 28/07/2021    WH          1.18        -Added deceleration
    * 02/08/2021    JG          1.19        -reset accel after hazzard 
    * 03/08/2021    WH          1.20        -Added bool so cheese only moves when race starts
    * 04/08/2021    WH          1.21        -Sound now plays when race starts
    * 04/08/2021    JG          1.22        -Updated tag to Game manager 
    * 08/08/2021    JG          1.23        -removed set accel to 0 if melted cheese hit
    * 14/08/2021    JG          1.24        -added animation for start and speed 
    * 15/08/2021    JG          1.25        -added animation speed mutiplyer
    * 19/08/2021    WH          1.26        -Seperated the speed and movement into functions
    *                                        to clean things up
    **************************************************************************************/
    protected void Update()
    {
        //If the race has started
        if (m_raceManager.GetComponent<RaceManager>().m_racing)
        {
            m_cheeseAnimation.SetBool("isRacing",true);
            m_cheeseAnimation.speed = m_speed * m_animationMutiplyer;

            if (!m_audioSource.isPlaying)
            {
                m_audioSource.Play();
            }


            SpeedChange();

            TrackMove();
        }
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: SpeedChange
    *
    * Author: Will Harding
    *
    * Description: Changes the speed of the cheese
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 19/08/2021    JG          1.00        -Initial Created, made a function from Update
    **************************************************************************************/
    public void SpeedChange()
    {
        if (m_speed < m_maxSpeed)
        {
            m_speed += m_acceleration * Time.deltaTime;
        }

        if (m_speed > m_maxSpeed)
        {
            m_speed -= m_acceleration * Time.deltaTime;
        }

        //if speed hits 0 restart acceleration
        if (m_speed <= 0)
        {
            m_speed = 2f;
            //check size of racer to reset accel(I call Update racer health instead of check size because i cant pass through everything for checksize).
            GameObject.FindGameObjectWithTag("GameManager").GetComponentInParent<HealthSystem>().UpdateRacersHealth(transform.Find("Model").tag, 0);
        }
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: TrackMove
    *
    * Author: Will Harding
    *
    * Description: Moves cheese across the track
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 19/08/2021    JG          1.00        -Initial Created, made a function from Update
    **************************************************************************************/
    public void TrackMove()
    {
        m_targetPosition = transform.position;

        //If the node 2 nodes ahead is the end of a curve
        if (m_node + 2 < m_nodes.Length && m_nodes[m_node + 2][m_lane].parent.CompareTag("CurveEnd"))
        {
            m_onCurve = true;
            CurveMove(m_nodes[m_node - 1][m_lane].position, m_nodes[m_node][m_lane].position, m_nodes[m_node + 1][m_lane].position, m_nodes[m_node + 2][m_lane].position);

            //If you reach the node
            if (Vector3.Distance(transform.position, m_nodes[m_node + 2][m_lane].position) < 0.01f)
            {
                m_node += 2;
                if (m_node > m_nodes.Length - 1)
                {
                    m_node = 0;
                }
                m_curveMove = 0f;
                m_onCurve = false;
                m_target = m_nodes[m_node][m_lane];
            }
        }

        //If the node 3 nodes ahead is the start node and it is a curve end
        else if (m_node == m_nodes.Length - 3 && m_nodes[0][m_lane].parent.CompareTag("CurveEnd") && m_loop == true)
        {
            m_onCurve = true;
            CurveMove(m_nodes[m_node][m_lane].position, m_nodes[m_node + 1][m_lane].position, m_nodes[m_node + 2][m_lane].position, m_nodes[0][m_lane].position);

            if (Vector3.Distance(transform.position, m_nodes[0][m_lane].position) < 0.01f)
            {
                m_node = 0;
                m_curveMove = 0f;
                m_onCurve = false;
                m_target = m_nodes[m_node][m_lane];
                Debug.Log("Curve node end add");
            }
        }

        //If not a curve
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, m_target.position, m_speed * Time.deltaTime);

            RotateCamera(m_target.position);

            if (Vector3.Distance(transform.position, m_target.position) < 0.01f)
            {
                m_node++;
                if (m_node > m_nodes.Length - 1 && m_loop == true)
                {
                    m_node = 0;
                    Lap();
                }
                else if (m_node > m_nodes.Length - 1 && m_loop == false)
                {
                    m_node--;
                }
                m_target = m_nodes[m_node][m_lane];
                m_curveMove = 0f;
            }
        }
    }


    /**************************************************************************************
    * Type: Function
    * 
    * Name: MoveLeft
    *
    * Author: Joe Gilmore
    *
    * Description: Moves Cheese left
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 20/07/2021    JG          1.00        -Initial Created, adapted Will's code from 
    *                                        update into a function
    * 21/07/2021    WH          1.01        -Changed check to account for shortcuts
    * 28/07/2021    WH          1.02        -Removed the foward movement
    **************************************************************************************/
    protected void MoveLeft()
    {
        if (m_lane > m_laneMin)
        {
            m_lane--;

            //m_targetPosition += transform.forward * (speed / 4);

            m_targetPosition -= transform.right * m_laneDist;
            m_target = m_nodes[m_node][m_lane];

            transform.position = m_targetPosition;
        }
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: MoveRight
    *
    * Author: Joe Gilmore
    *
    * Description: Moves Cheese right
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 20/07/2021    JG          1.00        -Initial Created, adapted Will's code from 
    *                                        update into a function
    * 21/07/2021    WH          1.01        -Changed check to account for shortcuts
    * 28/07/2021    WH          1.02        -Removed the foward movement
    **************************************************************************************/
    protected void MoveRight()
    {
        if (m_lane < m_laneMin + m_laneAmount - 1)
        {
            m_lane++;

            //m_targetPosition += transform.forward * (speed / 4);

            m_targetPosition += transform.right * m_laneDist;
            m_target = m_nodes[m_node][m_lane];

            transform.position = m_targetPosition;
        }
    }
}
