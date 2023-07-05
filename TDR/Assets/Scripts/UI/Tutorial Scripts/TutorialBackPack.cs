using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialBackPack : MonoBehaviour
{
    private int empty = 0;
    [SerializeField]
    List<GameObject> sideBarPlaceHolders;
    [SerializeField]
    List<GameObject> panelPlaceHolders;
    Dictionary<string, GameObject> nameToImgMap = new();
    Dictionary<GameObject, string> imgToObjName = new();
    [SerializeField]
    tutorialHandler tutorialHandler;

    bool dropInLoc = false;
    Vector3 location = Vector3.zero;


    public void PutInBackPack(string imageName, Vector2 delta, string objName)
    {
        if (empty == sideBarPlaceHolders.Count) {
            Debug.Log("**** BACKPACK IS FULL!!!");
            return;
        }

        // put image in backpack side bar
        GameObject img = createNewObjImage(imageName, delta, sideBarPlaceHolders[empty]);
        nameToImgMap.Add(objName, img);

        // put image in backpack panel
        GameObject img2 = createNewObjImage(imageName, delta / 2.2f, panelPlaceHolders[empty]);
        imgToObjName.Add(img2, objName);

        // add event listener to clicking on the img in the panel
        addEventListener(img2);

        empty++;
        Debug.Log("item in backpack!");
    }

    static public GameObject createNewObjImage(string objName, Vector2 delta, GameObject parent)
    {
        GameObject imgObject = new GameObject(objName);

        RectTransform trans = imgObject.AddComponent<RectTransform>();
        trans.transform.SetParent(parent.transform);            // setting parent
        trans.localScale = Vector3.one;
        trans.anchoredPosition = new Vector2(0f, 0f);           // setting position - center
        trans.sizeDelta = delta;                                // custom size

        Image image = imgObject.AddComponent<Image>();
        Texture2D tex = Resources.Load<Texture2D>(objName);
        image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        trans.transform.SetParent(parent.transform);            // setting parent

        return imgObject;
    }

    private void addEventListener(GameObject img)
    {
        try
        {
            Debug.Log("adding event listener");
            EventTrigger trigger = img.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) => {

                dropOutOfBackPack(img);

            });

            trigger.triggers.Add(entry);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Data);
            Debug.Log(e.StackTrace);
        }
        Debug.Log("Event listener added");
    }

    private void dropOutOfBackPack(GameObject gameObject)
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
