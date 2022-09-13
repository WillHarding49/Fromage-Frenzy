using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;

/****************************************************************************************************
* Type: Class
* 
* Name: PositionTracker
*
* Author: Will Harding
*
* Description: Tracks the position of each character during the race, used for rubberbanding
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 21/07/2021    WH          1.00        -Initally created
* 23/07/2021    WH          1.01        -Sorts dictionary without debug
* 26/07/2021    WH          1.10        -Added AI debug text
* 27/07/2021    WH          1.20        -Added rubberbanding
* 28/07/2021    WH          1.21        -Reformatted so there aren't different cases for player and
*                                        AI racers, they now both use the parent class
* 30/07/2021    WH          1.22        -Added comments
* 01/08/2021    WH          1.23        -Fixed that horrid double loop in the rubberbanding
* 18/08/2021    WH          1.24        -Added tooltips and made rubberbanding variables
****************************************************************************************************/
public class PositionTracker : MonoBehaviour
{
    public Dictionary<GameObject, float[]> m_racers = new Dictionary<GameObject, float[]>();

    private int m_pathLength;

    [Tooltip("Canvas text to display debug info")]
    public Text m_canvasText;

    private string[] m_tags = { "Player", "AIOne", "AITwo", "AIThree", "AIFour", "AIFive" };

    private int m_positionToReturn = 1;
    
    [Tooltip("Max speed for rubberbanding, should be the highest value of them all.")]
    public float m_maxSpeedMul = 1.3f;

    [Tooltip("2nd fastest spped for rubberbanding, should be between the max and fast speeds.")]
    public float m_fasterSpeedMul = 1.2f;

    [Tooltip("3nd fastest spped for rubberbanding, should be between default and faster speeds.")]
    public float m_fastSpeedMul = 1.1f;

    [Tooltip("Default speed for rubberbanding. Should be between fast and slow speeds.")]
    public float m_defaultSpeedMul = 1f;

    [Tooltip("3nd slowest spped for rubberbanding, should be between default and slower speeds.")]
    public float m_slowSpeedMul = 0.9f;

    [Tooltip("2nd slowest spped for rubberbanding, should be between the min and slow speeds.")]
    public float m_slowerSpeedMul = 0.8f;

    [Tooltip("Min speed for rubberbanding, should be the smallest value of them all.")]
    public float m_minSpeedMul = 0.7f;
    


    /**************************************************************************************
    * Type: Function
    * 
    * Name: Start
    *
    * Author: Will Harding
    *
    * Description: Finds all the racers in the scene
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 21/07/2021    WH          1.00        -Initial Created, calculates psotions of racers
    * 22/07/2021    WH          1.01        -Removed everything and made a seperate function
    * 27/07/2021    WH          1.10        -Gets path length
    **************************************************************************************/
    void Start()
    {
        m_pathLength = GameObject.FindWithTag("Path").GetComponent<PathDraw>().m_pathLength;
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: CalculatePostition
    *
    * Author: Will Harding
    *
    * Description: Calculates the position of each racer and orders them in a dictionary
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 21/07/2021    WH          1.00        -Initial Created 
    * 23/07/2021    WH          1.01        -Sorts dictionay without display
    * 28/07/2021    WH          1.10        -Reformatted to not have seperate cases for 
    *                                        the player and AI
    **************************************************************************************/
    void CalculatePostition()
    {
        //For all of the racers
        for (int i = 0; i < m_tags.Length; i++)
        {
            //Get racer
            GameObject racer = GameObject.FindWithTag(m_tags[i]);

            if (racer != null)
            {
                //Get the info from cheese movement
                float lap = racer.transform.parent.GetComponent<CheeseMovement>().m_lap;
                float node = racer.transform.parent.GetComponent<CheeseMovement>().m_node;
                float dist = racer.transform.parent.GetComponent<CheeseMovement>().GetDistToNode();

                //Make array of all values needed
                float[] racerValues = { lap, node, dist };

                //Add or update value in dictionary
                if (m_racers.ContainsKey(racer))
                {
                    m_racers[racer] = racerValues;
                }
                else
                {
                    m_racers.Add(racer, racerValues);
                }
            }
        }

        //Order dictionary by the lap number, then by the node number, then by the distance to the next node
        m_racers.OrderByDescending(i => i.Value[0]).ThenByDescending(j => j.Value[1]).ThenBy(k => k.Value[2]);
    }



    /**************************************************************************************
    * Type: Function
    * 
    * Name: DisplayPositions
    *
    * Author: Will Harding
    *
    * Description: Displays dictionary info for debug purposes only
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 21/07/2021    WH          1.00        -Initial Created 
    * 26/07/2021    WH          1.10        -Displays text programatically for all racers
    * 28/07/2021    WH          1.11        -Rounded Dist number to 2dp and reformatted so
    *                                        there isn't a different case for the player
    *                                        and AI. Also displays player position.
    * 28/07/2021    WH          1.12        -Added max speed display
    **************************************************************************************/
    private void DisplayPositions()
    {
        foreach (var racer in m_racers)
        {
            if (racer.Key.transform.parent.Find("Text/Lap") != null)
            {
                TextMeshPro lap = racer.Key.transform.parent.Find("Text/Lap").GetComponent<TextMeshPro>();
                lap.text = string.Format("Lap: {0}", racer.Value[0]);
            }

            if (racer.Key.transform.parent.Find("Text/Node") != null)
            {
                TextMeshPro node = racer.Key.transform.parent.Find("Text/Node").GetComponent<TextMeshPro>();
                node.text = string.Format("Node: {0}", racer.Value[1]);
            }
                
            if (racer.Key.transform.parent.Find("Text/Dist") != null)
            {
                TextMeshPro dist = racer.Key.transform.parent.Find("Text/Dist").GetComponent<TextMeshPro>();
                dist.text = string.Format("Dist: {0:N1}", racer.Value[2]);
            }

            if (racer.Key.transform.parent.Find("Text/Pos") != null)
            {
                TextMeshPro position = racer.Key.transform.parent.Find("Text/Pos").GetComponent<TextMeshPro>();
                position.text = string.Format("Pos: {0}", GetPosition(racer.Key));
            }
                
            if (racer.Key.transform.parent.Find("Text/Max Speed") != null)
            {
                TextMeshPro maxSpeed = racer.Key.transform.parent.Find("Text/Max Speed").GetComponent<TextMeshPro>();
                maxSpeed.text = string.Format("MxS:{0}", racer.Key.transform.parent.GetComponent<CheeseMovement>().m_maxSpeed);
            }

            //if (racer.Key.CompareTag("Player") && m_canvasText != null)
            //{
            //    m_canvasText.text = string.Format("Position: {0}", GetPosition(racer.Key));
            //}
        }

    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: Rubberbanding
    *
    * Author: Will Harding
    *
    * Description: Changes speed of racers depending on position of player
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 27/07/2021    WH          1.00        -Initial Created 
    * 28/07/2021    WH          1.01        -Racer now uses parent script
    * 28/07/2021    WH          1.02        -Added distance rubberbanding so there is more
    *                                        than just nodes and laps to rubberband
    * 01/08/2021    WH          1.03        -Got rid of that horrid double loop and now
    *                                        get player pair a bit more cleanly
    * 18/08/2021    WH          1.04        -Changed magic numbers to variables
    **************************************************************************************/
    private void Rubberbanding()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");

        if (m_racers.ContainsKey(playerObject))
        {
            KeyValuePair<GameObject, float[]> player = new KeyValuePair<GameObject, float[]>(playerObject, m_racers[playerObject]);

            foreach (var racer in m_racers)
            {
                //For each AI
                if (!racer.Key.CompareTag("Player"))
                {
                    //If racer is a lap behind player and they are not within the threshold of the finish line
                    if (racer.Value[0] < player.Value[0] && (racer.Value[1] < m_pathLength - 1 && player.Value[1] > 0))
                    {
                        racer.Key.transform.parent.GetComponent<CheeseMovement>().ChangeMaxSpeed(m_maxSpeedMul);
                        //Debug.Log("Speed Change");
                    }

                    //If racer is a node behind player
                    else if (racer.Value[1] < player.Value[1] - 1)
                    {
                        racer.Key.transform.parent.GetComponent<CheeseMovement>().ChangeMaxSpeed(m_fasterSpeedMul);
                        //Debug.Log("Speed Change");
                    }

                    //If racer is a lap ahead player and they are not within the threshold of the finish line
                    else if (racer.Value[0] > player.Value[0] && (player.Value[1] < m_pathLength - 1 && racer.Value[1] > 0))
                    {
                        racer.Key.transform.parent.GetComponent<CheeseMovement>().ChangeMaxSpeed(m_minSpeedMul);
                        //Debug.Log("Speed Change");
                    }

                    //If the racer is a node ahead of the player
                    else if (racer.Value[1] > player.Value[1] + 1)
                    {
                        racer.Key.transform.parent.GetComponent<CheeseMovement>().ChangeMaxSpeed(m_slowerSpeedMul);
                        //Debug.Log("Speed Change");
                    }

                    //If the racer and player are in the same node
                    else if (racer.Value[1] == player.Value[1])
                    {
                        //If the racer is behind the player
                        if (racer.Value[2] > player.Value[2] + 5)
                        {
                            racer.Key.transform.parent.GetComponent<CheeseMovement>().ChangeMaxSpeed(m_fastSpeedMul);
                        }

                        //If the racer is ahead of the player
                        else if (racer.Value[2] < player.Value[2] - 5)
                        {
                            racer.Key.transform.parent.GetComponent<CheeseMovement>().ChangeMaxSpeed(m_slowSpeedMul);
                        }
                    }

                    //Set max speed to be original value
                    else
                    {
                        racer.Key.transform.parent.GetComponent<CheeseMovement>().ChangeMaxSpeed(m_defaultSpeedMul);
                    }
                }
            }
        }

        //If the player has finished, speed up all the racers so they can finish
        else
        {
            foreach (var racer in m_racers)
            {
                racer.Key.transform.parent.GetComponent<CheeseMovement>().ChangeMaxSpeed(m_maxSpeedMul);
            }
        }
    }


    /**************************************************************************************
   * Type: Function
   * 
   * Name: GetPosition
   * Parameters: GameObject p_racer
   * Return: int position
   *
   * Author: Will Harding
   *
   * Description: Returns what position the racer is in the race
   * 
   *
   * Change Log:
   * Date          Initials    Version     Comments
   * ----------    --------    -------     ----------------------------------------------
   * 28/07/2021    WH          1.00        -Initial Created
   **************************************************************************************/
    public int GetPosition(GameObject p_racer)
    {
        int position = m_positionToReturn;

        //Loops through dictionary to find the given racer, increasing the position int if it isn't found
        foreach (var racer in m_racers.OrderByDescending(i => i.Value[0]).ThenByDescending(j => j.Value[1]).ThenBy(k => k.Value[2]))
        {
            if (racer.Key == p_racer)
            {
                break;
            }
            else
            {
                position++;
            }
        }
        return position;
    }


    /**************************************************************************************
    * Type: Function
    * 
    * Name: LeaderboardPop
    * Return: int racer
    *
    * Author: Will Harding
    *
    * Description: Returns the racer in 1st and removes them from the dictionary and tag list
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 04/08/2021    WH          1.00        -Initial Created
    **************************************************************************************/
    public GameObject LeaderboardPop()
    {
        //Get 1st racer in the race
        GameObject racer = m_racers.OrderByDescending(i => i.Value[0]).ThenByDescending(j => j.Value[1]).ThenBy(k => k.Value[2]).First().Key;
        
        //Remove the racer from the list of tags and the dictionary
        m_tags = m_tags.Where(tag => tag != racer.tag).ToArray();
        m_racers.Remove(racer);

        m_positionToReturn ++;

        //Return the racer
        return racer;
        
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: Update
    *
    * Author: Will Harding
    *
    * Description: Runs the calculating functions to update them 
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 21/07/2021    WH          1.00        -Initial Created 
    **************************************************************************************/
    private void Update()
    {
        CalculatePostition();
        DisplayPositions();
        Rubberbanding();
    }
}
