using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/****************************************************************************************************
* Type: Class
* 
* Name: UIController
*
* Author: Will Harding
*
* Description: Displays correct info on the UI
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 30/07/2021    WH          1.00        -Initally created, added player, position, and health gets
* 04/08/2021    WH          1.01        -Cleaned some stuff
* 09/08/2021    WH          2.00        -Changed to use assets and now derives from UIToggleParent
* 11/08/2021    WH          2.01        -Added item and lockon UI stuff
* 18/08/2021    WH          2.02        -Added tooltips
* 19/08/2021    WH          2.03        -Added comments
****************************************************************************************************/
public class UIController : UIToggleParent
{
    private PositionTracker m_positionTracker;
    private HealthSystem m_healthSystem;

    private int m_lapTotal;

    private int m_position;

    private int m_health;

    [HideInInspector]
    public string m_item;

    [HideInInspector]
    public bool m_lockedOn;

    private GameObject m_player;

    [Tooltip("Healthbar group from canvas containing the different healthbar values.")]
    public GameObject m_healthbar;

    [Tooltip("Lap counter object from the position object from the canvas.")]
    public GameObject m_lapCounter;

    [Tooltip("Position object from the canvas containing the position numbers.")]
    public GameObject m_positionNumber;

    [Tooltip("Items object from canvas containing the icons for what pickup you have.")]
    public GameObject m_pickUpIcon;

    [Tooltip("LockedOn object from canvas to display if your spear is locked on or not.")]
    public GameObject m_lockon;


    /**************************************************************************************
    * Type: Function
    * 
    * Name: Start
    *
    * Author: Will Harding
    *
    * Description: Gets scripts to get info to display on UI
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 30/07/2021    WH          1.00        -Initial Created, gets player, position, and
    *                                        health system
    * 02/08/2021    WH          1.01        -Cleaned position tracker object get
    * 04/08/2021    WH          1.02        -Changed tags for the managers
    **************************************************************************************/
    void Start()
    {
        m_player = GameObject.FindWithTag("Player");

        m_positionTracker = GameObject.FindWithTag("GameManager").GetComponent<PositionTracker>();
        m_healthSystem = GameObject.FindWithTag("GameManager").GetComponent<HealthSystem>();

        m_lapTotal = GameObject.FindWithTag("GameManager").GetComponent<RaceManager>().m_lapAmount;
    }



    /**************************************************************************************
    * Type: Function
    * 
    * Name: HealthUpdate
    *
    * Author: Will Harding
    *
    * Description: Updates healthbar UI
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 09/08/2021    WH          1.00        -Initial Created
    **************************************************************************************/
    private void HealthUpdate()
    {
        List<string> bars = new List<string> { "1", "2", "3", "4" };

        //Get the player health
        m_health = m_healthSystem.m_playerHealth;

        //Remove player health from the list
        bars.Remove(m_health.ToString());

        //Turn off all other health icons and turn on the player health one
        switch (m_health)
        {
            case 4:
            default:
                ToggleActive(m_healthbar, bars, "4");
                break;

            case 3:
                ToggleActive(m_healthbar, bars, "3");
                break;

            case 2:
                ToggleActive(m_healthbar, bars, "2");
                break;

            case 1:
                ToggleActive(m_healthbar, bars, "1");
                break;
        }
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: LapUpdate
    *
    * Author: Will Harding
    *
    * Description: Updates Lap count UI
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 09/08/2021    WH          1.00        -Initial Created
    **************************************************************************************/
    private void LapUpdate()
    {
        int lapNo = m_player.GetComponentInParent<CheeseMovement>().m_lap;

        //Sets lap text
        m_lapCounter.transform.Find("Text").GetComponent<Text>().text = "Lap " + lapNo + "/" + m_lapTotal;
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: PositionUpdate
    *
    * Author: Will Harding
    *
    * Description: Updates Position number UI
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 09/08/2021    WH          1.00        -Initial Created
    **************************************************************************************/
    private void PositionUpdate()
    {
        List<string> numbers = new List<string> { "1", "2", "3", "4", "5", "6" };

        //Get player's postiton
        m_position = m_positionTracker.GetPosition(m_player);

        //Remove posiiton from list
        numbers.Remove(m_position.ToString());

        //Turn off all other position icons and turn on the current position
        switch (m_position)
        {
            case 6:
            default:
                ToggleActive(m_positionNumber, numbers, "6");
                break;

            case 5:
                ToggleActive(m_positionNumber, numbers, "5");
                break;

            case 4:
                ToggleActive(m_positionNumber, numbers, "4");
                break;

            case 3:
                ToggleActive(m_positionNumber, numbers, "3");
                break;

            case 2:
                ToggleActive(m_positionNumber, numbers, "2");
                break;

            case 1:
                ToggleActive(m_positionNumber, numbers, "1");
                break;
        }
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: ItemUpdate
    *
    * Author: Will Harding
    *
    * Description: Updates Position number UI
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 11/08/2021    WH          1.00        -Initial Created
    **************************************************************************************/
    private void ItemUpdate()
    {
        List<string> items = new List<string> { "Cheese", "Spear", "MeltedCheese" };

        //m_item = p_pickUpName;
        items.Remove(m_item.ToString());

        //Sets correct item in icon
        switch (m_item)
        {
            case "":
            default:
                ToggleActive(m_pickUpIcon, items, "Icon");
                break;

            case "Cheese":
                ToggleActive(m_pickUpIcon, items, "Cheese");
                break;

            case "Spear":
                ToggleActive(m_pickUpIcon, items, "Spear");
                break;

            case "MeltedCheese":
                ToggleActive(m_pickUpIcon, items, "MeltedCheese");
                break;
        }
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: LockOn
    *
    * Author: Will Harding
    *
    * Description: Toggles locked on UI for spear
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 11/08/2021    WH          1.00        -Initial Created
    * 12/08/2021    WH          1.01        -Simplified
    **************************************************************************************/
    private void LockOn()
    {
        m_lockon.SetActive(m_lockedOn);
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: Update
    *
    * Author: Will Harding
    *
    * Description: Displays correct UI info
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 30/07/2021    WH          1.00        -Initial Created, assigns lap, position, and
    *                                        health on text while UI assets are finalised
    * 01/08/2021    WH          1.01        -Fixed health so it uses non static variable
    * 09/08/2021    WH          2.00        -Now using assets and not text
    * 11/08/2021    WH          2.01        -Added item and lockon updates
    **************************************************************************************/
    void Update()
    {
        HealthUpdate();
        LapUpdate();
        PositionUpdate();
        ItemUpdate();
        LockOn();
    }
}
