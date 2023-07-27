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

    // sounds effect
    public AudioClip clip;

    // Start is called before the first frame update
    void Start()
    {
        levelHandler.addLevelObject("redLight", this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override async void Interact()
    {
        Debug.Log("interact with button!!!");
        gameObject.GetComponent<AudioSource>().PlayOneShot(clip);
        
        // create dictionary with the data we want to send to the DB
        Dictionary<string, object> data = new Dictionary<string, object>
            {
                {"key", "isOn" }
            };


        bool response = (bool) await DataBaseManager.instance.levelManager.LaunchRequest("updateObject", "redLight", data);

        Debug.Log(response);
        

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


