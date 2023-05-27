using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackPackManager : MonoBehaviour
{
    private int empty = 0;
    [SerializeField]
    List<GameObject> sideBarPlaceHolders;
    [SerializeField]
    List<GameObject> panelPlaceHolders;
    //BackPackPanel bpPanel;
    Dictionary<string, GameObject> nameToImgMap = new();
    Dictionary<GameObject, string> imgToObjName = new();

    // backPack double click data
    float clicked = 0;
    float clicktime = 0;
    float clickdelay = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

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

                clicked++;

                if (clicked == 1)
                    clicktime = Time.time;

                if (clicked > 1 && Time.time - clicktime < clickdelay)
                {
                    clicked = 0;
                    clicktime = 0;
                    dropOutOfBackPack(img);
                }
                else if (clicked > 2 || Time.time - clicktime > 1)
                    clicked = 0;
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

    private async void dropOutOfBackPack(GameObject gameObject)
    {
        string objectName = imgToObjName[gameObject];
        GameObject sideBarImage = nameToImgMap[objectName];
        GameObject panelObject = gameObject;

        Debug.Log("item " + imgToObjName[gameObject] + " about to be drop out of the back pack.");

        GameObject player = DataBaseManager.instance.levelHandler.player;
        Vector3 playerPos = player.transform.position;
        Vector3 playerDirection = player.transform.forward;
        float spawnDistance = 1;
        string levelName = SceneManager.GetActiveScene().name;

        // create dictionary with the data we want to send to the DB
        Dictionary<string, object> data = new Dictionary<string, object>
        {
                { "owner", null },
                { "key", "owner" },
                { "playerLocationX", playerPos.x },
                { "playerLocationZ", playerPos.z },
                { "playerDirectionX", playerDirection.x},
                { "playerDirectionZ", playerDirection.z},
                { "desiredY", 0 },
                { "level" , levelName }
        };

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

            Vector3 spawnPos = playerPos + playerDirection * spawnDistance;
        }
    }

}
