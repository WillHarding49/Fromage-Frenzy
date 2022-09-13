using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/**************************************************************************************
* Type: (Class)
* 
* Name: SpearPickUp.cs
* 
* 
*
* Author: Joseph Gilmore
*
* Description: This is the Secound pick up. Racers can pickUp the spear and lock it onto a target direactly in front of it. once locked can fire 
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
*28/06/2021    JG           1.00        -Initial Created
*29/06/2021    JG           1.01        -added a switch statement to check for all racers on the track to see which pick up is acitve. Curretnly known bug is that it will only delect player as I am using a unity event in the trigger meaning I cant pass who is hitting it 
*04/06/2021    JG           1.02        -been trying to be able to get the spears to fire in a cone direction sytem using 3 raycasts. wip
*07/07/2021    JG           1.03        -removed a large chunk of code. it now only raycasts and fires in a forward direction due to designers instructions 
*09/07/2021    JG           1.04        -added to change the spear position relitve to the size of the racer. (need adding for AI Intergration). also cleaning. 
*21/07/2021    JG           1.05        -small AI Cleaning
*27/7/2021     JG           1.06        -added lane check
*03/08/2021    JG           1.07        -removed collider bound and now pass through data to the spear prefab instance due to collison bugs. sound also added. 
09/08/2021     JG           1.08        -removed the straight direction var and just used playerTarget.foward instead. removed static call of pick up manager 
10/08/2021     JG           1.10        -This class is now a child , removed (respawn, pickUp manager checks, player pickup)
11/08/2021     JG           1.11        -removed old UI and replaced with actual game UI
15/08/2021     JG           1.12        -moved code from update function to separate functions for cleaning 
16/08/2021     JG           1.13        -removed using add force when firing the spear and replaced with movetowards 
18/08/2021     JG           1.14        -moved movespear code from udpate to function
* **************************************************************************************/
public class ForkPickUp : ParentPickUp
{
    #region vars
    private bool m_instanceCreated = false; //bools for checking the status of the item
    private bool m_intanceCheck = false;
    [Tooltip("do not change")]
    public bool m_spearThrown = false;
    [SerializeField][Tooltip(" The speed of the spear")][Range(5,30)]
    private float m_throwSpearForce = 20f;
    [SerializeField][Tooltip("The max range the racer can detect a target to fire the spear at")][Range(5,30)]
    private float m_maxSpearRange = 20f;
    private float m_yThrowPos; //position you throw the spear from
    [Tooltip("Please enter the lane you put the pick up in")]
    public float m_lane;
    private List<string> m_racerNames = new List<string>() { "Player", "AIOne", "AITwo", "AIThree", "AIFour", "AIFive" };
    private RaycastHit m_hitFoward;
    private Vector3 m_colliderBound; // used to get then collider 
    private GameObject m_racerTarget; //the intented target of the spear
    private GameObject m_spearPickUpInstance; //used as the throwable instance
    #endregion
    void Start()
    {
        base.Start();
        m_pickUpName = "Spear";//setting name 
    }


    void Update()
    {
        base.Update();
        if (m_pickUpActive)
        {
            SpearSetUp();
            SpearCasting();
            
            
        }
        MoveSpearTowardsTarget();


    }
    /**************************************************************************************
   * Type: (function)
   * 
   * Name: MoveSpearTowardsTarget
   * 
   * 
   *
   * Author: Joseph Gilmore
   *
   * Description: This function is to move spear towards intented target
   * Change Log:
   *  
   * 
   * Date          Initials    Version     Comments
   * ----------    --------    -------     ----------------------------------------------
   * 18/08/2021    JG          1.00        -code has been moved from Update to keep things clean 
   * **************************************************************************************/
    private void MoveSpearTowardsTarget()
    {
        if (m_spearThrown)
        {

            float step = m_throwSpearForce * Time.deltaTime;
            //move the spear towards the racer target every frame by the amounf of spearForce 
            m_spearPickUpInstance.transform.position = Vector3.MoveTowards(m_spearPickUpInstance.transform.position, m_racerTarget.transform.position, step);
            if (m_spearPickUpInstance.activeSelf == false)
            {
                //if targte been hit deactivate 
                m_spearThrown = false;
            }
        }
    }
    /**************************************************************************************
   * Type: (function)
   * 
   * Name: SpearSetUp
   * 
   * 
   *
   * Author: Joseph Gilmore
   *
   * Description: This function is to set up the spear item before its fired (this code has been moved from the update).
   * Change Log:
   *  
   * 
   * Date          Initials    Version     Comments
   * ----------    --------    -------     ----------------------------------------------
   * 15/08/2021    JG          1.00        -code has been moved from Update to keep things clean 
   * **************************************************************************************/
    private void SpearSetUp()
    {
        //gets the current size of collider. this is if the cheese changes size it adjust the position of the spear
        m_colliderBound = m_playerTarget.GetComponent<Collider>().bounds.size;
        m_yThrowPos = (m_colliderBound.y) / 2 + 0.5f;

        if (m_instanceCreated == false && m_intanceCheck == true)
        {
            //checks object pool for instance to get it
            m_spearPickUpInstance = m_pickUpPool.GetObject(m_prefab);
            //setting the active racer in the item so it does not collide with itself while its above the racer 
            m_spearPickUpInstance.GetComponentInParent<SpearPrefab>().m_activeRacer = m_activeRacer;
            m_instanceCreated = true;
        }
        if (m_instanceCreated == true)
        {
            //constently updates the instance postion so it remains above the player 
            m_spearPickUpInstance.transform.position = new Vector3(m_playerTarget.position.x, m_playerTarget.position.y + m_yThrowPos, m_playerTarget.position.z);

        }
    }
        /**************************************************************************************
      * Type: function)
      * 
      * Name: SpearCasting
      * 
      *
      * Author: Joseph Gilmore
      *
      * Description: This function is to dectect the spears target and lock on. Also updates UI
      * Change Log:
      * Date          Initials    Version     Comments
      * ----------    --------    -------     ----------------------------------------------
      * 15/08/2021    JG          1.00        -code has been moved from Update to keep things clean 
      * **************************************************************************************/
        private void SpearCasting()
        {
            //sends out ray in  a forward direction of the player 
            if (Physics.Raycast(m_playerTarget.position, m_playerTarget.forward, out m_hitFoward, m_maxSpearRange))
            {

                //checks to see if the ray hits a racer 
                if (m_racerNames.Contains(m_hitFoward.collider.transform.tag))
                {
                    //locks onto target 
                    m_spearPickUpInstance.transform.LookAt(m_hitFoward.transform);
                    m_racerTarget = GameObject.FindGameObjectWithTag(m_hitFoward.collider.tag);
                    
                if (m_activeRacer == "Player")
                    {
                        //tells player it can fire in the UI
                        GameObject.FindObjectOfType<UIController>().m_lockedOn = true;
                        //press fire key to throw spear
                        if (Input.GetKeyDown(KeyCode.E))
                            ThrowSpear();
                    }
                    //AI fire when they lock onto  the first target they get 
                    if (m_activeRacer != "Player")
                    {
                        ThrowSpear();
                        //remove lane lock
                        m_playerTarget.GetComponentInParent<AI>().m_laneLocked = false;
                    }
                }
                else
                {
                    if (m_activeRacer == "Player")
                    {
                        //removes the locked on code from UI
                        GameObject.FindObjectOfType<UIController>().m_lockedOn = false;
                    }

                }
            }
            else if (m_intanceCheck == true)
            {
                //look forward if target not locked and a speare has been made 
                m_spearPickUpInstance.transform.LookAt(m_playerTarget.TransformDirection(Vector3.forward));
            if (m_activeRacer == "Player")
            {
                //removes the locked on code from UI
                GameObject.FindObjectOfType<UIController>().m_lockedOn = false;
            }
            }
        }
        
    
    /**************************************************************************************
    * Type: function)
    * 
    * Name: ThrowSpear
    * 
    * 
    *
    * Author: Joseph Gilmore
    *
    * Description: This function is for Throwing the spear in the direction of hit raycast
    * Change Log:
    *  
    * 
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 25/06/2021    JG       1.00           -Initial Created
    * 07/07/2021    JG       1.01           -Added raycast direction 
    * 28/07/2021    JG       1.02           -Normalized direction and added force mode to spear throw
    * 03/08/2021    JG       1.03           -added throw sound. 
    * 09/08/2021    JG       1.04           -added a bool for the spearprefab
    * 11/08/2021    JG       1.05           -updates UI
    * 13/08/2021    JG       1.06           -reset active pickup
    * 16/08/2021    JG       1.07           -removed adding force to a rb
    * **************************************************************************************/
    private void ThrowSpear()
    {
       
        //gets audio source from racer to play through sound. 
        m_playerTarget.parent.Find("ThrowSound").GetComponent<AudioSource>().Play();
        //tells object it has been fired and can now hit objects 
        m_spearPickUpInstance.GetComponent<SpearPrefab>().m_fired = true;
        //resting class 
        m_pickUpActive = false;
        m_instanceCreated = false;
        if (m_activeRacer == "Player")
        {
            //removes the locked on code from UI
            GameObject.FindObjectOfType<UIController>().m_lockedOn = false;
        }
        //throw spear
        m_spearThrown = true;
        //resets active pickup
        GameObject.FindGameObjectWithTag(m_activeRacer).GetComponentInParent<CheeseMovement>().m_activePickUp = "";
    }
    //returns spear to pool if the racer picks up another pickup
    protected override void ReturnSpearToPool()
    {
        m_pickUpPool.ReturnGameObject(m_spearPickUpInstance);
        m_instanceCreated = false;
    }
    //set intsanceCheck to true when the racer picks up 
    protected override void SpearInstanceCheck()
    {
        m_intanceCheck = true;
    }
}



