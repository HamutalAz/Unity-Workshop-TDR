using System.Collections;
using System.Collections.Generic;
using Google.MiniJSON;
using UnityEngine;
using UnityEngine.UIElements;

public class Box : Interactable
{
    [SerializeField]
    private bool isOpen = false;
    [SerializeField]
    private GameObject panel;
    public LevelHandler levelHandler;
    private bool isPanelVisable = false;
    [SerializeField]
    private GameObject plate;


    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(isPanelVisable);
        levelHandler.addLevelObject("box", this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override async void Interact()
    {
        panel.GetComponent<InteractivePanel>().setFeedbackMessage("");
        Debug.Log("At: Box::Interact");
       
        // create dictionary with the data we want to send to the DB
        Dictionary<string, object> data = new Dictionary<string, object>
        {
                { "key", "owner" }
        };
        //send request to try and capture the panel!
        bool response = (bool) await DataBaseManager.instance.levelManager.LaunchRequest("pickUpObject", "box", data);
        if (response)
        {
            toggleVisability();
        }
        
        
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

    async public override void sendCode(Dictionary<string, object> data)
    {
        bool response = (bool)await DataBaseManager.instance.levelManager.LaunchRequest("checkCode", "box", data);
        if (response)
        {
            toggleVisability(); // disable code panel
            gameObject.GetComponent<BoxCollider>().enabled = false;  //disable box interaction.
            plate.GetComponent<BoxCollider>().enabled = true; // start plate interaction.
        }
        else
            panel.GetComponent<InteractivePanel>().setFeedbackMessage("Wrong code, try again.");
        
    }

    public void toggleVisability()
    {
        isPanelVisable = !isPanelVisable;

        // able/disable player's movment
        levelHandler.togglePlayerInputSystem(isPanelVisable);

        panel.SetActive(isPanelVisable);

    }

    async public void dropObject(Dictionary<string, object> data)
    {
        bool response = (bool)await DataBaseManager.instance.levelManager.LaunchRequest("dropObject", "box", data);
        if (response)
            toggleVisability();

        Debug.Log(response);
    }

   
}
