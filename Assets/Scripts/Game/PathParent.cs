using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************************************
* Type: Class
* 
* Name: PathParent
*
* Author: Will Harding
*
* Description: Parent class containing important functions for path movement and creation
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 06/07/2021    WH          1.00        -Initally created, has functions for child classes
* 08/07/2021    WH          1.01        -Fixed a bug in MakeList
* 20/07/2021    WH          1.10        -Added correct naming conventions
* 18/08/2021    WH          1.11        -Added tooltips
****************************************************************************************************/
public class PathParent : MonoBehaviour
{
    public Transform[][] m_nodes;

    [HideInInspector]
    public int m_pathLength;

    [Tooltip("Number of lanes in the track")]
    public int m_laneAmount = 5;

    /**************************************************************************************
    * Type: Function
    * 
    * Name: CubicBezier
    * Parameters: Vector3 p_point0, p_point1, p_point2, p_point3
    *             float t
    * Return: Vector3 Cubic Bezier position 
    *
    * Author: Will Harding
    *
    * Description: Gives position of point on cubic bezier curve given the parameters
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 06/07/2021    WH          1.00        -Initial Created
    **************************************************************************************/
    protected Vector3 CubicBezier(Vector3 p_point0, Vector3 p_point1, Vector3 p_point2, Vector3 p_point3, float t)
    {
        //Forces 0 <= t <= 1
        t = Mathf.Clamp01(t);
        //Substitution for 1-t as it's repeated many times and it's cleaner for the math
        float tSub = 1f - t;
        return Mathf.Pow(tSub, 3f) * p_point0 + 3f * Mathf.Pow(tSub, 2f) * t * p_point1 + 3f * tSub * Mathf.Pow(t, 2f) * p_point2 + Mathf.Pow(t, 3f) * p_point3;
    }


    /**************************************************************************************
    * Type: Function
    * 
    * Name: CubicBezierDer1
    * Parameters: Vector3 p_point0, p_point1, p_point2, p_point3
    *             float t
    * Return: Vector3 Cubic Bezier derivitive position 
    *
    * Author: Will Harding
    *
    * Description: Gets velocity at point on cubic bezier using it's 1st derivative
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 06/07/2021    WH          1.00        -Initial Created
    **************************************************************************************/
    protected Vector3 CubicBezierDer1(Vector3 p_point0, Vector3 p_point1, Vector3 p_point2, Vector3 p_point3, float t)
    {
        t = Mathf.Clamp01(t);
        float tSub = 1f - t;
        return 3f * Mathf.Pow(tSub, 2f) * (p_point1 - p_point0) + 6f * tSub * t * (p_point2 - p_point1) + 3f * Mathf.Pow(t, 2f) * (p_point3 - p_point2);
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: MakeList
    * Parameters: Transform path
    *
    * Author: Will Harding
    *
    * Description: Makes a list of all the nodes in the path
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 06/07/2021    WH          1.00        -Initial Created
    * 08/07/2021    WH          1.01        -Changed the loop so it used the parameter
    *                                        like it should have orignally
    **************************************************************************************/
    protected void MakeList(Transform p_path)
    {
        m_pathLength = p_path.childCount;
        m_nodes = new Transform[m_pathLength][];

        //Loops through and appends path nodes to list
        for (int i = 0; i < m_pathLength; i++)
        {
            m_nodes[i] = new Transform[m_laneAmount];

            for (int j = 0; j < m_laneAmount; j++)
            {
                m_nodes[i][j] = p_path.GetChild(i).GetChild(j);
            }
        }
    }
}
