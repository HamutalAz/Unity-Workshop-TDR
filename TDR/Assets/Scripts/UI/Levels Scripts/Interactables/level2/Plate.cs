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

    // Start is called before the first frame update
    void Start()
    {
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

        owner = (string)data["owner"];

        // put in backPack or set visable false if belongs to someone else.
        if(owner != null)
        {
            if (owner == DataBaseManager.userID)
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
            gameObject.SetActive(!gameObject.activeSelf);
            Debug.Log("is plate activee?" + isActiveAndEnabled);
        }

        Debug.Log("owner:" + owner);
    }
}
