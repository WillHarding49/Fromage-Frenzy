using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
* Type: Class
* 
* Name: UIToggleParent
*
* Author: Will Harding
*
* Description: Parent class which has ui toggel function
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 09/08/2021    WH          1.00        -Initally created
****************************************************************************************************/
public class UIToggleParent : MonoBehaviour
{

    /**************************************************************************************
    * Type: Function
    * 
    * Name: ToggleActive
    * Parameters: GameObject p_container
    *             List<string> p_inactive, 
    *             string p_active
    *
    * Author: Will Harding
    *
    * Description: Toggles the active of multiple objects in the same container
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 09/08/2021    WH          1.00        -Initial Created
    **************************************************************************************/
    protected void ToggleActive(GameObject p_container, List<string> p_inactive, string p_active)
    {
        //Loop through all the objects in the inactive list and deactivate them
        foreach (string name in p_inactive)
        {
            p_container.transform.Find(name).gameObject.SetActive(false);
        }

        //Activate the given object to activate
        p_container.transform.Find(p_active).gameObject.SetActive(true);
    }
}
