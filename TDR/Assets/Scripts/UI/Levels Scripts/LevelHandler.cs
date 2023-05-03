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

public class LevelHandler : MonoBehaviour
{
    // other players data
    private List<GameObject> otherPlayersAvatars = new List<GameObject>();
    public Dictionary<string, Vector3> playersLoc = new Dictionary<string, Vector3>();

    [SerializeField]
    public GameObject chatHandler;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("At LevelHandler.start()");
        DataBaseManager.instance.setLevelHandler(this);
        createPlayersAvatars();
    }

    // create player's "avatar" and add them to the scene
    private async void createPlayersAvatars()
    {
        Dictionary<string, Vector3> playersLoc = await DataBaseManager.instance.levelManager.getOtherPlayersData();
        //Dictionary<string, Vector3> playersLoc = DataBaseManager.instance.levelManager.getOtherPlayersLoc();

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
            Debug.Log("LH: createPlayersAvatars(): avatar created at: " + playerLoc);

            Destroy(referencePlayer);
        }

        // set listener on other players location & update their location
        DataBaseManager.instance.levelManager.listenOnOtherPlayersDoc();
        checkIfChatIsNeeded();
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
                //Debug.Log("**Update** playersLoc:" + playersLoc.ToString());
                //Debug.Log("**Update** avaterLoc:" + avatar.transform.position.ToString());

                avatar.transform.position = playerLoc;
                //Debug.Log("changing otherPlayerLoc to: " + playerLoc);
            }
            i++;
        }
       
    }

    public void setPlayersLoc(Dictionary<string, Vector3> playersLoc)
    {
    this.playersLoc = playersLoc;

    }   

    public void checkIfChatIsNeeded()
    {
        Debug.Log("At LevelHandler.checkIfChatIsNeeded()");
        Debug.Log("number of other players in room: " + otherPlayersAvatars.Count + " " + playersLoc.Count);
        if (otherPlayersAvatars.Count == 0)
        {
            Debug.Log("destroying chat...");
            Destroy(chatHandler);
        }
    }
}
