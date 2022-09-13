using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/****************************************************************************************************
* Type: Class
* 
* Name: PathDraw
*
* Author: Will Harding
*
* Description: Make a path for the cheese to follow
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 01/07/2021    WH          1.00        -Initial Created, shows nodes and path between them
* 03/07/2021    WH          1.01        -Added basic curve that is purley visual
* 05/07/2021    WH          1.02        -Improved curve code to use math, will be used for pathing
*                                        later for cheese movement
* 06/07/2021    WH          1.10        -Improed how the path displays to make it easier for curves
* 07/07/2021    WH          1.11        -Made class inherit from a base path parent class
* 09/07/2021    WH          1.12        -Cleaned loops and added functions to make it cleaner
* 11/07/2021    WH          1.13        -Added closed loop toggle
* 20/07/2021    WH          1.14        -Added correct naming conventions
* 12/08/2021    WH          1.15        -Set a default colour because it was annoying me it wasn't done
* 18/08/2021    WH          1.16        -Added tooltips and if unity editor check so builds work
****************************************************************************************************/
public class PathDraw : PathParent
{
    [Tooltip("Colour of the path")]
    public Color m_lineColour = Color.black;

    [Tooltip("Determines if the track should loop")]
    public bool m_loop = true;

    private bool m_curvePause = false;

    /**************************************************************************************
    * Type: Function
    * 
    * Name: DrawCurve
    * Parameters: Vector3 p_currentNode, p_nextNode, p_startPoint, p_endPoint, 
    *                     p_startTangent, p_endTangent
    *
    * Author: Will Harding
    *
    * Description: Draws the curve of the path in the scene
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 09/07/2021    WH          1.00        -Initial Created, draws curves in function
    *                                        instead of messily in the main function
    **************************************************************************************/
    void DrawCurve(Vector3 p_currentNode, Vector3 p_nextNode, Vector3 p_startPoint, Vector3 p_endPoint, Vector3 p_startTangent, Vector3 p_endTangent)
    {
        #if UNITY_EDITOR
        //Draws curve using points given
        Handles.DrawBezier(p_startPoint, p_endPoint, p_startTangent, p_endTangent, Color.black, null, 3f);

        ////Draws spheres at the start node and 2 handles
        Gizmos.DrawWireSphere(p_currentNode, 0.5f);
        Gizmos.DrawWireSphere(p_startTangent, 0.5f);
        Gizmos.DrawWireSphere(p_endTangent, 0.5f);

        ////Draws lines for handles
        Gizmos.DrawLine(p_currentNode, p_nextNode);
        Gizmos.DrawLine(p_endTangent, p_endPoint);
        #endif
    }


    /**************************************************************************************
    * Type: Function
    * 
    * Name: OnDrawGizmos
    *
    * Author: Will Harding
    *
    * Description: Draws the path using Unity's gizmo drawing function
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 01/07/2021    WH          1.00        -Initally created, shows nodes and path 
    *                                        between them
    * 03/07/2021    WH          1.01        -Added basic curve that is purley visual
    * 05/07/2021    WH          1.02        -Improved curve code to use math
    * 06/07/2021    WH          1.10        -Improed how the path displays to make it 
    *                                        easier for curves
    * 07/07/2021    WH          1.11        -Added inherited functions and cleaned                                       
    * 09/07/2021    WH          1.12        -Cleaned more with DrawCurve and added node 0
    *                                        curve end functionality
    * 11/07/2021    WH          1.13        -Added toggleable closed loop fuctionality
    * 12/07/2021    WH          1.20        -Added lane loop stuff
    **************************************************************************************/
    public void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        Gizmos.color = m_lineColour;

        //Makes list of nodes in path
        MakeList(transform);

        for (int i = 0; i < m_nodes.Length; i++)
        {
            for (int j = 0; j < m_nodes[i].Length; j++)
            {
                Vector3 currentNode = m_nodes[i][j].position;
                Vector3 nextNode;

                if (i + 1 < m_nodes.Length)
                {
                    nextNode = m_nodes[i + 1][j].position;
                }

                //If at the final node and the path is a closed loop
                else if (i + 1 >= m_nodes.Length && m_loop == true)
                {
                    nextNode = m_nodes[0][j].position;
                }

                //If at the final node and the path is not a closed loop
                else
                {
                    nextNode = m_nodes[i][j].position;
                }

                //if 3 m_nodes ahead is a curve end
                if (i + 3 < m_nodes.Length && m_nodes[i + 3][j].parent.CompareTag("CurveEnd"))
                {
                    DrawCurve(currentNode, nextNode, m_nodes[i][j].position, m_nodes[i + 3][j].position, m_nodes[i + 1][j].position, m_nodes[i + 2][j].position);

                    m_curvePause = true;
                    //i += 2;
                }

                //If 3 nodes before the end and the first node is a curve end and the path is a closed loop
                else if (i == m_nodes.Length - 3 && m_nodes[0][j].parent.CompareTag("CurveEnd") && m_loop == true)
                {
                    DrawCurve(currentNode, nextNode, m_nodes[i][j].position, m_nodes[0][j].position, m_nodes[i + 1][j].position, m_nodes[i + 2][j].position);

                    m_curvePause = true;
                    //i += 2;
                }

                //If any other node
                else if (m_curvePause == false)
                {
                    Gizmos.DrawLine(currentNode, nextNode);
                    Gizmos.DrawWireSphere(currentNode, 0.5f);
                }

                m_nodes[i][j].transform.LookAt(nextNode);

                if (m_curvePause == true && j == m_laneAmount - 1)
                {
                    m_curvePause = false;
                    i += 2;
                }

            }
        }
        #endif

    }
}