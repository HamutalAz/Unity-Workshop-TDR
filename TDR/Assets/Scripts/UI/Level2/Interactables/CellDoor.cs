using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellDoor : Interactable
{
    public LevelHandler levelHandler;
    private bool isOpen = false;

    // sounds effect
    public AudioClip clip;

    // Start is called before the first frame update
    void Start()
    {
        levelHandler.addLevelObject("cellDoor", this);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void UpdateUI(Dictionary<string, object> data)
    {
        isOpen = (bool)data["isOpen"];
        if(isOpen)
            gameObject.GetComponent<AudioSource>().PlayOneShot(clip);
        gameObject.GetComponent<Animator>().SetBool("isOpen", isOpen);
    }
}
