using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateControls : MonoBehaviour
{
    [Header("Active Platform")]
    public GameObject ActivePlatform; //Get active platform object to rotate

    [Header("Joystick Object")]
    [SerializeField] protected Joystick m_JoyStick; //Not using this

    [Header("Variables")]
    [SerializeField] protected float m_RotateSpeed = 0.1f; //Rotation speed

    [Header("Can Platforms Move")]
    public bool m_CanMove = true; //Bool to check if platforms can move
    [SerializeField] protected bool m_MouseControl = false;

    private GameObject m_LoseScreen; //Lose screen object
    private GameObject m_WinScreen; //Win screen object
    private Touch m_Touch;
    private Vector2 m_NewTouchPos; //Last touch position
    private Vector2 m_StartingTouchPos; //First touch position
    
 
    void Start()
    {
        //Find win and lose screens
        m_LoseScreen = GameObject.Find("LoseObject").transform.GetChild(0).gameObject;
        m_WinScreen = GameObject.Find("WinObject").transform.GetChild(0).gameObject;
    }
    void Update()
    {
        //Joystick Inputs (turns infinitely as long as you hold)
        //ActivePlatform.transform.Rotate(m_JoyStick.Vertical * m_RotateSpeed, m_JoyStick.Horizontal * m_RotateSpeed, 0.0f, Space.World);

        //Set if player can move or not
        if(m_LoseScreen.activeSelf || m_WinScreen.activeSelf)
        {
            m_CanMove = false;
        }
        else
        {
            m_CanMove = true;
        }
        //Check if movement disabled
        if(m_CanMove)
        {
            //Touch Inputs
            if(Input.touchCount > 0)
            {
                //Get Touch
                m_Touch = Input.GetTouch(0);

                //If touch moves
                if(m_Touch.phase == TouchPhase.Moved)
                {
                    //Get rotation quaternion
                    Quaternion rotation = Quaternion.Euler(m_Touch.deltaPosition.y * m_RotateSpeed * 0.1f, 0f, -m_Touch.deltaPosition.x * m_RotateSpeed * 0.1f);
                    //Rotate platform
                    ActivePlatform.transform.rotation = rotation * ActivePlatform.transform.rotation;
                } 
                         
            }
            
            //Mouse Inputs
            if(m_MouseControl)
            {
                if(Input.GetMouseButton(0))
                    ActivePlatform.transform.Rotate((Input.GetAxis("Mouse Y") * m_RotateSpeed), 0, -(Input.GetAxis("Mouse X") * m_RotateSpeed), Space.World);
            }
            
        }
        
    }
}
