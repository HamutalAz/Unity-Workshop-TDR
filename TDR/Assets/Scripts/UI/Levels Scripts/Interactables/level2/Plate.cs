using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Plate : Interactable
{
    [SerializeField]
    private bool isReadable = false;
    private string owner = null;
    public LevelHandler levelHandler;
    [SerializeField]
    public GameObject backPack;
    private Vector2 deltaSize = new Vector3(1800, 1200);
    [SerializeField]
    private Material visableMaterial;
    [SerializeField]
    private Material inVisableMaterial;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("at plate.start()");
        levelHandler.addLevelObject("plate", this);
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override async void Interact()
    {

        Debug.Log("**** interact with plate!!!");

        // create dictionary with the data we want to send to the DB
        Dictionary<string, object> data = new Dictionary<string, object>
        {
                { "owner", DataBaseManager.userID },
                { "key", "owner" }
            };

        // send request to DB to put the plate in backpack
        bool response = (bool) await DataBaseManager.instance.levelManager.LaunchRequest("pickUpObject", "plate", data);

    }

    // Apply received changes
    public override void UpdateUI(Dictionary<string, object> data)
    {
        Debug.Log("updating data from the DB!");

        // extract data
        owner = (string)data["owner"];
        isReadable = (bool)data["isReadable"];
        string location = (string)data["location"];

        if (owner != null) // if someone owns the plate
        {
            if (owner == DataBaseManager.userID) // if the user owns it
            {
                Debug.Log("you own the plate!");

                if(isReadable)
                    backPack.GetComponent<BackPackManager>().PutInBackPack("readablePlate", deltaSize, "plate");
                else
                    backPack.GetComponent<BackPackManager>().PutInBackPack("unreadablePlate", deltaSize, "plate");
            }
            else
            {
                Debug.Log("someone else owns the plate!");
                
            }
            // make it unactive (unvisable)
            gameObject.SetActive(false);
            Debug.Log("is plate active?" + isActiveAndEnabled);
        }
        else // the plate isn't owned by someone
        {
            //update plate's new location
            gameObject.transform.position = DataBaseManager.instance.levelManager.stringToVec(location);
            
            // change image
            if (isReadable)
                gameObject.GetComponent<MeshRenderer>().material = visableMaterial;
            else
                gameObject.GetComponent<MeshRenderer>().material = inVisableMaterial;


            //make plate active again
            gameObject.SetActive(true);

        }


        Debug.Log("owner:" + owner);
    }

}
