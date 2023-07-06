using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialBackPack : BackPackInterface
{
    
    [SerializeField]
    tutorialHandler tutorialHandler;
    Vector3 location = Vector3.zero;

    public override void dropOutOfBackPack(GameObject gameObject)
    {
        // get thr info about the object that needs to be dropped out of backPack
        string objectName = imgToObjName[gameObject];
        GameObject sideBarImage = nameToImgMap[objectName];
        GameObject panelObject = gameObject;

        // find to player location in which the object will be dropped.
        GameObject player = tutorialHandler.player;
        Vector3 playerPos = player.transform.position;
        Vector3 playerDirection = player.transform.forward;

        var data = new Dictionary<string, object>
        {
            { "position", playerPos },
            { "direction", playerDirection }
        };

        //change location of item in the item itself.
        tutorialHandler.UpdateRoomObjectUI(objectName, data);

        // delete object from backpack side bar & destroy image
        nameToImgMap.Remove(objectName);
        Destroy(sideBarImage);

        // delete object from backpack panel & destroy image
        imgToObjName.Remove(panelObject);
        Destroy(panelObject);

        empty--;

        tutorialHandler.toggleBackPackVisability();

    }

}
