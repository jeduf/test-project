using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] protected enum m_trapType {Bouncer, FireTrap, Other}; //Select trap type
    [Header("Trap Type")]
    [SerializeField] protected m_trapType m_TrapType;

    [Header("Bouncer Bounce Force")]
    [SerializeField] protected float BounceForce = 5; //Force of bouncer trap
    
    //Activate trap
    public void TrapActivate(Vector3 vec = default(Vector3))
    {
        //Bouncer trap
        if(m_TrapType == m_trapType.Bouncer)
        {
            GetComponent<Animator>().SetBool("isBouncing", true); //Play anim
            StartCoroutine("falseAnim"); //Set anim bool to false after 0.1f
            GameObject.FindWithTag("Player").GetComponent<Rigidbody>().AddExplosionForce(BounceForce * 100, vec, 1); //Push player back
        }

        //Fire trap
        else if (m_TrapType == m_trapType.FireTrap)
        {
            GameObject Explosion = GameObject.Find("Explosion"); //Find explosion
            Explosion.transform.position = GameObject.FindWithTag("Player").transform.position; //Get explosion to player's position
            Explosion.GetComponent<ParticleSystem>().Play(); //Play explosion particle
            GameObject.FindWithTag("GameManager").GetComponent<GameManager>().LoseGame(); //Load lose game screen
        }
        //Other traps
        else
        {
            GameObject.FindWithTag("GameManager").GetComponent<GameManager>().LoseGame(); //Load lose game screen
        }
        
    }

    //Wait for 0.1f to set bool false
    IEnumerator falseAnim()
    {
        yield return new WaitForSeconds(0.1f);
        GetComponent<Animator>().SetBool("isBouncing", false);
    }
}
