using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/****************************************************************************************************
* Type: Class
* 
* Name: CheeseMovement
*
* Author: Joseph Gilmore
*
* Description: Prefab for melted slow cheese
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 12/07/2021    JG          1.01        -current know bug for AI. Will potential refer to the wrong racer. This will need chnaging when AI is added.
* 14/07/2021    JG          1.02        -added list for racers to make things cleaner. ai bu still here
****************************************************************************************************/

public class MeltedCheesePrefab : MonoBehaviour
{
    private PickUpPool m_prefabPool;
    private List<string> m_racerNames = new List<string>() { "Player", "AIOne", "AITwo", "AIThree", "AIFour", "AIFive" };//list of racer  
    private void Start()
    {
        //getting pick up pool
        m_prefabPool = FindObjectOfType<PickUpPool>();
    }
/****************************************************************************************************
* Type: function
* 
* Name: OnCollisionEnter
*
* Author: Joseph Gilmore
*
* Description: when a racer enters the colider for melted cheese 
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 12/07/2021    JG          1.00       -created
* 27/07/2021    JG          1.01       -melted cheese hit no longer static 
* 04/08/2021    JG          1.03       -added sound 
****************************************************************************************************/
    private void OnCollisionEnter(Collision p_other)
    {
        if(m_racerNames.Contains(p_other.collider.tag))
        {
            //altres speeds in the racers movement code 
            p_other.gameObject.GetComponentInParent<CheeseMovement>().MeltedCheeseHit();
            transform.GetComponent<AudioSource>().Play();
        }
       
    }
 /****************************************************************************************************
* Type: function
* 
* Name: OnCollisionExit
*
* Author: Joseph Gilmore
*
* Description: when a racer exits the colider for melted cheese 
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 12/07/2021    JG          1.00       -created
* 27/07/2021    JG          1.01       -melted cheese hit no longer static 
* 04/08/2021    JG          1.02       -added sound
****************************************************************************************************/
    private void OnCollisionExit(Collision p_other)
    {
        if (m_racerNames.Contains(p_other.collider.tag))
        {
            //resets changes are leaving collisons
            transform.GetComponent<AudioSource>().Stop();
            p_other.gameObject.GetComponentInParent<CheeseMovement>().MeltedCheeseExit();
            //deactivtes object and returns to the pool
            m_prefabPool.ReturnGameObject(gameObject);
           
        }

    }
}
