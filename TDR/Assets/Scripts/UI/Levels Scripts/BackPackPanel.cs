using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BackPackPanel : MonoBehaviour
{
    private int empty = 0;
    [SerializeField]
    List<GameObject> placeholders;
    Dictionary<GameObject, string> imageToObjectMap = new();
    [SerializeField]
    GameObject backPack;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PutInBackPack(string imageName, Vector2 delta, string gameObjName)
    {
        Vector2 newDelta = delta / 2.2f;
        GameObject img = BackPackManager.createNewObjImage(imageName, newDelta, placeholders[empty]);
        imageToObjectMap.Add(img, gameObjName);
        addEventListener(img);

    }

    private void addEventListener(GameObject img)
    {
            try {
            Debug.Log("adding event listener");
            EventTrigger trigger = img.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) => {
                dropOutOfBackPack(img);
            });

            trigger.triggers.Add(entry);
        } catch (System.Exception e)
        {
            Debug.Log(e.Data);
            Debug.Log(e.StackTrace);
        }
        Debug.Log("Event listener added");
    }

    private async void dropOutOfBackPack(GameObject gameObject)
    {
        Debug.Log("item" + imageToObjectMap[gameObject] + " taken out of back pack.");

        // create dictionary with the data we want to send to the DB
        Dictionary<string, object> data = new Dictionary<string, object>
        {
                { "owner", null },
                { "key", "owner" }
            };

        // send request to server to drop item
        bool response = (bool)await DataBaseManager.instance.levelManager.LaunchRequest("dropObject", imageToObjectMap[gameObject], data); ;

        // todo: delete object from backpack
        backPack.GetComponent<BackPackManager>().deleteFromBackPack(imageToObjectMap[gameObject]);

        // delete from dictionary
        imageToObjectMap.Remove(gameObject);

        Destroy(gameObject);
        empty--;
    }
}
