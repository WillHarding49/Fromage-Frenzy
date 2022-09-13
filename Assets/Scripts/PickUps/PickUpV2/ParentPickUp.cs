using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**************************************************************************************
* Type: (Class)
* 
* Name: ParentPickUp.cs
* 
* 
*
* Author: Joseph Gilmore 
*
* Description: Main Parent for pickUp class. This class is using existing code created for each individual pickup put togther for inheretance. This is why the change log is small.
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 10/08/2021    JG           1.00       -created 
* 11/08/2021    WH           1.01       -added and fixed pickup UI changer
* 11/08/2021    JG           1.02       -changed UI update by changing var instead 
* 13/08/2021    JG           1.03       -removed the use of pickUpManager and found a more effectint way. 
* 18/08/2021    JG           1.04       -changed pickUpActive to protected & move code from update into functions
* **************************************************************************************/

public abstract class ParentPickUp : MonoBehaviour
{
    #region vars
    protected bool m_pickUpActive = false;
    private bool m_respawnPickUp = false;
    protected Transform m_playerTarget;//used to get the position of the target racer.  
    [SerializeField][Tooltip("The amount of time for the pick up to respawn, Keep in mind its after this time and if the pickup has been used")][Range(1,30)]
    private float m_respawnTime = 10f;
    protected string m_activeRacer;
    protected  string m_pickUpName;//name of pick up for pickup manager 
    protected PickUpPool m_pickUpPool; //Game has object pooling for all collectable items 
    private AudioSource m_pickUpSound;
    [SerializeField][Tooltip("the object you use for each pickup")]
    protected GameObject m_prefab;
    #endregion
    
   protected void Start()
    {
        m_pickUpSound = GetComponent<AudioSource>();
        m_pickUpPool = FindObjectOfType<PickUpPool>();
    }

    protected void Update()
    {
        CheckRespawn();
        UpdatePickUp();
    }
    /**************************************************************************************
  * Type: function)
  * 
  * Name: CheckRespawn
  * 
  * 
  *
  * Author: Joseph Gilmore
  *
  * Description: Check to see if pick up needs repsawning 
  * Change Log:
  * Date          Initials    Version     Comments
  * ----------    --------    -------     ----------------------------------------------
  * 18/08/2021    JG          1.00          -moved code from Update 
  * **************************************************************************************/
    private void CheckRespawn()
    {
        if (m_respawnPickUp == true && m_pickUpActive == false)
        {
            //reactive trigger for pickup to be repawned 
            m_respawnPickUp = false;
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    /**************************************************************************************
  * Type: function)
  * 
  * Name: UpdatePickUp
  * 
  * 
  *
  * Author: Joseph Gilmore
  *
  * Description: This function is for check duplicates of pickUps and updating UI
  * Change Log:
  * Date          Initials    Version     Comments
  * ----------    --------    -------     ----------------------------------------------
  * 18/08/2021    JG          1.00          -moved code from Update 
  * **************************************************************************************/
    private void UpdatePickUp()
    {
        if (m_pickUpActive)
        {
            //if the racer has picked up another pick up while this one is active deactivate pickup
            if (GameObject.FindGameObjectWithTag(m_activeRacer).GetComponentInParent<CheeseMovement>().m_activePickUp != m_pickUpName)
            {

                m_pickUpActive = false;
                ReturnSpearToPool();
            }
        }
        //removing the pickup from Ui when its no longer active 
        if (m_pickUpActive == false && m_activeRacer == "Player")
        {
            GameObject.FindObjectOfType<UIController>().m_item = "";
            m_activeRacer = "";
        }
        //passing through the active pick up to the UI
        if (m_pickUpActive == true && m_activeRacer == "Player")
        {
            GameObject.FindObjectOfType<UIController>().m_item = m_pickUpName;
        }
    }
    /**************************************************************************************
   * Type: function)
   * 
   * Name: PlayerPickedUp.cs
   * 
   * 
   *
   * Author: Joseph
   *
   * Description: This function is about activating the corret pick up    *
   * Change Log:
   *  
   * 
   * Date          Initials    Version     Comments
   * ----------    --------    -------     ----------------------------------------------
   * 25/06/2021    JG        1.00        -Initial Created
   * 28/06/2021    JG        1.01        -Added active racer pass through 
   * 14/07/2021    JG        1.02        -added for one AI to pick it up
   * 28/07/2021    JG        1.03        -if statement added for activing AI
   * 01/02/2021    JG        1.04        -cleaned AI refernce 
   * 03/08/2021    JG        1.05        -added sound
   * 08/09/2021    JG        1.06        -removed static call of pickUpManager 
   * 10/08/2021    JG        1.10        -This code is copied from all indvisual pickup scripts. It has been moved because of inhereitence. added instance check for spear
   * 13/08/2021    JG        1.11        -added a if statement to check if you already have the pickup, if you do no need to active this instance 
   * **************************************************************************************/
    public void PlayerPickedUp(string p_activeRacer)
    {
        //setting who triggered the pick up
        m_activeRacer = p_activeRacer;
        //check if you already have pick up so it does not create double when you use it
        if (GameObject.FindGameObjectWithTag(m_activeRacer).GetComponentInParent<CheeseMovement>().m_activePickUp == m_pickUpName)
        {
            m_pickUpActive = false;
        }
        else
        {
            //sets instance check for spear to true 
            SpearInstanceCheck();
            //sound effect
            m_pickUpSound.Play();
            m_playerTarget = GameObject.FindGameObjectWithTag(p_activeRacer).transform;
            m_pickUpActive = true;
            //updating pickup manager 
            GameObject.FindGameObjectWithTag(m_activeRacer).GetComponentInParent<CheeseMovement>().m_activePickUp = m_pickUpName;
            
            if (p_activeRacer != "Player")
            {
                //tells AI it has cheese pick up. passes through correct throwPickUp instance.
                GameObject.FindGameObjectWithTag(m_activeRacer).GetComponentInParent<AI>().AIGotPickUp(m_pickUpName, gameObject);
            }
        }
        //sets trigger box to inactive so its not visable nor will it trigger. 
        transform.GetChild(0).gameObject.SetActive(false);
        //after 10 secs and pick up has been thrown 
        StartCoroutine(RespawnPickUp(m_respawnTime));
    }
    /**************************************************************************************
   * Type: IEnumerator 
   * 
   * Name: RespawnPickUp
   * 
   * 
   *
   * Author: Joseph
   *
   * Description: used for a timer to repspawn pickup after a certian time 
   * Change Log:
   * Date          Initials    Version     Comments
   * ----------    --------    -------     ----------------------------------------------
     02/08/2021     JG         1.00         created
    10/08/2021      JG         1.10         - moved from old pickUp scripts (now the children)
   * **************************************************************************************/
    IEnumerator RespawnPickUp(float p_time)
    {


        //sets delay to AI think time. 
        yield return new WaitForSeconds(p_time);
        //after delay use pick up
        m_respawnPickUp = true;
        //stop coroutine so it does not loop foreevr 
        StopCoroutine("RespawnPickUp");

    }
    //use to return the spear in the object pool in its class. 
    protected abstract void ReturnSpearToPool();
    //used to set the status of if the spear has created an intsance 
    protected abstract void SpearInstanceCheck();
}

