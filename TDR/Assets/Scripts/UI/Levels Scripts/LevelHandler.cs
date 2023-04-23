using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Firestore;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using Random = UnityEngine.Random;

public class LevelHandler : MonoBehaviour
{
    // other players data
    private List<GameObject> otherPlayersAvatars = new List<GameObject>();
    public Dictionary<string, Vector3> playersLoc = new Dictionary<string, Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        DataBaseManager.instance.setLevelHandler(this);
        DataBaseManager.instance.levelManager.getOtherPlayersData();
        createPlayersAvatars();
    }

    // create player's "avatar" and add them to the scene
    private void createPlayersAvatars()
    {
        Dictionary<string, Vector3> playersLoc = DataBaseManager.instance.levelManager.getOtherPlayersLoc();

        foreach (Vector3 playerLoc in playersLoc.Values)
        {
            GameObject referencePlayer = (GameObject)Instantiate(Resources.Load("3RdPersonPlayer"));

            // the following 2 lines generates random color to player - todo: delete when changing to avatars
            var playerRenderer = referencePlayer.GetComponent<Renderer>();
            playerRenderer.material.SetColor("_Color", UnityEngine.Random.ColorHSV());

            GameObject avatar = (GameObject)Instantiate(referencePlayer, transform);
            otherPlayersAvatars.Add(avatar);

            avatar.transform.position = playerLoc;
            Debug.Log("another player created at: " + playerLoc);

            Destroy(referencePlayer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Dictionary<string, Vector3> playersLoc = DataBaseManager.instance.levelManager.getOtherPlayersLoc();
        //Debug.Log("is null?: " + playersLoc == null);
        //Debug.Log("playerLoc: " + playersLoc.Count);

        int i = 0;

        foreach (Vector3 playerLoc in playersLoc.Values)
        {
            GameObject avatar = otherPlayersAvatars[i];

            if (playersLoc.ToString() != avatar.transform.position.ToString())
            {
                avatar.transform.position = playerLoc * Time.deltaTime;
                //Debug.Log("changing otherPlayerLoc to: " + newLoc);
            }
            i++;
        }
       
    }

    public void setPlayersLoc(Dictionary<string, Vector3> playersLoc)
    {
    this.playersLoc = playersLoc;

    }   
}
