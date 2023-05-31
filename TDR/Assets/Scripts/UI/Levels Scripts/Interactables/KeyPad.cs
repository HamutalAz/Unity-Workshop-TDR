using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Common;
using UnityEngine;

public class KeyPad : Interactable
{
    [SerializeField]
    private GameObject door;
    private bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        DataBaseManager.instance.levelHandler.addLevelObject("door", this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override async void Interact()
    {
        // create dictionary with the data we want to send to the DB
        Dictionary<string, object> data = new Dictionary<string, object>
            {
                {"key", "isOpen" }
            };

        bool response = (bool) await DataBaseManager.instance.levelManager.LaunchRequest("updateObject", "door", data);
        Debug.Log(response);
        

        //// send the data to the DB
        //bool status = await DataBaseManager.instance.levelManager.WriteToDb("door", data);

        //// check if sucseed
        //if (!status)
        //{
        //    Debug.Log("couldn't open door!!!");
        //}

    }

    // Apply received changes
    public override void UpdateUI(Dictionary<string, object> data)
    {
        //Debug.Log("updating data from the DB!");

        isOpen = (bool)data["isOpen"];

        // Update value
        door.GetComponent<Animator>().SetBool("IsOpen", isOpen);
    }
}
