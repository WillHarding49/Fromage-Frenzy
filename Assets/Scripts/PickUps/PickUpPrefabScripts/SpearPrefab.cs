using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**************************************************************************************
* Type: (Class)
* 
* Name: SpearPref.cs
* 
* 
*
* Author: Joseph
*
* Description: spear pick up prefab. 
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------    -------     ----------------------------------------------
* 29/06/2021    JG        1.00        -Initial Created
* 07/07/2021    JG        1.01        -addded if spear hit ground deletes (make sure the ground is tagged with track). also  added spinout but track hazzard needs chnages so its commeneted out for now.  
*`13/07/2021    JG        1.02        -object pooling added  
* 14/07/2021    JG        1.03        -adddes list of racers and timer for hitting track.
* 04/08/2021    JG        1.04        -replaced tags with GameManager
* 15/08/2021    JG        1.05        -added ToolTips
* **************************************************************************************/
public class SpearPrefab : MonoBehaviour
{
    #region Vars
    [Tooltip("The Amount of health the spear inflicts")]
    [SerializeField]
    private int m_healthChange = -1;
    private PickUpPool m_prefabPool;
    private List<string> m_racerNames = new List<string>() { "Player", "AIOne", "AITwo", "AIThree", "AIFour", "AIFive" };
    [Tooltip("time to destory object if it hits track.")]
    [SerializeField]
    private float m_destroyAfter = 0.5f; 
    [Tooltip("no need to change")]
    public  string m_activeRacer;
    public bool m_fired = false;
    #endregion
    private void Start()
    {
        m_prefabPool = FindObjectOfType<PickUpPool>();
    }
    /**************************************************************************************
    * Type: (function)
    * 
    * Name: OnCollisionEnter
    * 
    * 
    *
    * Author: Joseph
    *
    * Description: when the Spear collides with a target
    * *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 29/06/2021    JG         1.00        -Initial Created
    * 07/07/2021    JG         1.01        -added if spear hit ground deletes (make sure the ground is tagged with track). also  added spinout but track hazzard needs chnages so its commeneted out for now.  
    * 13/07/2021    JG         1.03        -object pooling added  
    * 28/07/2021    JG         1.04        -removed old timer(was not working) replaced with corountine
    * 01/08/2021    JG         1.05        -removed static call of health system 
    * 03/08/2021    JG         1.06        -changed the condiditon of when the spear has hit its target. Added spin out. 
    * 04/08/2021    JG         1.07        -replaced spearspinout & health system tag with GameManager
    * 09/08/2021    JG         1.08        -added to check if the spear is active when it hits track and has been thrown 
    * **************************************************************************************/
    private void OnCollisionEnter(Collision p_other)
    {
       
        //if the spear has been fired (avoid acedental collisions) and not collided with the racer fireing the spear
        if ( p_other.collider.tag != m_activeRacer && m_fired && m_racerNames.Contains(p_other.collider.tag))
        {
            //refers to health system to update racer health when hit by spear 
            GameObject.FindGameObjectWithTag("GameManager").GetComponentInParent<HealthSystem>().UpdateRacersHealth(p_other.collider.tag, m_healthChange);
            //activate spin out for hit target
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<TrackHazzards>().AcitvateSpin(p_other,"Spear");
            p_other.collider.transform.parent.Find("SpearHitSound").GetComponent<AudioSource>().Play();
            m_fired = false;
            //returns onject to pool
            m_prefabPool.ReturnGameObject(gameObject); 
        }
        if (p_other.collider.tag == "Track" && gameObject.activeSelf && m_fired)
        {
            //when spear hits track delete 
            StartCoroutine("DeleteSpear",m_destroyAfter);
            m_fired = false;
                
        }
    }
    /**************************************************************************************
    * Type: (IEnumerator)
    * 
    * Name: DeleteSpear
    * 
    * 
    *
    * Author: Joseph
    *
    * Description: if spear hits track delete after a small team 
    * *
    * Change Log:
    * Date          Initials    Version     Comments
    * ----------    --------    -------     ----------------------------------------------
    * 28/07/2021    JG         1.00        -Initial Created
    * **************************************************************************************/
    IEnumerator DeleteSpear(float p_time)
    {

        //sets delay to delete object 
        yield return new WaitForSeconds(p_time);
        //after delay return object
        m_prefabPool.ReturnGameObject(gameObject);
        StopCoroutine("DeleteSpear");

    }
}