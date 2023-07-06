using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class DoorPanel : Interactable
//public class RedButton : MonoBehaviour
{
    public LevelHandler levelHandler;
    [SerializeField]
    public GameObject backPack;

    // Start is called before the first frame update
    void Start()
    {
        //levelHandler.addLevelObject("doorPanel", this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void Interact()
    {
        Debug.Log("interact with doorPanel!!!");

        Vector3 loc = transform.position;

        backPack.GetComponent<BackPackManager>().dropItemInLoc(loc);

    }

    // Apply received changes
    public override void UpdateUI(Dictionary<string, object> data)
    {
        Debug.Log("updating data from the DB!");

    }


}


