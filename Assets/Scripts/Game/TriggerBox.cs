using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
/**************************************************************************************
* Type: (Class)
* 
* Name: TriggerBox.cs
* 
* 
*
* Author: Joseph Gilmore
*
* Description: Used as a pickUp  box 
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
*  23/06/2021   JG          1.00        -Initial Created
*  24/06/2021   JG          1.01        -added switch statment so it could work for single AI. (needs altering for pickups)
*  14/07/2021   JG          1.02        -Made it so all Pickups use one script and should for AI so extra scripts now been deleted. (also added list)
*  23/07/2021   JG          1.03        -removed this.
*  10/08/2021   JG          1.04         -changed script reference of pickups  as they are now children of parent pick up and named differenlty 
*  18/08/2021   JG          1.05        -removed switch statement to check which pickup the parent is. This is because they are now inhereited so the reference is the same for all pickups
**************************************************************************************/
public class TriggerBox : MonoBehaviour
{
    
    private List<string> m_racerNames = new List<string>() { "Player", "AIOne", "AITwo", "AIThree", "AIFour", "AIFive" }; //list of racers that can pick up box
 
    private void OnTriggerEnter(Collider p_other)
    {
       
        if (m_racerNames.Contains(p_other.tag))
        {
            ///tells the pickup a racer has it
            transform.gameObject.GetComponentInParent<ParentPickUp>().PlayerPickedUp(p_other.tag);
        }
        
    }
}
