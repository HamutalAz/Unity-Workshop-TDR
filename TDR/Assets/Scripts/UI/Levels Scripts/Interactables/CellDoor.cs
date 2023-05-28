using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellDoor : Interactable
{
    private bool isOpen = false;
    // Start is called before the first frame update
    void Start()
    {
        //DataBaseManager.instance.levelHandler.addLevelObject("cellDoor", this);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void UpdateUI(Dictionary<string, object> data)
    {
        //isOpen = (bool)data["isOpen"];
        //gameObject.GetComponent<Animator>().SetBool("isOpen", isOpen);
    }
}
