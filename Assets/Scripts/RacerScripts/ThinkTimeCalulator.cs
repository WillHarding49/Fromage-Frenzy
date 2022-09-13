using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
/**************************************************************************************
* Type: (Class)
* 
* Name: ThinkTime.cs
* 
* 
*
* Author: Joseph Gilmore
*
* Description: used to calculate the think time of each AI and select cheese behaviour depening on size 
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 20/07/2021    JG          1.00        -Initial Created
* 21/07/2021    JG          1.01        -Thnk time now is generated randomly and each AI has a different think time witin a randomly generated differenc and kept within a bound. needs refinement of concept from designer 
* 02/08/2021    JG          1.02        -randomise think time number aswell as think time difference due to a rare bug 
* **************************************************************************************/
public class ThinkTimeCalulator : MonoBehaviour
{
    #region Vars
    [Tooltip("AI Think Time, reccommend leaving blank to allow the think time calulator to work")]
    public float m_aiOneThinkTime; //Each assignable AI Think Time. 
    [Tooltip("AI Think Time, reccommend leaving blank to allow the think time calulator to work")]
    public float m_aiTwoThinkTime;
    [Tooltip("AI Think Time, reccommend leaving blank to allow the think time calulator to work")]
    public float m_aiThreeThinkTime;
    [Tooltip("AI Think Time, reccommend leaving blank to allow the think time calulator to work")]
    public float m_aiFourThinkTime;
    [Tooltip("AI Think Time, reccommend leaving blank to allow the think time calulator to work")]
    public float m_aiFiveThinkTime;
    private float m_thinkNumber;
    [SerializeField][Tooltip("The amount of time the AI takes to make a decision")][Range(5,10)]
    private  float m_maxThinkTime = 2f; //the max lenght of time it takes for an AI to make a decision
    [SerializeField][Tooltip("The minimal amount of the time it takes for the AI to make a decision")][Range(1,2)]
    private float m_minThinkTime; // the minimal time it take the AI to make a decision
    private float m_thinkTimeDifference; // used to make each AI think time a certain amount different. 
    private float m_thinkTimeDifferenceMax = 2f / 6f;
    private float m_thinkTimeDifferenceMin = 0.08f;
    public  string[] m_assignedBehaviours = new string[5];
    private List<string> m_setBehaviour = new List<string>(){ "Small","Small","Big", "Big", "Aggreeseive","Aggreeseive" };
    private bool m_timeDifferenceInBounds = false;
    #endregion
    void Start()
    {
       //random think time number within bounds
        m_thinkNumber = Random.Range(m_minThinkTime, m_maxThinkTime);
        //random number to calculate the difference in think time for each AI
        m_thinkTimeDifference = Random.Range(m_thinkTimeDifferenceMin, m_thinkTimeDifferenceMax);
        AssignBehaviour();
    }

    // Update is called once per frame
    void Update()
    {
        //checks if the think times difference when applyied will keep the AI think time within the bounds
        if ((m_thinkTimeDifference * 2 )+ m_thinkNumber >=  m_maxThinkTime && m_timeDifferenceInBounds == false)
        {
            //get a new think time difference if its too big
            m_thinkTimeDifference = Random.Range(m_thinkTimeDifferenceMin, m_thinkTimeDifferenceMax);
            //also get a new think time as a rare bug can casue it to be too big.
            m_thinkNumber = Random.Range(m_minThinkTime, m_maxThinkTime);

        }
        if((m_thinkTimeDifference * 2) + m_thinkNumber < m_maxThinkTime && m_timeDifferenceInBounds == false)
        {
            //think time difference is okay dont randomise 
            m_timeDifferenceInBounds = true;
            AssignThinkTime();
        }   
      
    }
 /**************************************************************************************
* Type: (function)
* 
* Name: AssignThinkTime
* 
* 
*
* Author: Joseph Gilmore
*
* Description: used to asign think time to AI randomly
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 21/07/2021    JG         1.01        - currenlty just assgins into an array. need to assgin to each AI
* 27/07/2021    JG         1.02         - each AI assigned a think time . removed array
* **************************************************************************************/
    private void AssignThinkTime()
    {
        m_aiOneThinkTime = m_thinkNumber;
        m_aiTwoThinkTime = m_thinkNumber + m_thinkTimeDifference;
        m_aiThreeThinkTime= m_thinkNumber + (m_thinkTimeDifference) * 2;
        m_aiFourThinkTime = m_thinkNumber - m_thinkTimeDifference;
        m_aiFiveThinkTime = m_thinkNumber - (m_thinkTimeDifference) * 2;

    } 
/**************************************************************************************
* Type: (function)
* 
* Name: AssignBehaviour
* 
* 
*
* Author: Joseph Gilmore
*
* Description: used to asign behaviour to AI randomly
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 30/07/2021    JG          1.01        - created
* 1/08/2021     JG          1.10        -function remade. changed the list to contain two of each instead of trying to manaully check for duplicates using distinct().count() (theory help from will)
* **************************************************************************************/
    private void AssignBehaviour()
    {
        
        for(int i = 0; i< m_assignedBehaviours.Length; i++)
        {
            //geta a random indexs 
            int ranNum = Random.Range(0, m_setBehaviour.Count() );
           //assgins that index to a list of the AI behaviour
            m_assignedBehaviours[i] = m_setBehaviour[ranNum];
            //removes from the list meanig there can only be two of each. 
            m_setBehaviour.RemoveAt(ranNum);

        }
       
    }
}
