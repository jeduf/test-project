using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPortalEnterance : MonoBehaviour
{
    [Header("Exit Portal Position")]
    [SerializeField] protected Transform m_TeleportPos = null; //Portal exit position
    protected GameObject m_NextPlatform = null; //Next platform to control

    private GameObject manager; //Game Manager

    void Start()
    {
        //Find Game Manager
        manager = GameObject.FindWithTag("GameManager");

        //Find which platform to control from exit portal
        m_NextPlatform = m_TeleportPos.transform.parent.gameObject;
    }

    //If player ball enters collision teleport it to exit portal
    void OnCollisionEnter(Collision col)
    {
        
        if(col.gameObject.tag == "Player")
        {
            if (m_TeleportPos != null)
            {
                col.gameObject.transform.position = m_TeleportPos.GetChild(0).position; //Find exit location from portal
                manager.GetComponent<RotateControls>().ActivePlatform = m_NextPlatform; //Change active platform

            }  
        }
    }

}
