using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Common;
using Unity.VisualScripting;
using UnityEngine;

public class ClipBoard : Interactable
{

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Interact()
    {
        Debug.Log("interacted with ClipBoard. TODO: Put in backpack.");
        //todo: put in backpack
    }
}
