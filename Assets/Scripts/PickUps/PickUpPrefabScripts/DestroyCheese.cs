using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**************************************************************************************
* Type: (Class)
* 
* Name: Destroycheese.cs
* 
* 
*
* Author: Joseph Gilmore
*
* Description: Atteched to the cheese item pick up to control its actions.
*
* Change Log:
* 
* 
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 25/06/2021    JG        1.00          -Initial Created
*  29/06/2021   JG        1.01          -added a way to  alter health depending who picks it up. Needs to add all racers   
*  13/07/2021   JG        1.02          -added serializefield and object pooling
*  04/08/2021   JG        1.02          -added cheese sound

* **************************************************************************************/
public class DestroyCheese : MonoBehaviour
{
    [Tooltip("The Amount of health the Increase from picking up the cheese. There is only a 4 sizes with each 1 health different between")]
    [SerializeField]
    private int m_healthGain = 1;
    private PickUpPool m_prefabPool;
    private List<string> m_racerNames = new List<string>() { "Player", "AIOne", "AITwo", "AIThree", "AIFour", "AIFive" };
    private void Start()
    {
        m_prefabPool = FindObjectOfType<PickUpPool>();
    }
/**************************************************************************************
* Type: (function)
* 
* Name: OnCollisionEnter
* 
* 
*
* Author: Joseph
*
* Description: used when racer hits thrown pick up
*
* Change Log:
* 
* 
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 25/06/2021    JG        1.00        -Initial Created
* 01/08/2021    JG        1.01        -removed static call of health system 
* 04/08/2021    JG        1.02        -added cheese sound
* 04/08/2021    JG        1.03        -replaced  healthsystem tag with Game Manager
* **************************************************************************************/
    private void OnCollisionEnter(Collision p_other)
    {
        //checks tag from list 
        if (m_racerNames.Contains(p_other.collider.tag))
        {
            
             p_other.transform.Find("CheeseSound").GetComponent<AudioSource>().Play();
            //updates racer health
            GameObject.FindGameObjectWithTag("GameManager").GetComponentInParent<HealthSystem>().UpdateRacersHealth(p_other.collider.tag, m_healthGain);
            
            //deactivtes object and returns to the pool
            m_prefabPool.ReturnGameObject(gameObject);
           
        }
       
    }
}
