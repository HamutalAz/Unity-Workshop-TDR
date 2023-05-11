using System.Collections;
using System.Collections.Generic;
using Google.MiniJSON;
using UnityEngine;

public class Box : Interactable
{
    [SerializeField]
    private bool isOpen = false;
    [SerializeField]
    private GameObject panel;
    public LevelHandler levelHandler;
    private bool isPanelVisable = false;

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

    protected override void Interact()
    {
        Debug.Log("interact with box!!!");
        toggleVisability();
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

    public override void sendCode(string code)
    {
        Debug.Log("BOXXXXX : " + code);

        // create dictionary with the data we want to send to the DB
        Dictionary<string, object> data = new Dictionary<string, object>
        {
                { "code", code },
                { "key", "code" }
        };

        //todo: send code to serve.
        //bool res = DataBaseManager.instance.levelManager.LaunchRequest("insertCode", "box", data);
        //if (!res)
        //{
            //panel.GetComponent<InteractivePanel>().setFeedbackMessage("Incorrect code, try again.");
        //}

    }

    public void toggleVisability()
    {
        isPanelVisable = !isPanelVisable;
        panel.SetActive(isPanelVisable);
    }
}
