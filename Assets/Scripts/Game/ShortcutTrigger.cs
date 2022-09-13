using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/****************************************************************************************************
* Type: Class
* 
* Name: TriggerBox
*
* Author: Will Harding
*
* Description: Class for trigger box to trigger functions on enter
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 19/07/2021    WH          1.00        -Initally created
* 20/07/2021    WH          1.01        -Added correct naming conventions
* 18/08/2021    WH          1.02        -Added tooltips
****************************************************************************************************/
public class ShortcutTrigger : MonoBehaviour
{
    [Tooltip("How many lanes are there beyond this trigger")]
    public int m_newLaneAmount;

    [Tooltip("The leftmost lane beyond this trigger, the lane with the smallest number")]
    public int m_newLaneMin;


    /**************************************************************************************
    * Type: Function
    * 
    * Name: OnTriggerEnter
    * Parameters: Collider p_other
    *
    * Author: Will Harding
    *
    * Description: Changes what lanes the racer can switch betweeen
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 19/07/2021    WH          1.00        -Initial Created
    * 23/07/2021    WH          1.10        -Added AI support
    * 28/07/2021    WH          1.20        -Reforatted to have 1 if rather than 2 using
    *                                        parent class of racers
    **************************************************************************************/
    public void OnTriggerEnter(Collider p_other)
    {
        if (p_other.CompareTag("Player") || p_other.tag.Contains("AI"))
        {
            p_other.transform.parent.gameObject.GetComponent<CheeseMovement>().ChangeLaneNo(m_newLaneAmount, m_newLaneMin);
        }
    }

}
