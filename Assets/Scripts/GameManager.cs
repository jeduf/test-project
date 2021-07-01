using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private GameObject m_LoseScreen; //Lose Screen UI
    private GameObject m_WinScreen; //Win Screen UI
    private GameObject m_PlayerBall; //Ball object (player)
    private Rigidbody m_RigidBody; //Ball rigidbody
    private Text m_LevelCounterText; // Level Counter UI
    private int m_TotalLevelCounter; //Total Level Counter Int
    private int m_ActiveLevelIndex; //Active Level Counter Int
    private Text m_PortalCounterText; //Portal Counter UI
    private int m_WinCount = 0; //Win Counter (resets every time)
    private GameObject m_LevelLoader; //Object that holds level prefabs to load
    private GameObject m_MainMenu; // Main Menu Object
    private int m_WinRecord = 0; //Win Record (Saves the progress and can be resetted from main menu)
    private RotateControls m_RotateControl;
    
    void Start()
    {
        //Get the saved win record
        m_WinRecord = PlayerPrefs.GetInt("LastLevel", 0);

        //Find main menu (must be the last object inside canvas)
        m_MainMenu = GameObject.Find("Canvas").transform.GetChild(GameObject.Find("Canvas").transform.childCount -1 ).gameObject;

        //Find lose screen
        m_LoseScreen = GameObject.Find("LoseObject").transform.GetChild(0).gameObject;

        //Find win screen
        m_WinScreen = GameObject.Find("WinObject").transform.GetChild(0).gameObject;

        //Find level counter UI
        m_LevelCounterText = GameObject.Find("LevelCounter").GetComponent<Text>();

        //Find portal counter UI
        m_PortalCounterText = GameObject.Find("PortalCounter").GetComponent<Text>();

        //Find player ball object
        m_PlayerBall = GameObject.FindWithTag("Player");

        //Find player ball rigidbody
        m_RigidBody = m_PlayerBall.GetComponent<Rigidbody>();

        //Find the object that holds levels
        m_LevelLoader = GameObject.Find("LevelLoader");

        //Find rotate control script
        m_RotateControl = this.gameObject.GetComponent<RotateControls>();

        //Find total level number
        m_TotalLevelCounter = m_LevelLoader.transform.childCount;

        //Set main menu buttons interactive based on win record
        setMainMenuButtons();

        //Set active scene number
        setActiveLevel();
        
        
    }
    void Update()
    {
        //Write active level and number of levels
        m_LevelCounterText.text = m_ActiveLevelIndex + "/" + m_TotalLevelCounter;

        //Write the number of portals gone through
        m_PortalCounterText.text = m_WinCount.ToString();

        //Stop ball and rotation of platforms if main menu is activated
        if(m_MainMenu.activeSelf == true)
        {
            m_RigidBody.constraints = RigidbodyConstraints.FreezeAll;  
            this.gameObject.GetComponent<RotateControls>().m_CanMove = false;
        }
    }
    //Lose Game Screen
    public void LoseGame()
    {
        m_LoseScreen.SetActive(true);
        m_PlayerBall.SetActive(false);
    }
    //Win Level Screen
    public void WinLevel()
    {
        m_WinCount++;
        m_WinRecord++;
        m_WinScreen.SetActive(true);
        //Freeze player to prevent rolling
        m_RigidBody.constraints = RigidbodyConstraints.FreezeAll;  
    }
    public void LoadNextLevel()
    {
        //Load next level
        try
        {
            //Disable current level
            m_LevelLoader.transform.GetChild(m_WinCount-1).gameObject.SetActive(false);

            //Find next level
            GameObject NextLevel = m_LevelLoader.transform.GetChild(m_WinCount).gameObject;

            //Enable next level
            NextLevel.SetActive(true);

            //Put ball to starting position of next level
            m_PlayerBall.transform.position = NextLevel.transform.Find("StartPos").position;

            //Disable win screen
            m_WinScreen.SetActive(false);

            //Unfreeze player ball
            m_RigidBody.constraints = RigidbodyConstraints.None;

            //Set velocity zero
            m_RigidBody.velocity = Vector3.zero;

            //Change active platform (First active platform must be the first child of the level prefab)
            m_RotateControl.ActivePlatform = NextLevel.transform.GetChild(0).gameObject;

            //Change active level counter
            m_ActiveLevelIndex++; 
            m_RotateControl.m_CanMove = true;
        }
        //If WinCount is more than the level prefabs, return to main menu
        catch (Exception)
        {
            m_WinScreen.SetActive(false);
            m_LoseScreen.SetActive(false);
            openMainMenu();
        }
        
    }
    //Load same level on lose
    public void LoadSameLevel()
    {
        //Find current level
        GameObject CurrentLevel = m_LevelLoader.transform.GetChild(m_WinCount).gameObject;

        //Put player ball at starting position
        m_PlayerBall.transform.position = CurrentLevel.transform.Find("StartPos").position;
        //Set player ball active
        m_PlayerBall.SetActive(true);

        //Disable lose screen
        m_LoseScreen.SetActive(false);

        //Unfreeze player ball
        m_RigidBody.constraints = RigidbodyConstraints.None;

        //Set velocity zero
        m_RigidBody.velocity = Vector3.zero;

        //Reset rotation of platforms
        foreach(Transform child in CurrentLevel.transform)
        {
            if(child.gameObject.name != "StartPos")
                child.eulerAngles = new Vector3(90,0,0);
        }

        //Set active platform
        this.gameObject.GetComponent<RotateControls>().ActivePlatform = CurrentLevel.transform.GetChild(0).gameObject;
        this.gameObject.GetComponent<RotateControls>().m_CanMove = true;
    }

    //Load levels from main menu
    public void LoadFromMainMenu(int level)
    {
        //Level arg starts with 1 not 0
        m_MainMenu.SetActive(false);
        
        //Set win count
        m_WinCount = (level-1);

        //Find level to load
        GameObject NextLevel = m_LevelLoader.transform.GetChild(level-1).gameObject;

        //Set other levels disabled
        int i = 0;
        foreach(Transform child in m_LevelLoader.transform)
        {
            if(i >= level)
            {
                child.gameObject.SetActive(false);
            }
        }

        //Reset positions of level platforms
        foreach(Transform child in NextLevel.transform)
        {
            if(child.gameObject.name != "StartPos")
                child.eulerAngles = new Vector3(90,0,0);
        }
        
        //Activate the level
        NextLevel.SetActive(true);

        //Set player ball position
        m_PlayerBall.transform.position = NextLevel.transform.Find("StartPos").position;

        //Unfreeze player ball
        m_RigidBody.constraints = RigidbodyConstraints.None;

        //Set velocity zero
        m_RigidBody.velocity = Vector3.zero;

        //Set active platform
        m_RotateControl.ActivePlatform = NextLevel.transform.GetChild(0).gameObject;

        //Set active level index
        m_ActiveLevelIndex = level;
        m_RotateControl.m_CanMove = true;
    }

    void openMainMenu()
    {
        m_MainMenu.SetActive(true);
        setMainMenuButtons(); //Check win record and set buttons in main menu
    }
    void OnApplicationQuit()
    {
        //Save win record
        PlayerPrefs.SetInt("LastLevel", m_WinRecord);
    }

    //Delete progress
    public void DeleteProgressBtn()
    {
        PlayerPrefs.SetInt("LastLevel", 0);
        m_WinRecord = 0;
        setMainMenuButtons();
    }

    //Set main menu buttons inactive based on win record
    void setMainMenuButtons()
    {
        int a = 0;
        foreach(Transform child in m_MainMenu.transform) //Checks all the objects in main menu (might be changed to only buttons)
        {
            if(child.gameObject.name != "DeleteProgress") //Except Delete Progress button
            {
                if(a > m_WinRecord) //If index of object is bigger than win record disable it
                {
                    child.gameObject.GetComponent<Button>().interactable = false;
                }
            }
            a++;
        }
    }

    //Find active level and set level index to its index
    void setActiveLevel()
    {
        for (int i = 0; i < m_TotalLevelCounter; i++)
        {
            if(m_LevelLoader.transform.GetChild(i).gameObject.activeSelf == true)
            {
                m_ActiveLevelIndex = i + 1;
            }
        }
    }
}
