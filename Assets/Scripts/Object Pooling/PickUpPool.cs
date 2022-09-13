using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**************************************************************************************
* Type: (Class)
* 
* Name: PickUpPool.cs
* 
* 
*
* Author: Joseph Gilmore
*
* Description: Class purpose is to create a pool of prefab objects queue to set active when needed instead of destory/instiate 
*
* Change Log:
* Date          Initials    Version     Comments
* ----------    --------   -------     ---------------------------------
* 13/07/2021    JG        1.00        - Used as reference and altered for project: www.youtube.com. (n.d.). Fast & Efficient Spawning with Object Pooling - Unity and C#. [online] Available at: https://www.youtube.com/watch?v=wGgeCki1vC8 [Accessed 13 Jul. 2021].‌
* **************************************************************************************/
public class PickUpPool : MonoBehaviour
{
    private Dictionary<string, Queue<GameObject>> m_prefabPool = new Dictionary<string, Queue<GameObject>>(); // creating  a queue of objects which will be filled with item prefabs

 /**************************************************************************************
* Type: (function)
* 
* Name: GetGameObject.cs
* Author: Joseph Gilmore
*
* Description: function is for the pickup classes to active gameobject from the pool
*
* Change Log:
* Date          Initials    Version     Comments
* ---------     -----       ------       ------------------
* 13/07/2021    JG          1.00        - Used as reference and altered for project: www.youtube.com. (n.d.). Fast & Efficient Spawning with Object Pooling - Unity and C#. [online] Available at: https://www.youtube.com/watch?v=wGgeCki1vC8 [Accessed 13 Jul. 2021].‌
* **************************************************************************************/
    public GameObject GetObject(GameObject p_gameObject)
    {
        //checks to see if the prefab name already exists in the dicionary
        if (m_prefabPool.TryGetValue(p_gameObject.name, out Queue<GameObject> objectList))
        {
            //if the Queue currently does not have any entires  for that prefab call function
            if (objectList.Count == 0)
                return CreateNewGameObject(p_gameObject);
            else
            {
                //if it does remove from Q and set to active 
                GameObject tempObject = objectList.Dequeue();
                tempObject.SetActive(true);
                return tempObject;
            }
        }
        //create an object as it has not been made yet
        else
            return CreateNewGameObject(p_gameObject);
   }
    //used to create the game pobject from the passed through prefab
   private GameObject CreateNewGameObject(GameObject p_gameObject)
   {
        //creates the new Object requseted by pickup scripts
        GameObject prefabInstance = Instantiate(p_gameObject);
        //resets the name to remove the number at the end if there is more than one. This is so we can always find the prefab name in the Dictionary
        prefabInstance.name = p_gameObject.name;
        return prefabInstance;
   }
 /**************************************************************************************
* Type: (function)
* 
* Name: PickUpPool.cs
* 
* 
*
* Author: Joseph Gilmore
*
* Description: Used  to manage the pool and ensure each object has the correct Queue
*
* Change Log:
*  
* 
* Date          Initials    Version     Comments
* ------       --------    ------       ------------------
* 13/07/2021    JG        1.00        - Used as reference and altered for project: www.youtube.com. (n.d.). Fast & Efficient Spawning with Object Pooling - Unity and C#. [online] Available at: https://www.youtube.com/watch?v=wGgeCki1vC8 [Accessed 13 Jul. 2021].‌
* **************************************************************************************/
    public void ReturnGameObject(GameObject p_gameObject)
    {
        //checks to see if a queue already exists for that object
        if(m_prefabPool.TryGetValue(p_gameObject.name,out Queue<GameObject> ObjectList))
        {
            //add to existing Queue 
            ObjectList.Enqueue(p_gameObject);

        }
        else
        {
            //if it does not create new queue  for that  object and than add to the pool. This mean each object has it own Q.
            Queue<GameObject> tempObjectQueue = new Queue<GameObject>();
            //add object to queue
            tempObjectQueue.Enqueue(p_gameObject);
            //add both to pool
            m_prefabPool.Add(p_gameObject.name, tempObjectQueue);
        }
        //ensure not active while in queue 
        p_gameObject.SetActive(false);
    }
   
}
