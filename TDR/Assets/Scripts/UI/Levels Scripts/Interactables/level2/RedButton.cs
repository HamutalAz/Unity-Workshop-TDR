using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class RedButton : Interactable
//public class RedButton : MonoBehaviour
{
    [SerializeField]
    private GameObject redLight;
    private bool isOn = false;
    public LevelHandler levelHandler;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("hereeee");
        //Debug.Log("isnull?:" + DataBaseManager.instance.levelHandler == null);
        levelHandler.addLevelObject("redLight", this);

        //addLevelObject("redLight", this);

        //DataBaseManager.instance.levelHandler.addLevelObject("redLight", this);
    }

    // Update is called once per frame
    void Update()
    {
        //if (!Input.GetKeyUp(KeyCode.E) && isOn)
        //{
        //    TurnLightOff();
        //}
    }

    protected async override void Interact()
    {
        Debug.Log("interact with button!!!");

        // create dictionary with the data we want to send to the DB
        Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "isOn", !isOn }
            };

        // send the data to the DB
        bool status = await DataBaseManager.instance.levelManager.WriteToDb("redLight", data);

        // check if sucseed
        if (!status)
        {
            Debug.Log("couldn't turn on light!!!");
        }
    }
    // Apply received changes
    public override void UpdateUI(Dictionary<string, object> data)
    {
        Debug.Log("updating data from the DB!");

        isOn = (bool)data["isOn"];

        // Update value
        redLight.GetComponent<Light>().enabled = isOn;

        Debug.Log("isOn:" + isOn);
    }

    async private void TurnLightOff()
    {
        Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "isOn", false }
            };
        await DataBaseManager.instance.levelManager.WriteToDb("redLight", data);
    }
}


