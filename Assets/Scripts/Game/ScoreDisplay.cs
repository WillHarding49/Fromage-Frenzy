using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/****************************************************************************************************
* Type: Class
* 
* Name: ScoreDisplay
*
* Author: Will Harding
*
* Description: Manages playerprefs in various ways
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 10/08/2021    WH          1.00        -Initally created
* 16/08/2021    WH          1.01        -Added functionality
* 18/08/2021    WH          1.02        -Added tooltips and 1st, 2nd, and 3rd score thresholds
****************************************************************************************************/
public class ScoreDisplay : MonoBehaviour
{
    [Tooltip("The name of the player, must be the same as the tagged player for correctly saving the scores")]
    public string m_playerName = "Cheddar (You)";
    
    [Tooltip("Congratulation object from canvas")]
    public Text m_Congratulation;

    [Tooltip("Highscore text object from canvas")]
    public Text m_highscoreText;

    [Tooltip("Score text object from canvas")]
    public Text m_scoreText;
    
    [Tooltip("Trophy gameobject containing the diffrernt trophy sprites")]
    public GameObject m_trophy;

    [Tooltip("The score needed to get 3rd place and display the correct things for 3rd place.")]
    public int m_thirdThreashold = 20;

    [Tooltip("The score needed to get 2nd place and display the correct things for 2nd place.")]
    public int m_secondThreashold = 30;

    [Tooltip("The score needed to get 1st place and display the correct things for 1st place.")]
    public int m_firstThreashold = 40;

    /**************************************************************************************
    * Type: Function
    * 
    * Name: Start
    *
    * Author: Will Harding
    *
    * Description: Calls DisplayScore
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 16/08/2021    WH          1.00        -Initial Created
    **************************************************************************************/
    public void Start()
    {
        DisplayScore();
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: DisplayScore
    *
    * Author: Will Harding
    *
    * Description: Displays score on score screen
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 16/08/2021    WH          1.00        -Initial Created
    * 18/08/2021    WH          1.01        -Added trophy and 1st, 2nd, and 3rd place score
    *                                        checks
    **************************************************************************************/
    public void DisplayScore()
    {
        //Get player's score and display it
        int score = PlayerPrefs.GetInt(m_playerName);
        m_highscoreText.text = "Highscore: " + PlayerPrefs.GetInt("Highscore");
        m_scoreText.text = "Your Score: " + score;

        //If the score is within specific bounds, display info for if the player has gotten enough points
        if (score >= m_firstThreashold)
        {
            m_Congratulation.text = "You're the big cheese!";
            m_trophy.transform.Find("1").gameObject.SetActive(true);
        }
        else if (score < m_firstThreashold && score >= m_secondThreashold)
        {
            m_Congratulation.text = "Grate job, but you can do cheddar!";
            m_trophy.transform.Find("2").gameObject.SetActive(true);
        }
        else if (score < m_secondThreashold && score >= m_thirdThreashold)
        {
            m_Congratulation.text = "Feta luck next time!";
            m_trophy.transform.Find("3").gameObject.SetActive(true);

        }
        else if (score < m_thirdThreashold)
        {
            m_Congratulation.text = "Get Gouda!";
        }
    }
}
