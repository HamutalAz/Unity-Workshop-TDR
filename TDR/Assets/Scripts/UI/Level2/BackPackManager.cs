using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackPackManager : BackPackInterface
{
    
    [SerializeField]
    LevelHandler levelHandler;

    bool dropInLoc = false;
    Vector3 location = Vector3.zero;

  
    public override async void dropOutOfBackPack(GameObject gameObject)
    {
        string objectName = imgToObjName[gameObject];
        GameObject sideBarImage = nameToImgMap[objectName];
        GameObject panelObject = gameObject;
        string levelName = SceneManager.GetActiveScene().name;
        Dictionary<string, object> data;

        Debug.Log("item " + imgToObjName[gameObject] + " about to be drop out of the back pack.");

        if (!dropInLoc)
        {
            GameObject player = DataBaseManager.instance.levelHandler.player;
            Vector3 playerPos = player.transform.position;
            Vector3 playerDirection = player.transform.forward;

            // create dictionary with the data we want to send to the DB
            data = new Dictionary<string, object>
        {
                { "owner", null },
                { "key", "owner" },
                { "playerLocationX", playerPos.x },
                { "playerLocationZ", playerPos.z },
                { "playerDirectionX", playerDirection.x},
                { "playerDirectionZ", playerDirection.z},
                { "desiredY", 0 },
                { "level" , levelName },
                { "dropInLoc" , dropInLoc }

        };
        }
        else
        {
            // create dictionary with the data we want to send to the DB
            data = new Dictionary<string, object>
        {
                { "owner", null },
                { "key", "owner" },
                { "playerLocationX", location.x },
                { "desiredY", location.y},
                { "playerLocationZ", location.z },
                { "level" , levelName },
                { "dropInLoc" , dropInLoc }
        };
        }

        // send request to server to drop object
        bool response = (bool)await DataBaseManager.instance.levelManager.LaunchRequest("dropObject", objectName, data); ;

        if (response) {

            // delete object from backpack side bar & destroy image
            nameToImgMap.Remove(objectName);
            Destroy(sideBarImage);

            // delete object from backpack panel & destroy image
            imgToObjName.Remove(panelObject);
            Destroy(panelObject);

            empty--;

            levelHandler.toggleBackPackVisability();

            dropInLoc = false;
        }
    }

    public void dropItemInLoc(Vector3 loc)
    {
        dropInLoc = true;
        location = loc;

        levelHandler.toggleBackPackVisability();

        // todo: if tab repressed - it doesn't change dropInLoc
    }

}
