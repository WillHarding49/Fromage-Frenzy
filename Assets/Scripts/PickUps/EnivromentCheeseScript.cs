using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**************************************************************************************
* Type: (Class)
* 
* Name: EnviromentCheese.cs
* 
* 
*
* Author: Joseph Gilmore
*
* Description: slightly different than the destroy cheese class,
*
* Change Log:
* 
* 
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 25/06/2021    JG          1.00        -Initial Created
* 29/06/2021    JG          1.01        -added a way to  alter health depending who picks it up. Needs to add all racers   
  13/07/2021    JG          1.02        -added serializefield and object pooling
* 27/07/2021    JG          1.03        -lane checker for AI
* 04/08/2021    JG          1.04        -added cheese sound & game manager tag
* **************************************************************************************/
public class EnivromentCheeseScript : MonoBehaviour
{
    [Tooltip("The Amount of health you gain from picking up the cheese")]
    [SerializeField]
    private int m_healthGain = 1; //health change for when a racer collides 
    private List<string> m_racerNames = new List<string>() { "Player", "AIOne", "AITwo", "AIThree", "AIFour", "AIFive" };
    [Tooltip("Please put the lane of which you put the pick up in")]
    public float m_lane;
    
    
    /**************************************************************************************
    * Type: (Function)
    * 
    * Name: OnCollisionEnter
    * 
    * 
    *
    * Author: Joseph Gilmore
    *
    * Description: for when the player runs over eniviroment cheese 
    *
    * Change Log:
    * 
    * 
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 25/06/2021   JG        1.00           -Initial Created
    * 01/08/2021   JG        1.01           -removed static call of healthSystem
    * 04/08/2021   JG        1.02           -added cheese sound & updated tag to Game Manager 
    * **************************************************************************************/
    private void OnCollisionEnter(Collision p_other)
    {
        //checks tag from list 
        if (m_racerNames.Contains(p_other.collider.tag))
        {
            //PLAY cheese sound
            p_other.transform.Find("CheeseSound").GetComponent<AudioSource>().Play();
            //get health system and updates racers health 
            GameObject.FindGameObjectWithTag("GameManager").GetComponentInParent<HealthSystem>().UpdateRacersHealth(p_other.collider.tag, m_healthGain);
            //deactivtes object (need to make active after laps)
            transform.gameObject.SetActive(false);

        }

    }
}
