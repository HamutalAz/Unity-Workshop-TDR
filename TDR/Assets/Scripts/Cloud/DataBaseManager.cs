using UnityEngine;
public class DataBaseManager : MonoBehaviour
{
    //Local user identification
    public static string userName;
    public static string userID;
    //game id
    //room id


    //Scene Handlers
    public WaitingRoomSceneHandler waitingRoomSceneHandler;

    //Managers
    public LoginManager loginManager;
    public WaitingRoomManager waitingRoomManager;

    //Other variables
    public static int MAXPLAYERS = 20;
    public static DataBaseManager instance;

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

}