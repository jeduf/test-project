using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    private Rigidbody m_Rigidbody;
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        //Disable Sleeping
        m_Rigidbody.sleepThreshold = 0.0f;
    }

    //Check collisions
    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Trap")
        {
            col.gameObject.GetComponent<Trap>().TrapActivate(col.contacts[0].point);
        }
        if(col.gameObject.tag == "WinPortal")
        {
            GameObject.FindWithTag("GameManager").GetComponent<GameManager>().WinLevel();
        }
    }

}
