using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**************************************************************************************
* Type: (Class)
* 
* Name: HealthSytem.cs
* 
* 
*
* Author: Joseph Gilmore
*
* Description: Used to Track health/cheese size of the racers
*
* Change Log:
* Date          Initials    Version     Comments
* ---------     --------    -----      ------------------
* 29/06/2021    JG        1.00         -Initial Created
* 01/08/2021    JG        1.01         -restructed the whole class , removed static functions , removed pointless code and added cheese accel vars insteade of magic numbers 
* 01/08/2021    WH        1.02         -Added cheese respawn
* 02/08/2021    JG        1.03         -removed magic numbers , added a scale mutiplyer. 
* 03/08/2021    Wh        1.04         -Set Default Scale to be (1, 1, 1)
* **************************************************************************************/
public class HealthSystem : MonoBehaviour
{
    #region Vars
    //cheese size acceleration multiplier 
    [SerializeField] [Tooltip("This is the amount you mutiply the default acceleration by depening on the size, change compared to other values")]
    private float m_smallCheeseAccelerationMultiplier = 2f;
    [SerializeField] [Tooltip("This is the amount you mutiply the default acceleration by depening on the size, change compared to other values")]
    private float m_normalCheeseAccelerationMultiplier = 1f;
    [SerializeField] [Tooltip("This is the amount you mutiply the default acceleration by depening on the size, change compared to other values")]
    private float m_bigCheeseAccelerationMultiplier = 0.8f;
    [SerializeField] [Tooltip("This is the amount you mutiply the default acceleration by depening on the size, change compared to other values")]
    private float m_fattyCheeseAccelerationMultiplier = 0.5f;
    //cheese scale multiplier 
    [SerializeField] [Tooltip("This is the amount you mutiply the scale by to get the size you want for the cheese, keep values consistent with the other scale multiplyer")]
    private float m_smallCheeseScaleMultiplyer = 0.5f;
    [SerializeField] [Tooltip("This is the amount you mutiply the scale by to get the size you want for the cheese, keep values consistent with the other scale multiplyer")]
    private float m_bigCheeseScaleMultiplyer = 1.5f;
    [SerializeField] [Tooltip("This is the amount you mutiply the scale by to get the size you want for the cheese, keep values consistent with the other scale multiplyer")]
    private float m_fattyCheeseScaleMultiplyer = 2f;
    //racers default health
    [SerializeField] [Tooltip("The health for each racer, this changes the scale and acceleration. There is only four different health states that increase in ones. current default is 2")] [Range(1, 4)]
    public int m_playerHealth = 2;
    [SerializeField] [Tooltip("The health for each racer, this changes the scale and acceleration. There is only four different health states that increase in ones. current default is 2")][Range(1, 4)]
    private int m_aiOneHealth = 2;
    [SerializeField][Tooltip("The health for each racer, this changes the scale and acceleration. There is only four different health states that increase in ones. current default is 2")][Range(1, 4)]
    private  int m_aiTwoHealth = 2;
    [SerializeField][Tooltip("The health for each racer, this changes the scale and acceleration. There is only four different health states that increase in ones. current default is 2")][Range(1, 4)]
    private int m_aiThreeHealth = 2;
    [SerializeField][Tooltip("The health for each racer, this changes the scale and acceleration. There is only four different health states that increase in ones. current default is 2")][Range(1, 4)]
    private  int m_aiFourHealth = 2;
    [SerializeField][Tooltip("The health for each racer, this changes the scale and acceleration. There is only four different health states that increase in ones. current default is 2")][Range(1, 4)]
    private  int m_aiFiveHealth = 2;
    [SerializeField][Tooltip("Default scale , I reccomend not changing but if you do Ensure you raise the path")]
    private Vector3 m_defaultScale = new Vector3(1, 1, 1);
    #endregion
    
    /**************************************************************************************
    * Type: (function)
    * 
    * Name: UpdateRacersHealth
    * 
    * 
    *
    * Author: Joseph Gilmore
    *
    * Description: when function called it passes throguh the corret raccers and changes its health. 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    *----------    --------    -------     ----------------------------------------------
    * 29/06/2021   JG          1.00        -Initial Created
    * 08/07/2021   JG          1.01        -add change speed depending on player size 
    * 27/07/2021   JG          1.02        -call check size for all racers not just the player
    * 11/08/2021   JG          1.03        -added particle effects for loosing/gaining health. fixed naming error of p_healthChange
    * **************************************************************************************/

    public  void UpdateRacersHealth(string p_racer, int p_healthChange)
    {
        if(p_healthChange == 1)
        {
            //if gaining health play gain health particles 
            GameObject.FindGameObjectWithTag(p_racer).transform.parent.Find("HealthGain").GetComponent<ParticleSystem>().Play();
        }
        if (p_healthChange == -1)
        {
            //if player take damage plays damage particle effects
            GameObject.FindGameObjectWithTag(p_racer).transform.parent.Find("HealthLose").GetComponent<ParticleSystem>().Play();
        }
        //changes the correct racers health and the updates it scale 
        switch (p_racer)
        {
            case "Player":
                m_playerHealth += p_healthChange;
                CheckSize(m_playerHealth, "Player");
            break;
            case "AIOne":
                m_aiOneHealth += p_healthChange;
                CheckSize(m_aiOneHealth, "AIOne");
                break;
            case "AITwo":
                m_aiTwoHealth += p_healthChange;
                CheckSize(m_aiTwoHealth, "AITwo");
                break;
            case "AIThree":
                m_aiThreeHealth += p_healthChange;
            CheckSize(m_aiThreeHealth, "AIThree");
                break;
            case "AIFour":
                m_aiFourHealth += p_healthChange;
                CheckSize(m_aiFourHealth, "AIFour");
                break;
            case "AIFive":
                m_aiFiveHealth += p_healthChange; 
            CheckSize(m_aiFiveHealth, "AIFive");
                break;

        }
    }
    /**************************************************************************************
    * Type: (function)
    * 
    * Name: CheckSize
    * 
    * 
    *
    * Author: Joseph Gilmore
    *
    * Description: used to check cheese size, change accelration of transform and scale
    *
    * Change Log:
    * Date          Initials    Version     Comments
    *----------    --------    -------     ----------------------------------------------
    * 08/07/2021   JG          1.01        -added change speed depending on player size 
    * 27/07/2021   JG          1.02        -Change Accelration in cheese movement is no longer static
    * 01/08/2021   JG          1.03        -removed static call and using a mutplier instead of a default acel
    * 01/08/2021   WH          1.04        -Added respawn thing that should work but is untested
    * 02/08/2021   JG          1.05        -removed magic numbers for scale and added a mutiplyer. Also now does not scale on the y so no need to change y pos
    * **************************************************************************************/
    public  void CheckSize(int p_racerHealth , string p_racer)
    {
        Transform tempRacer = GameObject.FindGameObjectWithTag(p_racer).transform;
        switch (p_racerHealth)
        {
            case 0:
                //the racer has 0 health call respawn and reset health
                tempRacer.gameObject.GetComponentInParent<CheeseMovement>().Respawn();
                tempRacer.parent.Find("DeathSound").GetComponent<AudioSource>().Play();
                UpdateRacersHealth(p_racer, 2);
                break;
            case 1:
                //change scale and acceleration depending on the size
                tempRacer.gameObject.GetComponentInParent<CheeseMovement>().ChangeAcceleration(m_smallCheeseAccelerationMultiplier);
                tempRacer.localScale = new Vector3(m_defaultScale.x * m_smallCheeseScaleMultiplyer, m_defaultScale.y, m_defaultScale.z * m_smallCheeseScaleMultiplyer);
                break;
            case 2:
                tempRacer.gameObject.GetComponentInParent<CheeseMovement>().ChangeAcceleration(m_normalCheeseAccelerationMultiplier);
                tempRacer.localScale = m_defaultScale;
                break;
            case 3:
                tempRacer.gameObject.GetComponentInParent<CheeseMovement>().ChangeAcceleration(m_bigCheeseAccelerationMultiplier);
                tempRacer.localScale = new Vector3(m_defaultScale.x * m_bigCheeseScaleMultiplyer, m_defaultScale.y, m_defaultScale.z * m_bigCheeseScaleMultiplyer);
                break;
            case 4:
                tempRacer.gameObject.GetComponentInParent<CheeseMovement>().ChangeAcceleration(m_fattyCheeseAccelerationMultiplier);
                tempRacer.localScale = new Vector3(m_defaultScale.x * m_fattyCheeseScaleMultiplyer, m_defaultScale.y , m_defaultScale.z * m_fattyCheeseScaleMultiplyer);
                break;

        }

    }
    
} 
