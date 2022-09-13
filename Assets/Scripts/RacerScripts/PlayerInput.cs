using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**************************************************************************************
* Type: (Class)
* 
* Name: PlayerInput.cs
* 
* 
*
* Author: Joseph Gilmore
*
* Description: Child of cheese movement. Controls Inputs 
*
* Change Log:
* 
* 
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 19/07/2021    JG          1.00        -Initial Created
* 21/07/2021    JG          1.10        -changed parent to updated script 
* 02/08/2021    WH          1.11        -Added speed controls
* 03/08/2021    WH          1.12        -Made it so cheese defults to normal speed when
*                                        not manually changing speed
* 10/08/2021    WH          1.13        -Added check if race is ended so you can't move
*                                        still if true
* 12/08/2021    WH          1.14        -Added check so you can't move if the race hasn't
*                                        started yet
* **************************************************************************************/
public class PlayerInput : CheeseMovement
{
    
    void Start()
    {
        base.Start();
    }

    
    void Update()
    {
        base.Update();

        //While the race is happening and not finished.
        if (m_raceManager.GetComponent<RaceManager>().m_racing && !m_finished)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                MoveRight();
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                MoveLeft();
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                ChangeMaxSpeed(1.1f);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                ChangeMaxSpeed(0.9f);
            }

            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
            {
                ChangeMaxSpeed(1f);
            }
        }

        //When the player has finished the race, display the leaderboard
        else if (m_finished)
        {
            m_raceManager.GetComponent<RaceManager>().DisplayLeaderboard();
        }
    }
   
}

