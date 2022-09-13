using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
* Type: Class
* 
* Name: MoveMinimapIcons
*
* Author: Will Harding
*
* Description: Moves minimap icon so it moves in line with the racer and doesn't rotate around it
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 15/08/2021    WH          1.00        -Initally created
****************************************************************************************************/
public class MoveMinimapIcons : MonoBehaviour
{
    private Transform m_racer;

    [Tooltip("The offset from the racer to where the minimap icon will be")]
    public float m_offset = 30f;


    /**************************************************************************************
    * Type: Function
    * 
    * Name: Start
    *
    * Author: Will Harding
    *
    * Description: Gets racer parent
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 15/08/2021    WH          1.00        -Initial Created
    **************************************************************************************/
    void Start()
    {
        m_racer = transform.parent.transform;
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: Update
    *
    * Author: Will Harding
    *
    * Description: Moves minimap icon to be in line with the player but above them
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 15/08/2021    WH          1.00        -Initial Created
    **************************************************************************************/
    void Update()
    {
        transform.position = new Vector3(m_racer.position.x, m_racer.position.y + m_offset, m_racer.position.z);
    }
}
