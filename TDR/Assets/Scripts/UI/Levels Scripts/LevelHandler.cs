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

public class LevelHandler : MonoBehaviour
{
    // other players data
    private List<GameObject> otherPlayersAvatars = new List<GameObject>();
    public Dictionary<string, Vector3> playersLoc = new Dictionary<string, Vector3>();

    [SerializeField]
    public GameObject chatHandler;
    [SerializeField]
    public TextMeshProUGUI teamsPassaedLabel;
    [SerializeField]
    public GameObject backPack;
    public int backPackAvailableSlotInd = 1;

    public Dictionary<string, Interactable> levelObjects = new();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("At LevelHandler.start()");
        DataBaseManager.instance.setLevelHandler(this);
        createPlayersAvatars();
        DataBaseManager.instance.levelManager.listenOnRoomObjects();

        DataBaseManager.instance.levelManager.getTeamPassedInfo();
    }

    // create player's "avatar" and add them to the scene
    private async void createPlayersAvatars()
    {
        Dictionary<string, Vector3> playersLoc = await DataBaseManager.instance.levelManager.getOtherPlayersData();

        //Debug.Log("**** LH: createPlayersAvatars: playersLoc: ****" + playersLoc.Count);

        foreach (Vector3 playerLoc in playersLoc.Values)
        {
            GameObject referencePlayer = (GameObject)Instantiate(Resources.Load("3RdPersonPlayer"));

            // the following 2 lines generates random color to player - todo: delete when changing to avatars
            var playerRenderer = referencePlayer.GetComponent<Renderer>();
            playerRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV());

            GameObject avatar = (GameObject)Instantiate(referencePlayer, transform);
            otherPlayersAvatars.Add(avatar);

            avatar.transform.position = playerLoc;
            //Debug.Log("LH: createPlayersAvatars(): avatar created at: " + playerLoc);

            Destroy(referencePlayer);
        }

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
                avatar.transform.position = playerLoc;
            
            i++;
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

    public void UpdateRoomObjectUI(string name, Dictionary<string, object> data)
    {
        try {
        levelObjects[name].UpdateUI(data);
        }
        catch (Exception)
        {
            
        }
    }

    public void addLevelObject(string name, Interactable obj)
    {
        Debug.Log("trying to add!");
        levelObjects.Add(name, obj);
        Debug.Log("Obj added");
    }

    public void addItemToBackpack(string photoName)
    {
          

    }

    //async private void createPlayer()
    //{
    //    Vector3 loc = await DataBaseManager.instance.levelManager.getInitialPlayerLoc();
    //    GameObject referencePlayer = (GameObject)Instantiate(Resources.Load("Player"));
    //    GameObject avatar = (GameObject)Instantiate(referencePlayer, transform);

    //    avatar.transform.position = loc;
    //    Destroy(referencePlayer);
    //    (Player)referencePlayer.
    //}
}
