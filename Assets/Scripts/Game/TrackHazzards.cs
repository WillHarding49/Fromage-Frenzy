using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**************************************************************************************
* Type: (Class)
* 
* Name: TrackHazzard.cs
* 
* 
*
* Author: Joseph Gilmore
*
* Description: This is for hazzards on the track that the player/ai can hit and a spinout effect that the spear also uses 
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 25/06/2021    JG         1.00        -Initial Created
* 06/07/2021    JG         1.01        -class now creates a spin out effect. designer can change the amount of spins , the time and distance of the spin.
* 14/07/2021    JG         1.02        -improved trigger enter to work for AI aswell.
* 21/07/2021    JG         1.03        -added a lane number that can be applied in the inspector for Ai dection
* 27/07/2021    JG         1.04        -removed having to assgin two lanes in inspector will do in AI code instead , removed static functions and changed rotation axis 
* 02/08/2021    JG         1.05        -now allows serval racers to hit hazaard at once and updates 
* 04/08/2021    JG         1.06        -added Game Manager tag
* 17/08/2021    JG         1.10        -changed the rotation system to be more clean and remove bugs where it rotated the incorrect amount of times
**************************************************************************************/
public class TrackHazzards : MonoBehaviour
{
    #region Vars
    private Transform m_target; //target to rotate 
    private List<string> m_racerNames = new List<string>() { "Player", "AIOne", "AITwo", "AIThree", "AIFour", "AIFive" }; //list of racers 
    private List<Transform> m_rotatingRacers = new List<Transform>();//list to store current racers rotating
    private float m_finalVelocity = 0; //final velocity is always going to be 0 as we want the racers to come to a stop.
    [SerializeField][Tooltip("How long you want the spinout to last, change in comparison to other spinout values")][Range(1,5)]
    private float m_timeToSpin = 1f; //time to spin
    [SerializeField][Tooltip("The distance you want the racer to spin over")][Range(1,20)]
    private float m_ditsanceToSpin = 10f; //distance to spin over
    private float m_deceleration; //suvats deceleration amount
    private float m_intialVelocity;
    [SerializeField][Tooltip("The Amount of spins the racer will do over time to spin, the less spins the slower it will look. Change timeToSpin to compare")][Range(1, 10)]
    private float m_spinCount = 5;// amount of spins the cheese does over the slow time.
    [Tooltip("Please enter the lane you put the hazzard in, left lane being 0")][Range(0, 4)]
    public int m_laneOne; //public so it can be accesed for AI class. use to check which lane the hazzard is in
    [SerializeField][Tooltip("The amount of health changed by hitting hazzard.")][Range(-1,0)]
    private int  m_healthChange = -1;
    #endregion
    void Update()
    {
        //if hits hazzard start rotating the target 
        if ( m_rotatingRacers.Count >= 1)
        {
            //uses a list so it can rotate serveral racers at once without conflicting with each other 
            foreach (Transform racer in m_rotatingRacers)
            {
                //rotating the target a certain amount over a certain time
                racer.Rotate(0, ((360 * m_spinCount) / m_timeToSpin) * Time.deltaTime, 0);
                StartCoroutine(StopSpinAfter(racer));
            }
        }
    }
    /**************************************************************************************
    * Type: IEnumrator
    * 
    * Name: StopSpinAfter
    * 
    * 
    *
    * Author: Joseph Gilmore
    *
    * Description: called when the racer starts to spin. I used the timer instead of counting a complete rotation of the racer as its hard to ge the exact rotation and was casuing bugs
    *
    * Change Log:
    *
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    *  17/08/2021   JG          1.00        -created
    **************************************************************************************/
    IEnumerator StopSpinAfter(Transform racer)
    {
        yield return new WaitForSeconds(m_timeToSpin);
        //reset rotation and remove from list as its completed the correct amount of rotations
        racer.localRotation = Quaternion.Euler(0, 0, 0);
        m_rotatingRacers.Remove(racer);
    }
    /**************************************************************************************
    * Type: (function)
    * 
    * Name: OnCollisonEnter
    * 
    * 
    *
    * Author: Joseph Gilmore
    *
    * Description: when a racer runs over a hazzard 
    *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 06/07/2021    JG         1.00        -created
    * 02/08/2021    JG         1.01        -update  racers health when hazaards hit  chnaged from trigger to oncollision enter. also now can have mutitple racers rotate 
    * 02/08/2021    JG         1.02        -now should take away one health from each racer who enters ( bug fixed where if two enter on the same loop only one would lose health)
    * 03/08/2021    JG         1.10        -moved code to function so the spear can also call it to activate a spin out 
    **************************************************************************************/
    private void OnCollisionEnter(Collision p_other)
    {
        
        //checks to see what hits collider 
        AcitvateSpin(p_other, "hazzard");   
        
    }
    /**************************************************************************************
    * Type: (function)
    * 
    * Name: haazard hit.cs
    * 
    * 
    *
    * Author: Joseph
    *
    * Description: called when transform hits hazzard. applys effect to correct transform
    *
    * Change Log:
    *
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    *  06/07/201    JG         1.00         
    *  27/07/2021   JG         1.01      Changed calling "ChangeAcllerationSpeed" staticly so all racers are not effect by hazzard 
    *  02/08/2021   JG         1.02     removed if hit hazzard statement 
    **************************************************************************************/
    private void HazzardHit(Transform p_target)
    {
          //gets what hits hazzard
            m_target = p_target;
            //(Theory done by Will Code by JG). using suvat work out the decleration requried to stop the player depending on the time and speed inputed.
            m_deceleration = 2 * (m_finalVelocity * m_timeToSpin - m_ditsanceToSpin) / (m_timeToSpin * m_timeToSpin);
            //works out the required velocity to ensure that the target stops.
            m_intialVelocity = 2 * m_ditsanceToSpin / m_timeToSpin + m_finalVelocity;
            
            //change the racer speed and accelration
            m_target.gameObject.GetComponentInParent<CheeseMovement>().ChangeAcclerationSpeed(m_deceleration, m_intialVelocity);
            p_target.parent.Find("SpinSound").GetComponent<AudioSource>().Play();
    }
    /**************************************************************************************
  * Type: (function)
  * 
  * Name: ActivateSpin
  * 
  * 
  *
  * Author: Joseph Gilmore
  *
  * Description: (This code was in on collision entered but moved as need for spear aswell). Used for when a spin out needs to be activited 
  *
  * Change Log:
  *
  * Date          Initials    Version     Comments
  * ----------    --------    -------     ----------------------------------------------
    03/08/2021    JG          1.01        -added a checker so it only removes health if called from inside hazzard. spear needs to change health inpendently 
    04/08/2021    JG          1.02        -replaced HealhSystem tag with GameManager
    17/08/2021    JG          1.03        -now when spinout active it remove the racers pickup
    18/08/2021    JG          1.04        -removed Locked on UI
  **************************************************************************************/
    public void AcitvateSpin(Collision p_other, string p_object)
    {
        
        if (m_racerNames.Contains(p_other.collider.tag))
        {
            //checks to see if racer has already been added to a list to avoid calling dups
            if (m_rotatingRacers.Contains(p_other.collider.transform) == false)
            {
                //checks if its the hazzard as spear removes its own health
                if(p_object == "hazzard")
                {
                    //change health of collided racer 
                    GameObject.FindGameObjectWithTag("GameManager").GetComponentInParent<HealthSystem>().UpdateRacersHealth(p_other.collider.tag, m_healthChange);
                }
                //if the racer gets hit by spear or hazzard it removes the active pickup from them as punishment for hititng hazzard 
                p_other.collider.transform.GetComponentInParent<CheeseMovement>().m_activePickUp = "";
                if (p_other.collider.tag == "Player")
                {
                    //removes the locked on code from UI
                    GameObject.FindObjectOfType<UIController>().m_lockedOn = false;
                }
                //adds to a list of racers that need rotating so can do serval times 
                m_rotatingRacers.Add(p_other.collider.transform);
                //slow racer down
                HazzardHit(p_other.collider.transform);
            }
        }
    }

}
