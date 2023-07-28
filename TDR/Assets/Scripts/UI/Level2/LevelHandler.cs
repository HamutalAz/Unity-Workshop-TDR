using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Firestore;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.ProBuilder.Shapes;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Sunbox.Avatars;
using System.Xml;
using static UnityEngine.EventSystems.EventTrigger;

public class LevelHandler : SceneHandler
{
    // other players data
    private List<GameObject> otherPlayersAvatars = new List<GameObject>();
    public Dictionary<string, Vector3> playersLoc = new Dictionary<string, Vector3>();
    public Dictionary<GameObject, string> avatarToUsername = new Dictionary<GameObject, string>();


    [SerializeField]
    public GameObject chatHandler;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("At LevelHandler.start(), levelObjects.size(): " + levelObjects.Count);
        DataBaseManager.instance.setLevelHandler(this);
        
        createPlayersAvatars();

        DataBaseManager.instance.levelManager.listenOnRoomObjects();
        DataBaseManager.instance.levelManager.listenOnRoomDocument();
        DataBaseManager.instance.levelManager.getTeamPassedInfo();
    }

    // create player's "avatar" and add them to the scene
    private async void createPlayersAvatars()
    {
        Dictionary<string, Vector3> playersLoc = await DataBaseManager.instance.levelManager.getOtherPlayersData();

        foreach (KeyValuePair<string, Vector3> entry in playersLoc)
        {
            Vector3 playerLoc = entry.Value;

            // create the avatar
            GameObject referencePlayer = (GameObject)Instantiate(Resources.Load("Avatar"));
            GameObject avatar = (GameObject)Instantiate(referencePlayer, transform);

            // set it's location
            Vector3 newLoc = new Vector3(playerLoc.x, 0, playerLoc.z);
            Debug.Log("******* about to put another player in:" + newLoc);
            avatar.transform.position = newLoc;

            // add to list & dictionary
            otherPlayersAvatars.Add(avatar);
            avatarToUsername.Add(avatar, entry.Key);

            Destroy(referencePlayer);
        }


        //foreach (Vector3 playerLoc in playersLoc.Values)
        //{
        //    GameObject referencePlayer = (GameObject)Instantiate(Resources.Load("Avatar"));

        //    // the following 2 lines generates random color to player - todo: delete when changing to avatars
        //    //var playerRenderer = referencePlayer.GetComponent<Renderer>();
        //    //playerRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV());

        //    GameObject avatar = (GameObject)Instantiate(referencePlayer, transform);
        //    otherPlayersAvatars.Add(avatar);

        //    Vector3 newLoc = new Vector3(playerLoc.x, 0, playerLoc.z);
        //    Debug.Log("******* about to put another player in:" + newLoc);
        //    avatar.transform.position = newLoc;

        //    Destroy(referencePlayer);
        //}

        // set listener on other players location & update their location
        DataBaseManager.instance.levelManager.listenOnOtherPlayersDoc();
        CheckIfChatIsNeeded();
        DataBaseManager.instance.levelManager.listenOnGameDocument();
    }

    // Update is called once per frame
    void Update()
    {
        Dictionary<string, Vector3> playersLoc = DataBaseManager.instance.levelManager.getOtherPlayersLoc();

        if (otherPlayersAvatars.Count == 0)
            return;


        int i = 0;

        foreach (Vector3 playerLoc in playersLoc.Values)
        {
            GameObject avatar = otherPlayersAvatars[i];

            if (playerLoc.ToString() != avatar.transform.position.ToString())
            {
                Vector3 newLoc = new Vector3(playerLoc.x, 0, playerLoc.z);
                avatar.GetComponent<ThirdPlayerAvatarController>().setNewLoc(newLoc);
            }

            i++;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            toggleBackPackVisability();
        }

    }

    public void SetPlayersLoc(Dictionary<string, Vector3> playersLoc)
    {
        this.playersLoc = playersLoc;

    } 

    public void CheckIfChatIsNeeded()
    {
        Debug.Log("At LevelHandler.checkIfChatIsNeeded()");
        Debug.Log("number of other players in room: " + otherPlayersAvatars.Count + " " + playersLoc.Count);
        if (otherPlayersAvatars.Count == 0)
        {
            Debug.Log("destroying chat...");
            Destroy(chatHandler);
        }
    }

    public void UpdateTeamPassedLabel(int passed, int total)
    {
        teamsPassaedLabel.text = passed + "/" + total + " Teams passed";
    }

    public async void moveScene(string scene)
    {
        await Task.Delay(5000);
        SceneManager.LoadScene(sceneName: scene);
    }

    //public Vector3 fetchPlayerLoc(GameObject obj)
    //{
    //    // find the index in the list
    //    if (avatarToUsername[obj] == null)
    //    {
    //        Debug.Log("****** Couldn't find the avatar!!!");
    //        return Vector3.zero;
    //    }

    //    return playersLoc[avatarToUsername[obj]];
    //}

}
