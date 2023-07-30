using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
public class DataBaseManager : MonoBehaviour
{
    //Local user identification
    public static string userName;
    public static string userID;
    public static string roomID;
    public static string gameID;
    public static string avatar;

    //Scene Handlers
    public WaitingRoomSceneHandler waitingRoomSceneHandler;
    public LevelHandler levelHandler;
    public ChatHandler chatHandler;

    //Managers
    public LoginManager loginManager;
    public WaitingRoomManager waitingRoomManager;
    public LevelManager levelManager;
    public ChatManager chatManager;

    //Other variables
    public static int MAXPLAYERS = 20;
    public static DataBaseManager instance;
    public static List<string> avatarNames = new List<string> {"Boy1", "Boy2", "Boy3", "Girl1", "Girl2"};

    private void Awake()
    {
        // checking if it isn't the first scene
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    private void Start()
    {
        //Assign all cloud managers, scripts needs to added as components at DatabaseManger object in login scene.
        loginManager = GetComponent<LoginManager>();
        waitingRoomManager = GetComponent<WaitingRoomManager>();
        levelManager = GetComponent<LevelManager>();
        chatManager = GetComponent<ChatManager>();
        Debug.Log(levelManager);
    }

    private void OnDestroy()
    {
        Debug.Log("DataBaseManager destroyed!!!\n problem!!");
    }

    //setters
    public void setWaitingRoomSceneHandler(WaitingRoomSceneHandler waitingRoomInstance)
    {
        waitingRoomSceneHandler = waitingRoomInstance;
    }

    public void setLevelHandler(LevelHandler levelHandler)
    {
        this.levelHandler = levelHandler;
    }

    public void setChatHandler(ChatHandler chatHandler)
    {
        this.chatHandler = chatHandler;
    }
}