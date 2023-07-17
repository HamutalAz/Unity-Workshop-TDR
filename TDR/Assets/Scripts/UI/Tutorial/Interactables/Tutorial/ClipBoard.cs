using System.Collections;
using System.Collections.Generic;
//using Palmmedia.ReportGenerator.Core.Common;
using Unity.VisualScripting;
using UnityEngine;

public class ClipBoard : Interactable
{
    [SerializeField]
    public GameObject backPack;
    [SerializeField]
    public tutorialHandler tHandler;
    bool wasTakenBefore = false;
    bool wasDroppedBefore = false;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
        tHandler.addLevelObject("ClipBoard", this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Interact()
    {
        Debug.Log("interacted with ClipBoard. TODO: Put in backpack.");

        //put in backpack
        backPack.GetComponent<TutorialBackPack>().PutInBackPack("clipboard", new Vector2(900f, 900f), "ClipBoard");

        if (!wasTakenBefore)
            tHandler.showNextGuidePanel(6);

        //wasTakenBefore = true;
        gameObject.SetActive(false);
    }

    public override void UpdateUI(Dictionary<string, object> data)
    {
        gameObject.SetActive(true);
        Vector3 position = (Vector3)data["position"] + Vector3.forward;
        position.y = 0;

        if (!wasDroppedBefore)
            tHandler.showNextGuidePanel(7);

        //wasDroppedBefore = true;

        gameObject.transform.position = position;
    }

}
