using System.Collections;
using System.Collections.Generic;
using Google.MiniJSON;
using UnityEngine;

public class Box : Interactable
{
    [SerializeField]
    private bool isOpen = false;
    public LevelHandler levelHandler;
    // Start is called before the first frame update
    void Start()
    {
        levelHandler.addLevelObject("box", this);

        // gameObject - the box
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Interact()
    {
        // todo: the box is protected by lock - need to send a code to clous and not just a request to open

        Debug.Log("interact with box!!!");

        // create dictionary with the data we want to send to the DB
        Dictionary<string, object> data = new Dictionary<string, object>
        {
                { "isOpen", !isOpen },
                { "key", "isOpen" }
            };


        DataBaseManager.instance.levelManager.LaunchRequest("updateObject", "box", data);
    }

    // Apply received changes
    public override void UpdateUI(Dictionary<string, object> data)
    {
        Debug.Log("updating data from the DB!");

        isOpen = (bool)data["isOpen"];

        // Update value
        gameObject.GetComponent<Animator>().SetBool("isOpen", isOpen);

        Debug.Log("IsOpen:" + isOpen);
    }
}
