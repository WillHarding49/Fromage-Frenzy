using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


/****************************************************************************************************
* Type: Class
* 
* Name: Racemanager
*
* Author: Will Harding
*
* Description: Manages when the race happens and has the end leaderboard and menu stuff
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 03/08/2021    WH          1.00        -Initally created
* 04/08/2021    WH          1.10        -Added functions
* 09/08/2021    WH          2.00        -Now derives from UIToggleParent
* 10/08/2021    WH          2.01        -Split RaceFinish into itself and a display Leaderboard
*                                        funtion and added a dictionary to store PlayerPrefs
* 15/05/2021    WH          2.02        -Changed some text to assets
* 18/08/2021    WH          2.03        -Added tooltips and changed score to a variable
* 19/08/2021    WH          2.04        -Added some comments
****************************************************************************************************/
public class RaceManager : UIToggleParent
{
    [HideInInspector]
    public bool m_racing = true;

    [Tooltip("Number of laps in the race.")]
    public int m_lapAmount;

    private int m_countdownTime = 3;

    [Tooltip("Countdown object from canvas")]
    public GameObject m_countdown;

    [Tooltip("Race Leaderboard object from canvas")]
    public GameObject m_raceLeaderboard;
    
    [Tooltip("Grand Prix Leaderboard object from canvas")]
    public GameObject m_gPLeaderboard;
    
    public GameObject m_canvas;

    private int m_racersDisplayed = 1;
    private readonly int m_numberOfRacers = 6;

    [Tooltip("Tick only on the final level, determines when to save the highscore")]
    public bool m_finalLevel = false;

    public Dictionary<string, int> m_raceLeaderboardDict = new Dictionary<string, int>();
    public Dictionary<string, int> m_gpLeaderboardDict = new Dictionary<string, int>();

    private bool m_displayGPLeaderboard = false;

    [Tooltip("The max amount of score you can have +1. Score is calculated by score minus the racer's final position in the race.")]
    public int m_scoreMax = 11;


    /**************************************************************************************
    * Type: Function
    * 
    * Name: Start
    *
    * Author: Will Harding
    *
    * Description: Starts race countdown
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 03/08/2021    WH          1.00        -Initial Created
    **************************************************************************************/
    void Start()
    {
       StartCoroutine(RaceCountDown());
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: RaceFinish
    *
    * Author: Will Harding
    *
    * Description: Adds racer to leaderboard dictionary when they finish the race
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 04/08/2021    WH          1.00        -Initial Created
    * 09/08/2021    WH          1.01        -Added button active when race ends
    * 10/08/2021    WH          2.00        -Took out leaderboard display stuff and made
    *                                        into its own function
    * 15/05/2021    WH          2.02        -Changed some text to assets
    * 16/05/2021    WH          2.03        -Added score adding between levels
    * 18/08/2021    WH          2.04        -Changed score to a variable
    **************************************************************************************/
    public void RaceFinish()
    {
        PositionTracker positionTracker = GameObject.FindWithTag("GameManager").GetComponent<PositionTracker>();

        //Get the racer's name that has just finished
        string racer = positionTracker.LeaderboardPop().transform.parent.name;

        //Gets score from previous races, if no score, default is 0
        int currentScore = PlayerPrefs.GetInt(racer, 0);

        //Set score for racer in the race leaderboard
        if (m_raceLeaderboardDict.ContainsKey(racer))
        {
            m_raceLeaderboardDict[racer] = m_scoreMax - m_racersDisplayed;
        }
        else
        {
            m_raceLeaderboardDict.Add(racer, m_scoreMax - m_racersDisplayed);
        }

        //Set score + previous score for racer in the grand prix leaderboard
        if (m_gpLeaderboardDict.ContainsKey(racer))
        {
            m_gpLeaderboardDict[racer] = currentScore + (m_scoreMax - m_racersDisplayed);
        }
        else
        {
            m_gpLeaderboardDict.Add(racer, currentScore + (m_scoreMax - m_racersDisplayed));
        }

        m_racersDisplayed++;

        //If all racers have finished, stop the race
        if (m_racersDisplayed > m_numberOfRacers)
        {
            m_racing = false;
        }
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: DisplayLeaderboard
    *
    * Author: Will Harding
    *
    * Description: Puts up leaderboard at the end of the race
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 10/08/2021    WH          1.00        -Initial Created, seperated from RaceFinish
    * 16/08/2021    WH          1.01        -Added final level highscore save
    * 17/08/2021    WH          1.02        -Removed highscore code and added to
    *                                        DisplayGPLeaderboard
    **************************************************************************************/
    public void DisplayLeaderboard()
    {
        //turn off UI and display finish text
        ToggleActive(m_canvas, new List<string> { "UI" }, "Finish!");

        //When the race has finished and you aren't displaying the grand prix leaderboard
        if (!m_racing && !m_displayGPLeaderboard)
        {
            //Turn on leaderboard
            ToggleActive(m_canvas, new List<string> { "Finish!" }, "Race Leaderboard");

            m_racersDisplayed = 1;

            //Turn on each racer text and set the name and point value in the text
            foreach (var racer in m_raceLeaderboardDict)
            {
                m_raceLeaderboard.transform.Find("Racer " + m_racersDisplayed).gameObject.SetActive(true);
                m_raceLeaderboard.transform.Find("Racer " + m_racersDisplayed).GetComponent<Text>().text = racer.Key + "        " + racer.Value;
                m_racersDisplayed++;
            }

            //Turn on the buttons
            m_raceLeaderboard.transform.Find("Buttons").gameObject.SetActive(true);
            m_racersDisplayed = 1;

            //Stop displaying the race leaderboard so the grand prix leaderboard can display
            m_displayGPLeaderboard = true;
        }
        
    }

    /**************************************************************************************
    * Type: Function
    * 
    * Name: DisplayGPLeaderboard
    *
    * Author: Will Harding
    *
    * Description: Puts up the Grand Prix leaderboard at the end of the race
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 17/08/2021    WH          1.00        -Initally created, made from DisplayLeaderboard
    **************************************************************************************/
    public void DisplayGPLeaderboard()
    {
        //Turn on each racer text and set the name and point value in the text
        foreach (var racer in m_gpLeaderboardDict.OrderByDescending(i => i.Value))
        {
            m_gPLeaderboard.transform.Find("Racer " + m_racersDisplayed).gameObject.SetActive(true);
            m_gPLeaderboard.transform.Find("Racer " + m_racersDisplayed).GetComponent<Text>().text = racer.Key + "      " + racer.Value;
            PlayerPrefs.SetInt(racer.Key, racer.Value);
            PlayerPrefs.Save();
            m_racersDisplayed++;
        }

        //If it is the final level, save the highscore if it is higher than previously saved
        if (m_finalLevel)
        {
            int playerScore = PlayerPrefs.GetInt(GameObject.FindWithTag("Player").name);
            if (PlayerPrefs.GetInt("Highscore", 0) < playerScore)
                PlayerPrefs.SetInt("Highscore", playerScore);
        }
    }

    /**************************************************************************************
    * Type: IEnumerator
    * 
    * Name: RaceCountDown
    *
    * Author: Will Harding
    *
    * Description: Starts a countdown and then starts the race when over
    * 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 04/08/2021    WH          1.00        -Initial Created, uses tcxt as a placeholder
    * 09/08/2021    WH          1.10        -Now using assests
    * 15/08/2021    WH          2.02        -Changed some text to assets
    * 16/08/2021    WH          2.03        -Minor reordering so UI appears before "Roll!"
    *                                        disappears
    **************************************************************************************/
    IEnumerator RaceCountDown()
    {
        //Loop for however many times countdownTime says
        while(m_countdownTime > 0)
        {
            //Waits 1 second before continuing
            yield return new WaitForSeconds(1f);

            //Object to activate
            string turnOn = (m_countdownTime - 1).ToString();

            //Once the countdown is 0, turn on the Roll text
            if (m_countdownTime - 1 == 0)
            {
                turnOn = "Roll!";
            }

            //Toggle active the correct objects
            ToggleActive(m_countdown, new List<string> { m_countdownTime.ToString() }, turnOn);

            m_countdownTime--;
        }

        //Starts the race
        m_racing = true;

        m_canvas.transform.Find("UI").gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        m_countdown.transform.Find("Roll!").gameObject.SetActive(false);
    }
}
