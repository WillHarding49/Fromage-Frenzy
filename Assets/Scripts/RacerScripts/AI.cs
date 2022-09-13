using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/**************************************************************************************
* Type: (Class)
* 
* Name: AI.cs
* 
* 
*
* Author: Joseph Gilmore
*
* Description: child to cheese movement. controls AI Behaviour 
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 19/07/2021    JG        1.00          -Initial Created
* 20/07/2021    JG        1.01          -Added a cone of raycasts and lane checking.
* 21/07/2021    JG        1.10          -changed parent to updated version.
* 22/07/2021    JG        1.11          -remove tag checker and add move lane for player 
* 26/07/2021    JG        1.12          -added bools in raycast loop to enusre it exits at the correct point.(if a single ray is hit or if movement started). Also added to check if think time has been asinged 
* 27/07/2021    JG        1.13          -add a parameter UseThinkTime
* 28/07/2021    JG        1.14          -each AI behaviour now has a prefered pick up to go towards. 
* 30/07/2021    JG        1.15          -moved bools from each switch case to in usr think time  to remove repeat code . Removed setting behaviour manualy. now does it randomly in ThinkTimeCalulator.
* 04/08/2021    JG        1.16          -added Game manager tag for all scripts that need to be in the scene but don't have a go
* 10/08/2021    JG        1.17          -changed refrence to pickup scritps as they ar renamed and childs of parent pick up
* 12/08/2021    WH        1.18          -Added check for AI behavior to only work when racing, so it should stop AI from teleporting to 000 before the race starts
* **************************************************************************************/
/* Cheese Behaviour 
     *  Each cheese an detect items in their own lane and one lane to each side. This is to work towards their behaviour goals
     * 1. Big Cheese: Want to be able to detect health related pick ups and cheese in the enviroment and actitvly go towards it. They also want to avoid hazzards more and won't go out its way for harmful pick ups
     * 2.Small Cheese: wants to be small and fast. More likely to hit hazzards as it wants to loose health.  if it picks up a health pick up either throw behind or throw in front and change lane. will try avoid enivorment cheese
     * 3. PickUpCheese: This cheeese seaks out aggresive pick ups and does not care for it size. */
public class AI : CheeseMovement
{
    #region Vars
    private int m_rayConeSize = 20; //amount of rays within the cone of vision 
    private float m_rayAngle = 90; // the angle of rays to be spread along
    private float m_thinkTime;//curently set to 2 as a temp until think time calculator works 
    private float m_rayRange = 10f;
    private RaycastHit m_rayHit;
    private List<string> m_trackTargets = new List<string>() { "Player", "AIOne", "AITwo", "AIThree", "AIFour", "AIFive", "Hazzard","ThrowCheese","SpearThrow","MeltedCheese" ,"CheeseItem"}; //list of potetinal things the AI need to detect 
    private int m_laneCheck; //used to see what lane the target is in 
    private bool m_moveRight = true; //conditions to move AI
    private bool m_moveLeft = true;
    private bool m_thinkTimeSet = false;
    private string m_behaviour;
    private GameObject m_thinkTimeGO; //empty game object to refer to the hink tiem
    private bool m_rayHitTarget = false;
    private bool m_racerMoving = false;
    public bool m_laneLocked = false;
    private float m_racerPosition;//used to get the racer track position 
    #endregion
    void Start()
    {
        m_thinkTimeGO = GameObject.FindGameObjectWithTag("GameManager");
        base.Start(); //caling base
        //randomly select behaviour 
        m_behaviour = "";
        //DebugText("no target");
    }


    void Update()
    {
      
        base.Update();

        if (m_raceManager.GetComponent<RaceManager>().m_racing)
        {
            if (m_thinkTimeSet == false)
            {
                //call set think time. this is done in update instead of start to ensure that think time calculator has finished calulating. 
                SetThinkTime();

            }
            //loops for the amount of rays needed
            if (m_racerMoving == false)
            {

                for (int i = 0; i < m_rayConeSize; i++)
                {
                    m_rayHitTarget = false;

                    //REREFENCE FOR CONCEPTING THE RAY OF CONES ONLY :www.youtube.com. (n.d.). Experimenting with Obstacle Avoidance in Unity 3D. [online] Available at: https://www.youtube.com/watch?v=SVazwHyfB7g&t=1100s [Accessed 20 Jul. 2021].‌
                    //getting AI rotation
                    Quaternion m_rayRotation = transform.rotation;
                    //chaning the angle of each ray when it loops so they are equally spread along  creating a view cone. 
                    Quaternion m_rotationMutiplyer = Quaternion.AngleAxis(angle: (i / ((float)m_rayConeSize)) * m_rayAngle * 2 - m_rayAngle, transform.up);
                    //create a direction for each ray to point towards 
                    Vector3 m_rayDirection = m_rayRotation * m_rotationMutiplyer * Vector3.forward;
                    //if any of the 20 rays hits a collider 
                    if (Physics.Raycast(transform.position, m_rayDirection, out m_rayHit, m_rayRange))
                    {
                        // checks list to see if AI has hit any potential targets. 
                        if (m_trackTargets.Contains(m_rayHit.collider.transform.tag))
                        {

                            string target = m_rayHit.collider.transform.tag;
                            //getting lane from parent.
                            m_laneCheck = m_lane;
                            //check parent script to see what lane the target is on 
                            switch (target)
                            {
                                case "Player":
                                    if (m_laneCheck == m_rayHit.collider.transform.GetComponentInParent<PlayerInput>().m_lane && m_laneLocked == false)
                                    {
                                        //After Think time  move in a random direction 
                                        StartCoroutine(UseThinkTime(m_thinkTime, "No direction"));
                                        //DebugText("overtake Player");
                                    }
                                    break;
                                case "Hazzard":
                                    // checking Behvaiour(see behaviour traits in header) 
                                    if (m_behaviour != "Small")
                                    {
                                        //hazzard allows to check for 2 lanes incase designer wants it to overlap. 
                                        if (m_laneCheck == m_rayHit.collider.transform.GetComponentInParent<TrackHazzards>().m_laneOne || m_laneCheck == m_rayHit.collider.transform.GetComponentInParent<TrackHazzards>().m_laneOne + 1)
                                        {
                                            StartCoroutine(UseThinkTime(m_thinkTime, "No direction"));
                                            //DebugText("move away from hazzard");
                                        }
                                    }
                                    if (m_behaviour == "Small")
                                    {
                                        //if small go towards the hazzard to loose health 
                                        GoTowardsItem(m_rayHit.collider.transform.GetComponentInParent<TrackHazzards>().m_laneOne);
                                        //DebugText("move twoards hazzard");
                                    }
                                    break;
                                //if any AI is target
                                case "AIOne":
                                case "AITwo":
                                case "AIThree":
                                case "AIFour":
                                case "AIFive":
                                    {
                                        //check to see if target is in the same lane and its not its self 
                                        if (m_laneCheck == m_rayHit.collider.transform.GetComponentInParent<AI>().m_lane && target != transform.GetChild(0).tag && m_laneLocked == false)
                                        {
                                            //starts corotine for AI think time delay
                                            StartCoroutine(UseThinkTime(m_thinkTime, "No direction"));
                                            //DebugText("overtake AI");
                                        }

                                    }
                                    break;
                                case "ThrowCheese":
                                    if (m_behaviour == "Big")
                                    {
                                        //go towards the throwCheese pickup
                                        GoTowardsItem(m_rayHit.collider.transform.GetComponentInParent<ThrowCheesePickUp>().m_lane);
                                        //DebugText("ThrowCheese");
                                    }
                                    break;
                                case "MeltedCheese":
                                    if (m_behaviour == "Aggreeseive")
                                    {
                                        //go towards melted cheese pick up if behaviour is Aggro
                                        GoTowardsItem(m_rayHit.collider.transform.GetComponentInParent<SlowCheesePickUp>().m_lane);
                                        //DebugText("Melted Cheese");
                                    }
                                    break;
                                case "SpearThrow":
                                    if (m_behaviour == "Aggreeseive")
                                    {
                                        //DebugText("SpearThrow");
                                        GoTowardsItem(m_rayHit.collider.transform.GetComponentInParent<ForkPickUp>().m_lane);
                                    }
                                    break;
                                case "CheeseItem":
                                    if (m_behaviour == "Big")
                                    {
                                        //go towards enviroment cheese to gain health 
                                        GoTowardsItem(m_rayHit.collider.transform.GetComponentInParent<EnivromentCheeseScript>().m_lane);
                                        //DebugText(" Enviroment Cheese");
                                    }
                                    break;


                            }


                        }

                        
                    }
                    //if a ray has hit a required target exit loop for this update cycle. 
                    if (m_rayHitTarget)
                        break;
                }
            }
        }
        
    }
    //ray gizmos 
    private void OnDrawGizmos()
    {
        for (int i = 0; i < m_rayConeSize; i++)
        {
            Quaternion m_rayRotation = transform.rotation;
            Quaternion m_rotationMutiplyer = Quaternion.AngleAxis(angle: (i / ((float)m_rayConeSize)) * m_rayAngle * 2 - m_rayAngle, transform.up);
            Vector3 m_rayDirection = m_rayRotation * m_rotationMutiplyer * Vector3.forward;
            Gizmos.DrawRay(transform.position, m_rayDirection);
        }
    }
    /**************************************************************************************
    * Type: (function)
    * 
    * Name: MoveLane
    * 
    * 
    *
    * Author: Joseph Gilmore
    *
    * Description: when AI needs to move lane it checks to see which direction it can move
    * Change Log:
    * 
    * 
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 20/07/2021    JG         1.00        -Initial Created
    * 27/07/2021    JG         1.1         -added pass through for directions 
    * **************************************************************************************/
    private void MoveLane(string p_direction)
    {
        
        //if a random direction is requried 
        if(p_direction == "No direction")
        {
            //check to see if AI is on outside lanes
            if (m_laneCheck == 0)
                m_moveLeft = false;
            else
                m_moveLeft = true;

            if (m_laneCheck == 4)
                m_moveRight = false;
            else
                m_moveRight = true;
            //if its not in the outside lanes AI can move in both directions. 
            if (m_moveRight == true && m_moveLeft == true)
            {
                //gets a random number to pick to move left/right
                System.Random ran = new System.Random();
                int ranNum = ran.Next(0, 2);
                if (ranNum == 1)
                {
                    MoveRight();
                }
                else
                    MoveLeft();

            }
            //if it can only move to one lane , move to correct lane 
            if (m_moveLeft == false && m_moveRight)
            {
                MoveRight();
            }
            if (m_moveLeft && m_moveRight == false)
            {
                MoveLeft();
            }
           
           
        }
        //these are used for when a certain direction needs to be called.
        ///he reason why we dont't  just call the functions is because these functions are in the parent meaning cant access child bool from them and easier to use thinktime. 
        if (p_direction == "Right")
        {
            MoveRight();
           
        }
        if (p_direction == "Left")
        {
            MoveLeft();
           
        }
        m_racerMoving = false;
      
    }
    /**************************************************************************************
   * Type: (IEnumerator)
   * 
   * Name: UseThinkTime
   * 
   * 
   *
   * Author: Joseph Gilmore
   *
   * Description: used apply think time and create a delay
   * Change Log:
   * 
   * 
   * Date          Initials    Version     Comments
   * ----------    --------    -------     ----------------------------------------------
   * 21/07/2021    JG        1.00        -Initial Created
   * 30/07/2021    JG        1.01        -added the bools here  to remove repeat code in update 
   * **************************************************************************************/
    IEnumerator UseThinkTime(float p_time, string p_direction)
    {
        //used to show racer is in the process of moving so don't do another action until its moved
        m_racerMoving = true;
        //exit ray loop 
        m_rayHitTarget = true;
        //sets delay to AI think time. 
        yield return new WaitForSeconds(p_time);
        //after delay move lane 
        MoveLane(p_direction);
        //stop coroutine so it does not loop foreevr 
        StopCoroutine("UseThinkTime");

    }
    /**************************************************************************************
   * Type: (Function)
   * 
   * Name: set think time
   * 
   * 
   *
   * Author: Joseph Gilmore
   *
   * Description: used to set the correct think time to the correct AI 
   * Change Log:
   * 
   * 
   * Date          Initials    Version     Comments
   * ----------    --------    -------     ----------------------------------------------
   * 23/07/2021    JG          1.00        -Initial Created
   * 26/07/2021    JG          1.01        -changed to check if think times does not = null before comfirming think time is set 
   * 30/07/2021    JG          1.02        -changed to find the name of the child instead of the order index 
   * **************************************************************************************/
    private void SetThinkTime()
    {
       
        //gets the gameobject tag from child transform 
        switch (transform.Find("Model").tag)
        {
            case "AIOne":
                //sets correct think time calulated for that AI 
                m_thinkTime = m_thinkTimeGO.GetComponentInParent<ThinkTimeCalulator>().m_aiOneThinkTime;
                m_behaviour = m_thinkTimeGO.GetComponentInParent<ThinkTimeCalulator>().m_assignedBehaviours[0];
                break;
            case "AITwo":
                m_thinkTime = m_thinkTimeGO.GetComponentInParent<ThinkTimeCalulator>().m_aiTwoThinkTime;
                m_behaviour = m_thinkTimeGO.GetComponentInParent<ThinkTimeCalulator>().m_assignedBehaviours[1];
                break;
            case "AIThree":
                m_thinkTime = m_thinkTimeGO.GetComponentInParent<ThinkTimeCalulator>().m_aiThreeThinkTime;
                m_behaviour = m_thinkTimeGO.GetComponentInParent<ThinkTimeCalulator>().m_assignedBehaviours[2];
                break;
            case "AIFour":
                m_thinkTime = m_thinkTimeGO.GetComponentInParent<ThinkTimeCalulator>().m_aiFourThinkTime;
                m_behaviour = m_thinkTimeGO.GetComponentInParent<ThinkTimeCalulator>().m_assignedBehaviours[3];

                break;
            case "AIFive":
                m_thinkTime = m_thinkTimeGO.GetComponentInParent<ThinkTimeCalulator>().m_aiFiveThinkTime;
                m_behaviour = m_thinkTimeGO.GetComponentInParent<ThinkTimeCalulator>().m_assignedBehaviours[4];
                break;
        }
        //check to see if think time has been set by calculator if it has does not need to set again
        if(m_thinkTime != 0)
            m_thinkTimeSet = true;
        //DebugText("");

    }
    /**************************************************************************************
   * Type: (Function)
   * 
   * Name: AIGotPickUP
   * 
   * 
   *
   * Author: Joseph Gilmore
   *
   * Description: manage AI pickups and check whats active 
   * Change Log:
   * Date          Initials    Version     Comments
   * ----------    --------    -------     ----------------------------------------------
   * 23/07/2021    JG        1.00         -Initial Created
   * 27/07/2021    JG        1.01         -added the effect of behaviours 
   * 30/07/2021    JG        1.10         -Moved big switch statement when it checks pickup/behvaiour to AIUsePickUp and added Think time 
   * 04/08/2021    JG        1.11         -replaced thinkTime tag with GameManager tag
   * **************************************************************************************/
    public void AIGotPickUp(string p_pickUp, GameObject p_pickUpObject)
    {
        //get posistion of the racer 
        m_racerPosition = GameObject.FindGameObjectWithTag("GameManager").GetComponentInParent<PositionTracker>().GetPosition(gameObject);
        //applys think time to use pick up 
        StartCoroutine(PickUpThinkTime(m_thinkTime, p_pickUp, p_pickUpObject));
        
    }
    /**************************************************************************************
  * Type: (Function)
  * 
  * Name: AIUsePickUp
  * 
  * 
  *
  * Author: Joseph Gilmore
  *
  * Description: manage AI pickups abd check behaviour for certain action
  * Change Log:
  * Date          Initials    Version     Comments
  * ----------    --------    -------     ----------------------------------------------
  * 30/07/2021    JG        1.00         -Initial Created
  * 10/08/2021    JG        1.01         -changed the "throwPickUp" to the new child "ThrowCheesePickUp". Changed name of meltedcheese to slow cheese 
  * 18/08/2021    JG        1.02         -throw cheese now just has one function which passes through the direaction. It used to have a function for each direaction
  * **************************************************************************************/
    private void AIUsePickUp(string p_pickUp,GameObject p_pickUpObject)
    {
        
        switch (p_pickUp)
        {
            case "Cheese":
                //check behaviour 
                if (m_behaviour == "Big")
                {
                    //throw cheese in front as it wants to be big
                    p_pickUpObject.GetComponent<ThrowCheesePickUp>().ThrowCheese("Foward");
                }
                if (m_behaviour == "Aggreeseive")
                {

                    //if position does not equal first or  last throw cheese randomly 
                    if (m_racerPosition != 1f || m_racerPosition != 6f)
                    {
                        System.Random ran = new System.Random();
                        int ranNum = ran.Next(0, 2);
                        //randomly pick to throw cheese behind or in front
                        if (ranNum == 1)
                        {

                            p_pickUpObject.GetComponent<ThrowCheesePickUp>().ThrowCheese("Foward");
                        }
                        if (ranNum == 0)
                        {

                            p_pickUpObject.GetComponent<ThrowCheesePickUp>().ThrowCheese("Backward");
                        }
                    }
                    //IF AI is in first  throw behind 
                    if (m_racerPosition == 1f)
                    {
                        p_pickUpObject.GetComponent<ThrowCheesePickUp>().ThrowCheese("Backward");
                    }
                    //if in last throw in front 
                    if (m_racerPosition == 6f)
                    {
                        p_pickUpObject.GetComponent<ThrowCheesePickUp>().ThrowCheese("Foward");
                    }

                }
                if (m_behaviour == "Small")
                {
                    //check to see if racer is not in first or last 
                    if (m_racerPosition != 1f || m_racerPosition != 6f)
                    {
                        System.Random ran = new System.Random();
                        int ranNum = ran.Next(0, 2);
                        //randomly pick to throw cheese behind or in front and move lane. small cheese does  not want to pick this back up
                        if (ranNum == 1)
                        {

                            p_pickUpObject.GetComponent<ThrowCheesePickUp>().ThrowCheese("Foward");
                            MoveLane("No direction");
                        }
                        if (ranNum == 0)
                        {

                            p_pickUpObject.GetComponent<ThrowCheesePickUp>().ThrowCheese("Backward");
                        }
                    }
                    //IF AI is in first  throw behind 
                    if (m_racerPosition == 1f)
                    {
                        p_pickUpObject.GetComponent<ThrowCheesePickUp>().ThrowCheese("Backward");
                    }
                    //if in last throw in front 
                    if (m_racerPosition == 6f)
                    {
                        p_pickUpObject.GetComponent<ThrowCheesePickUp>().ThrowCheese("Foward");
                        MoveLane("No direction");
                    }
                }
                break;
            case "Spear":
                //when locked fire instanly 
                m_laneLocked = true;
                break;
            case "MeltedCheese":
                //throws melted cheese 
                p_pickUpObject.GetComponent<SlowCheesePickUp>().ThrowMeltedCheese();
                break;

        }
    }
    /**************************************************************************************
   * Type: (Function)
   * 
   * Name: GoTowardsItem
   * 
   * 
   *
   * Author: Joseph Gilmore
   *
   * Description: Used to move racer towards an item in the lanes left or right of it
   * Change Log:
   * Date          Initials    Version     Comments
   * ----------    --------    -------     ----------------------------------------------
   * 27/07/2021    JG        1.00        -Initial Created
   * **************************************************************************************/
    private void GoTowardsItem(float p_itemLane)
    {
        // check if the item is in the lane either side of racer
        if (m_laneCheck == p_itemLane + 1)
        {
            
            //move racer in the direction of item
            StartCoroutine(UseThinkTime(m_thinkTime, "Left"));
        }
        if (m_laneCheck == p_itemLane - 1)
        {
            
            StartCoroutine(UseThinkTime(m_thinkTime, "Right"));
        }
       
        
    }
    /**************************************************************************************
   * Type: (IEnumerator)
   * 
   * Name: PickUpThinkTime
   * 
   * 
   *
   * Author: Joseph
   *
   * Description: apply think time for when the AI Should use a pickup
   * Change Log:
   * 
   * 
   * Date          Initials    Version     Comments
   * ----------    --------    -------     ----------------------------------------------
   * 30/07/2021    JG        1.00           -created
   * **************************************************************************************/
    IEnumerator PickUpThinkTime(float p_time, string p_pickUP, GameObject p_pickUpbject)
    {
        
         string pickUpName = p_pickUP;
        GameObject pickUpObject = p_pickUpbject;
        //sets delay to AI think time. 
        yield return new WaitForSeconds(p_time);
        //after delay use pick up
        AIUsePickUp(pickUpName, pickUpObject);
        //stop coroutine so it does not loop foreevr 
        StopCoroutine(" PickUpThinkTime");

    }
    //debug text is commented out as its not used for now but if i wanted to use it again i can just uncomment 
    //private void DebugText(string p_targetText)
    //{
    //    if (transform.Find("Text/Behaviour") != null)
    //    {
    //        TextMeshPro Behaviour = transform.Find("Text/Behaviour").GetComponent<TextMeshPro>();
    //        Behaviour.text = string.Format(m_behaviour);
    //    }

    //    if (transform.Find("Text/Target") != null)
    //    {
    //        TextMeshPro Target = transform.Find("Text/Target").GetComponent<TextMeshPro>();
    //        Target.text = string.Format(p_targetText);
    //    }
        
    //    if (transform.Find("Text/ThinkTime") != null)
    //    {
    //        TextMeshPro ThinkTime = transform.Find("Text/ThinkTime").GetComponent<TextMeshPro>();
    //        ThinkTime.text = string.Format(m_thinkTime.ToString());
    //    }
    //}
 
}
