using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**************************************************************************************
* Type: (Class)
* 
* Name: MeltedCheesePickUp.cs
* 
* 
*
* Author: Joseph Gilmore
*
* Description: Third pickup. Purpose is to deploy melted cheese behind the active racer to slow oppenents 
*
* Change Log:
* 
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 09/07/2021    JG        1.00       -Initial Created
* 28/07/2021    JG        1.01       -added lane checker , moved more code into a function and allowed AI to use pickup
* 02/08/2021    JG        1.02       -added respawn timer 
* 03/08/2021    JG        1.03       -fixed the position you throw from
* 10/08/2021    JG        1.10       -this class code is from an older script. It now uses inheretance so all duplicte code has been removed (respawn, pickUp manager checks, player pickup)
* 18/08/2021    JG        1.11       -fixed a bug when the melted cheese would deploy before use due to last cylce being cut short 
**************************************************************************************/

public class SlowCheesePickUp : ParentPickUp
{
    #region vars
    private bool m_pickUpThrown = false;
    private Vector3 m_colliderBound; //bounds of cheese
    private float m_yThrowPos; //setting the throw position up
    private float m_zThrowPos;
    private float m_timer;
    private float m_waitTime;
    [Tooltip("Please enter what lane you put the pick up in")]
    public float m_lane;
    [SerializeField][Tooltip("The time between the melted cheese deployments")][Range(0.3f,2f)]
    private float m_timeBetween = 0.5f;
    private float m_cheeseThrown = 0; //how many times the cheese has been thrown
    private Vector3 m_oneRBPos; //ech different instances throw positions. 
  
    #endregion
    void Start()
    {
        base.Start();
        m_pickUpName = "MeltedCheese";
    }

    
    void Update()
    {
        base.Update();
        //creating a timer
        m_timer += Time.deltaTime;
        if (m_pickUpActive)
        {
            //user presses e (can only do once)
            if (Input.GetKeyDown(KeyCode.E) && m_pickUpThrown == false && m_activeRacer == "Player")
            {

                ThrowMeltedCheese();

            }
            //afte timeBetween has been hit throw cheese. Reset and throw once more
            if (m_timer >= m_waitTime && m_pickUpThrown)
            {
                ThrowMeltedCheese();
                m_waitTime = m_timer + m_timeBetween;
                m_cheeseThrown++;
                if (m_cheeseThrown >= 2)
                {
                    m_pickUpActive = false;
                    m_pickUpThrown = false;
                    m_cheeseThrown = 0;
                    //resets active pickup
                    GameObject.FindGameObjectWithTag(m_activeRacer).GetComponentInParent<CheeseMovement>().m_activePickUp = "";
                }
            }


        }
        else
        {
            m_pickUpThrown = false;
            m_cheeseThrown = 0;
        }
          

    }
    /**************************************************************************************
    * Type: (Function)
    * 
    * Name: ThrowMeltedCheese
    * 
    * 
    *
    * Author: Joseph Gilmore
    *
    * Description: used create an instantce in the most recent position*
    * Change Log:
    * 
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 09/07/2021    JG        1.00        -Initial Created
    * 13/07/2021    JG        1.01        -Object pool added
    * 28/07/2021    JG        1.02        -rearranged code so it can be called from AI and work as intented
    * 03/08/2021    JG        1.03        -changed wthe posiiton where it thrown from so it alway does from behind. added sound
    **************************************************************************************/
    public void ThrowMeltedCheese()
    {
        //gets audio source from racer to play through sound. 
        m_playerTarget.parent.Find("ThrowSound").GetComponent<AudioSource>().Play();
       
        //gets bounds from transform
        m_colliderBound = m_playerTarget.GetComponent<Collider>().bounds.size;
        //works out the the center point from the object orgin.
        m_yThrowPos = (m_colliderBound.y) / 2f;
        m_zThrowPos = (m_colliderBound.z) / 2f;
        //creates a position to throw from the current player so it does not collide when its created 
        m_oneRBPos = new Vector3(m_playerTarget.position.x, m_playerTarget.position.y + m_yThrowPos, m_playerTarget.position.z);
        //ensure on the rotation that it alway throws behind and checks t size of the cheese.
        m_oneRBPos += m_playerTarget.forward * (m_zThrowPos - 1.08f);
        // gets new object from pool
        GameObject newCheese = m_pickUpPool.GetObject(m_prefab);
        //sets position
        newCheese.transform.position = m_oneRBPos;
        //sets up wait timer 
        m_waitTime = m_timer + m_timeBetween;
        m_pickUpThrown = true;
    }

    //not required for this class but casues error if not here 
    protected override void ReturnSpearToPool()
    {

    }
    protected override void SpearInstanceCheck()
    {

    }

}

