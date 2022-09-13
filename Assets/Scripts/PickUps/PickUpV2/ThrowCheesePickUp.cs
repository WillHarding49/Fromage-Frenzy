using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**************************************************************************************
* Type: (Class)
* 
* Name: ThrowCheesePickUp.cs
* 
* 
*
* Author: Joseph Gilmore
*
* Description: This is the first pick up. The player can throw in front/behind 
*
* Change Log:
*  
* 
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 25/06/2021    JG        1.00        -Initial Created
* 28/06/2021    JG        1.01        -Updated comments , changed so you can only fire once and if you pick up twice only fires a single cheese
* 29/06/2021    JG        1.02        -Added switch statement to update active pickup and ensure that only the most recent pickup for each player can be used
* 06/07/2021    JG        1.03        -Changed the direction of the throw.
* 09/07/2021    JG        1.04        -cleaning (not ready for AI Intergration only currently works for player)
* 13/07/2021    JG        1.05        -added Object Pooling 
* 23/07/2021    JG        1.06        -moved projectlies into functions  and inputs only work for player
* 27/07/2021    JG        1.07        -added a lane checker for the designer to fill when they put the pick up in the game 
* 02/08/2021    JG        1.08        -added respawn function
* 03/08/2021    JG        1.09        -sound effects added
* 09/08/2021    JG        1.10        -replaced call of pickUpManager from static to public 
* 10/08/2021    JG        1.11        -This code was from an older script that did not inheret. it now has a parent 
* 18/08/2021    JG        1.12         -combined throw cheese forward & throw cheese backards to make throwcheese function
* **************************************************************************************/
public class ThrowCheesePickUp : ParentPickUp
{

    #region Vars
    private bool m_forwardThrow = false;//used so physics can be applied in fixed update 
    private bool m_backwardThrow = false;//used so physics can be applied in fixed update 
    private Rigidbody m_cheesePickUpRB;//used as reference so can create instance of rb.
    [SerializeField][Tooltip("force applied when you throw the cheese forward, The bigger the force the more it will fly off the track & if too small the racer will just hit it")][Range(1,15)]
    private float m_cheeseFowardForce = 14f; //force used to throw cheese
    [SerializeField][Tooltip("force applied when throwing cheese behind if the force is too big the player cant see the deployment")][Range(1,6)]
    private float m_cheeseBackwardForce = 2f;//^
    private float m_yThrowPos;
    private float m_zThrowPos;
    [Tooltip("Please assign the lane which you put the pick up in")]
    public float m_lane; //the designer needs to assign the value of which lane they have pick up the box
    private Vector3 m_colliderBound;
    private Vector3 m_throwPos;
    private GameObject m_cheesePickUpInstance;

    #endregion
    void Start()
    {
        base.Start();
        m_pickUpName = "Cheese";
    }


    void Update()
    {
        base.Update();
        //checks to see if if player is the one who picked it up so they can use inputs
        if (m_pickUpActive && m_activeRacer == "Player")
        {

            //Press e to create an instance and fire forward 
            if (Input.GetKeyDown(KeyCode.E))
            {

                ThrowCheese("Foward");

            }
            //press Q to throw cheese behind the player 
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ThrowCheese("Backward");

            }


        }

    }
    private void FixedUpdate()
    {
        if (m_forwardThrow)
        {
            //throw cheese in front of the players position 
            m_cheesePickUpRB.AddRelativeForce(m_playerTarget.forward * m_cheeseFowardForce);
            m_forwardThrow = false;
        }
        if (m_backwardThrow)
        {
            //adds forces to the cheese in the direction behind the player 
            m_cheesePickUpRB.AddRelativeForce(-m_playerTarget.forward * m_cheeseBackwardForce);
            m_backwardThrow = false;
        }
    }

    /**************************************************************************************
   * Type: function)
   * 
   * Name: ThrowCheese
   * 
   * 
   *
   * Author: Joseph Gilmore
   *
   * Description: This function is used to create a instance and throw forward 
   * Change Log:
   *  
   * 
   * Date          Initials    Version     Comments
   * ----------    --------    -------     ----------------------------------------------
   * 23/07/2021    JG           1.01       -this code was crreated before but move from the update to functions so it can be called for AI. also added local multiplier.
   * 03/08/2021    JG           1.02       -added  throw sound. 
   * 10/08/2021    JG           1.03       -added a check if pick up active for AI calling 
   * 11/08/2021    JG           1.04       -reset active pick up here 
   * 18/08/2021    JG           1.10       -Combined functions "ThrowCheeseFoward" & "ThrowCheeseBackwards" due to some duplicate code. Now only this function for throwing cheese 
   * **************************************************************************************/
    public void ThrowCheese(string p_direction)
    {
        if (m_pickUpActive)
        {
            //gets audio source from racer to play through sound. 
            m_playerTarget.parent.Find("ThrowSound").GetComponent<AudioSource>().Play();
            //resets active pickup
            GameObject.FindGameObjectWithTag(m_activeRacer).GetComponentInParent<CheeseMovement>().m_activePickUp = "";

            //gets the up to date collider boundfs for when the cheese updates size 
            m_colliderBound = m_playerTarget.GetComponent<Collider>().bounds.size;
            //checks which direction is needed
            if(p_direction == "Foward")
            {
                //setting up position you throw the cheese from 
                m_yThrowPos = (m_colliderBound.y) / 2 + 0.5f;
                m_zThrowPos = (m_colliderBound.z) / 2 + 2f;
                //creates a separate position to throw the cheese in front of the player
                m_throwPos = new Vector3(m_playerTarget.position.x, m_playerTarget.position.y + m_yThrowPos, m_playerTarget.position.z + (m_playerTarget.forward.z * m_zThrowPos));
                //throw cheese in fixed update 
                m_forwardThrow = true;
            }
            if(p_direction == "Backward")
            {
                //set behind player
                m_yThrowPos = (m_colliderBound.y) / 2 + 0.25f;
                m_zThrowPos = (m_colliderBound.z) / 2 - 2f;
                //works out the position of where we want to throw the cheese from, this changes as the size of the cheese changes 
                 m_throwPos = new Vector3(m_playerTarget.position.x, m_playerTarget.position.y + m_yThrowPos, m_playerTarget.position.z + (m_playerTarget.forward.z * m_zThrowPos));
                m_backwardThrow = true;
            }
            //check pool for an instance of object
            m_cheesePickUpInstance = m_pickUpPool.GetObject(m_prefab);
            m_cheesePickUpRB = m_cheesePickUpInstance.GetComponent<Rigidbody>();
            m_cheesePickUpInstance.transform.position = m_throwPos;
            //sets to false so can throw once 
            m_pickUpActive = false;
        }
        
    }
    
    //not required for this class but casues error if not here 
    protected override void ReturnSpearToPool()
    {

    }
    protected override void SpearInstanceCheck()
    {
        
    }
}
